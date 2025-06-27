using System.Reflection;
using System.Xml.Linq;

public record SearchResult(string Name, string Kind);
public record MemberDetail(string Signature, string XmlDocumentation);

public static class MetadataService
{
    public static IEnumerable<SearchResult> Search(string pkg, string ver, string framework, string query)
    {
        var (asm, docs) = LoadPackage(pkg, ver, framework);
        if (asm == null)
            return Enumerable.Empty<SearchResult>();
        var results = new List<SearchResult>();
        foreach (var type in asm.GetTypes())
        {
            if (type.FullName != null && type.FullName.Contains(query, StringComparison.OrdinalIgnoreCase))
                results.Add(new SearchResult(type.FullName, "Type"));
            foreach (var member in type.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
            {
                if (member.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
                    results.Add(new SearchResult($"{type.FullName}.{member.Name}", member.MemberType.ToString()));
            }
        }
        return results;
    }

    public static MemberDetail? GetMemberDetails(string pkg, string ver, string framework, string memberName)
    {
        var (asm, docs) = LoadPackage(pkg, ver, framework);
        if (asm == null)
            return null;
        var lastDot = memberName.LastIndexOf('.');
        if (lastDot < 0)
            return null;
        var typeName = memberName.Substring(0, lastDot);
        var memberShort = memberName.Substring(lastDot + 1);
        var type = asm.GetType(typeName);
        if (type == null)
            return null;
        var member = type.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .FirstOrDefault(m => m.Name == memberShort);
        if (member == null)
            return null;
        var signature = member.ToString();
        var xmlId = GetXmlDocId(member);
        var xml = docs != null ? docs.Root?.Element("members")?.Elements("member")?.FirstOrDefault(e => e.Attribute("name")?.Value == xmlId)?.ToString() ?? string.Empty : string.Empty;
        return new MemberDetail(signature ?? string.Empty, xml);
    }

    static (Assembly? asm, XDocument? xml) LoadPackage(string pkg, string ver, string framework)
    {
        var basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget", "packages", pkg.ToLower(), ver, "lib", framework);
        if (!Directory.Exists(basePath))
            return (null, null);
        var dll = Directory.GetFiles(basePath, "*.dll").FirstOrDefault();
        if (dll == null)
            return (null, null);
        var xmlPath = Path.ChangeExtension(dll, ".xml");
        var resolver = new PathAssemblyResolver(new[] { dll, typeof(object).Assembly.Location });
        var mlc = new MetadataLoadContext(resolver);
        var asm = mlc.LoadFromAssemblyPath(dll);
        XDocument? xml = null;
        if (File.Exists(xmlPath))
            xml = XDocument.Load(xmlPath);
        return (asm, xml);
    }

    static string GetXmlDocId(MemberInfo member)
    {
        return member switch
        {
            Type t => "T:" + t.FullName,
            MethodBase m => "M:" + m.DeclaringType!.FullName + "." + m.Name + FormatParameters(m.GetParameters()),
            PropertyInfo p => "P:" + p.DeclaringType!.FullName + "." + p.Name,
            FieldInfo f => "F:" + f.DeclaringType!.FullName + "." + f.Name,
            EventInfo e => "E:" + e.DeclaringType!.FullName + "." + e.Name,
            _ => member.Name
        };
    }

    static string FormatParameters(ParameterInfo[] parameters)
    {
        if (parameters.Length == 0)
            return string.Empty;
        var names = parameters.Select(p => p.ParameterType.FullName ?? p.ParameterType.Name);
        return "(" + string.Join(",", names) + ")";
    }
}
