using System.Collections.Generic;
using System.Web.Routing;
using IronJS;

namespace IronJSMVC.Extensions
{
    public static class CommonObjectExtensions
    {
        public static RouteValueDictionary ToRouteValueDictionary(this CommonObject obj)
        {
            return new RouteValueDictionary((IDictionary<string, object>)obj.Members);
        }
    }
}
