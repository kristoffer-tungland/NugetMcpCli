using System;
using System.IO;
using Xunit;
using TestPackage;

public class MetadataServiceTests
{
    [Fact]
    public void Search_Finds_Type()
    {
        var root = SetupPackage();
        var results = MetadataService.Search("TestPackage", "1.0.0", "net9.0", "TestClass", packagesRoot: root);
        Assert.Contains(results, r => r.Name == "TestPackage.TestClass" && r.Kind == "Type");
    }

    [Fact]
    public void GetMemberDetails_Returns_Documentation()
    {
        var root = SetupPackage();
        var detail = MetadataService.GetMemberDetails("TestPackage", "1.0.0", "net9.0", "TestPackage.TestClass.SayHello", packagesRoot: root);
        Assert.NotNull(detail);
        Assert.Contains("SayHello", detail!.Signature);
        Assert.Contains("summary", detail.XmlDocumentation);
    }

    private static string SetupPackage()
    {
        var temp = Path.Combine(Path.GetTempPath(), "pkg_" + Guid.NewGuid().ToString("N"));
        var packageDir = Path.Combine(temp, "testpackage", "1.0.0", "lib", "net9.0");
        Directory.CreateDirectory(packageDir);
        var asmPath = typeof(TestClass).Assembly.Location;
        File.Copy(asmPath, Path.Combine(packageDir, "TestPackage.dll"));
        var xmlPath = Path.ChangeExtension(asmPath, ".xml");
        if (File.Exists(xmlPath))
            File.Copy(xmlPath, Path.Combine(packageDir, "TestPackage.xml"));
        return temp;
    }
}
