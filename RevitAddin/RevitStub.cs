namespace Autodesk.Revit.UI
{
    public class UIApplication
    {
        public object ControlledApplication { get; }
        public UIApplication(object app) => ControlledApplication = app;
    }

    public interface IExternalApplication
    {
        Autodesk.Revit.UI.Result OnStartup(UIControlledApplication application);
        Autodesk.Revit.UI.Result OnShutdown(UIControlledApplication application);
    }

    public class UIControlledApplication
    {
        public object ControlledApplication { get; } = new object();
    }

    public enum Result { Succeeded, Failed, Cancelled }
}

namespace Autodesk.Revit.DB
{
    public class Document { }
}
