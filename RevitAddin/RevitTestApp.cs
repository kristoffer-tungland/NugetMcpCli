using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

namespace RevitAddin
{
    public class RevitTestApp : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            var msg = PipeClient.WaitForMessage();
            if (msg != null && msg.Command == "RunTests")
            {
                var uiapp = new UIApplication(application.ControlledApplication);
                var resultPath = RevitNUnitExecutor.ExecuteTestsInRevit(msg.TestAssembly, uiapp);
                PipeClient.SendResult(resultPath);
            }
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
