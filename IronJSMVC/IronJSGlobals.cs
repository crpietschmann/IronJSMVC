using System;
using IronJS.Hosting;
using IronJS.Native;
using System.Web;

namespace IronJSMVC
{
    public static class IronJSGlobalMethods
    {
        public static void RegisterGlobals(CSharp.Context context, HttpContextBase httpContext)
        {
            context.Globals.Put("include", Utils.CreateFunction<Action<string>>(context.Environment, 1, s => { IronJSGlobalMethods.Include(httpContext, context, s); }));
        }

        public static object Include(HttpContextBase httpContext, IronJS.Hosting.CSharp.Context context, string moduleName)
        {
            var filepath = httpContext.Server.MapPath(string.Format("~/Controllers/{0}.js", moduleName));
            object obj = null;
            try
            {
                obj = context.ExecuteFile(filepath);
            }
            catch (IronJS.Error.Error ex)
            {
                throw new Exception("Source Module: " + moduleName + "\n\n" + ex.ToString(), ex);
            }

            return obj;
        }
    }
}
