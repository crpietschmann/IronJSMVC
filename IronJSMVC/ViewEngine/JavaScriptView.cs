// IronJSMVC - https://github.com/crpietschmann/IronJSMVC
// Copyright (c) 2011 Chris Pietschmann (http://pietschsoft.com)
// This work is licensed under a Creative Commons Attribution 3.0 United States License, unless explicitly stated otherwise within the posted content.
// http://creativecommons.org/licenses/by/3.0/us/

using System;
using System.Text;
using System.Web.Mvc;
using IronJS.Hosting;
using IronJS;
using IronJS.Native;
using System.Web.Mvc.Html;
using System.Web.Routing;
using IronJSMVC.Extensions;

// Credited to: http://haacked.com/archive/2009/04/22/scripted-db-views.aspx

namespace IronJSMVC.ViewEngine
{
    public class JavaScriptView : IView
    {
        public JavaScriptView(string viewContents, JavaScriptView master) //CSharp.Context context, string viewContents, JavaScriptView master)
        {
            //this.Context = context;
            this.Contents = viewContents;
            this.MasterView = master;
        }
        
        private JavaScriptTemplate _template = null;
        public JavaScriptTemplate Template
        {
            get
            {
                if (_template == null)
                {
                    _template = new JavaScriptTemplate(this.Contents);
                }
                return this._template;
            }
        }

        //public CSharp.Context Context { get; private set; }
        public string Contents { get; private set; }
        public JavaScriptView MasterView { get; private set; }

        public void Render(ViewContext viewContext, System.IO.TextWriter writer)
        {
            var script = new StringBuilder();
            try
            {
                this.Template.RenderScript("render_page", script);

                if (this.MasterView != null)
                {
                    MasterView.Template.RenderScript("render_layout", script);
                }
                else
                {
                    script.AppendLine("function render_layout(){ render_page(); }");
                }

                


                
                var context = new CSharp.Context();
                object model = viewContext.ViewData.Model;
                if (model is DynamicCommonObject)
                {
                    model = ((DynamicCommonObject)model).CommonObject;
                }
                if (model != null)
                {
                    context.SetGlobal("model", model);
                }
                else
                {
                    script.AppendLine("var model = {};");
                }
                



                
                // Passing Response directly to IronJS doesn't work right now
                // Apparently, .NET interop needs to be built yet.
                //context.SetGlobal("response", viewContext.HttpContext.Response);
                //context.SetGlobal("server", viewContext.HttpContext.Server);
                //context.SetGlobal("viewContext", viewContext);




                context.SetGlobal("response_write", Utils.CreateFunction<Action<string>>(context.Environment, 1, (obj) =>
                {
                    writer.Write(obj);
                }));
                context.SetGlobal("server_htmlencode", Utils.CreateFunction<Func<string, string>>(context.Environment, 1, (obj) =>
                {
                    return viewContext.HttpContext.Server.HtmlEncode(obj);
                }));
                script.AppendLine(@"
var response = {
    Write: function(s){
        response_write(s);
    }
};
var server = {
    htmlEncode: function(s){
        return server_htmlencode(s);
    }
};
");

                var html = new HtmlHelper(viewContext, new ViewDataContainer(viewContext.ViewData));

                context.SetGlobal("html_actionlink1", Utils.CreateFunction<Func<string, string, MvcHtmlString>>(context.Environment, 2, (linkText, actionName) =>
                {
                    return html.ActionLink(linkText, actionName);
                }));
                context.SetGlobal("html_actionlink2", Utils.CreateFunction<Func<string, string, string, MvcHtmlString>>(context.Environment, 3, (linkText, actionName, controllerName) =>
                {
                    return html.ActionLink(linkText, actionName, new { controllerName = controllerName });
                }));
                context.SetGlobal("html_actionlink3", Utils.CreateFunction<Func<string, string, string, CommonObject, MvcHtmlString>>(context.Environment, 4, (linkText, actionName, controllerName, obj) =>
                {
                    var routeValues = obj.ToRouteValueDictionary();
                    if (!routeValues.ContainsKey("controllerName"))
                    {
                        routeValues["controllerName"] = controllerName;
                    }
                    return html.ActionLink(linkText, actionName, new { controllerName = controllerName });
                }));

                script.AppendLine(@"
var html = {
    actionLink: function(){
        if (arguments.length == 2) {
            return html_actionlink1(arguments[0], arguments[1]);
        } else if (arguments.length == 3) {
            return html_actionlink2(arguments[0], arguments[1], arguments[2]);
        } else if (arguments.length == 4) {
            return html_actionlink3(arguments[0], arguments[1], arguments[2], arguments[3]);
        }
    }
};
");


                script.AppendLine("render_layout();");

                context.Execute(script.ToString());
            }
            catch (Exception ex)
            {
                viewContext.HttpContext.Response.ClearContent();
                writer.Write(ex.ToString().Replace("\n", "<br/>"));
                writer.Write("<br/><hr/><br/>");
                writer.Write(script.Replace(" ", "&nbsp;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("\n", "<br/>"));
            }
        }
    }
}
