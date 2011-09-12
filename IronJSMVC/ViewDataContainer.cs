using System.Web.Mvc;

namespace IronJSMVC
{
    public class ViewDataContainer : IViewDataContainer
    {
        public ViewDataContainer(ViewDataDictionary viewData)
        {
            this.ViewData = viewData;
        }

        public ViewDataDictionary ViewData { get; set; }
    }
}
