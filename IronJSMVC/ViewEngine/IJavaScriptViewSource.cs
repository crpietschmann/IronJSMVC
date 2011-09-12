using System.Web.Mvc;

// Credited to: http://haacked.com/archive/2009/04/22/scripted-db-views.aspx

namespace IronJSMVC.ViewEngine
{
    public interface IJavaScriptViewSource
    {
        string GetViewContents(ControllerContext controllerContext, string path);
        bool FileExists(ControllerContext controllerContext, string path);
    }
}
