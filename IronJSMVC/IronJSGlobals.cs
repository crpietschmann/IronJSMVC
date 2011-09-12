// IronJSMVC - https://github.com/crpietschmann/IronJSMVC
// Copyright (c) 2011 Chris Pietschmann (http://pietschsoft.com)
// This work is licensed under a Creative Commons Attribution 3.0 United States License, unless explicitly stated otherwise within the posted content.
// http://creativecommons.org/licenses/by/3.0/us/

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
