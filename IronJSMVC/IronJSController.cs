// Copyright (c) 2011 Chris Pietschmann - http://pietschsoft.com
// All rights reserved

using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IronJS;
using IronJS.Native;
using System.Text;
using System.Globalization;
using System.Dynamic;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IronJS.Hosting;

namespace IronJSMVC
{
    public class IronJSMVCController : Controller
    {
        protected override IActionInvoker CreateActionInvoker()
        {
            //return base.CreateActionInvoker();
            return new IronJSControllerActionInvoker();
        }

        //public ActionResult Main(string controllerName, string actionName, string id)
        //{
        //    var scriptPath = Server.MapPath(string.Format("~/Controllers/{0}.js", controllerName));

        //    var context = IronJSMVCController.initializeIronJSContext(this);
        //    CommonObject jscontroller = null;

        //    try
        //    {
        //        jscontroller = context.ExecuteFile(scriptPath) as CommonObject;
        //        wireUpControllerMethods(context, this, jscontroller);
        //    }
        //    catch (FileNotFoundException ex)
        //    {
        //        throw new Exception(string.Format("IronJSMVC: Controller '{0}' not found.", controllerName), ex);
        //    }

        //    if (!jscontroller.Members.Keys.Contains(actionName))
        //    {
        //        throw new Exception(string.Format("IronJSMVC: Action '{1}' does not exist within Controller '{0}'", controllerName, actionName));
        //    }

        //    BoxedValue rawModel;
        //    var actionFunc = jscontroller.Members[actionName] as FunctionObject;
        //    if (id == null)
        //    {
        //        rawModel = actionFunc.Call(jscontroller);
        //    }
        //    else
        //    {
        //        rawModel = actionFunc.Call<string>(jscontroller, id);
        //    }
            
        //    dynamic model = rawModel;
        //    if (model is BoxedValue)
        //    {
        //        model = model.ClrBoxed;
        //    }
        //    if (model is CommonObject)
        //    {
        //        model = new DynamicCommonObject(model);
        //    }

        //    if (model is ViewResult)
        //    {
        //        // the action method returned an ActionResult, so just return that
        //        return model;
        //    }

        //    var viewName = string.Format("{0}/{1}", controllerName, actionName);
        //    return View(viewName, model);
        //}

        //public static IronJS.Hosting.CSharp.Context initializeIronJSContext(Controller controller)
        //{
        //    var context = new IronJS.Hosting.CSharp.Context();

        //    context.Globals.Put("include", Utils.CreateFunction<Action<string>>(context.Environment, 1, s => { GlobalMethods.Include(context, s); }));

        //    return context;
        //}

        //public static void wireUpControllerMethods(IronJS.Hosting.CSharp.Context context, IronJSMVCController controller, CommonObject jscontroller)
        //{
        //    jscontroller.Prototype.Put("View", Utils.CreateFunction<Func<CommonObject, ActionResult>>(context.Environment, 1, controller.JSView));
        //}

        //#region "IronJS Interop Methods"

        //private ActionResult JSView(CommonObject obj)
        //{
        //    if (obj is ArrayObject)
        //    {
        //        var arr = (ArrayObject)obj;
        //        dynamic model = null;
        //        string viewName = null;
        //        string masterName = null;

        //        if (arr.Length == 1)
        //        {
        //            model = dynamicizeCommonObject(arr.Get(0));
        //        }
        //        else if(arr.Length == 2)
        //        {
        //            model = dynamicizeCommonObject(arr.Get(1));
        //            viewName = dynamicizeCommonObject(arr.Get(0));
        //        }
        //        else if (arr.Length == 3)
        //        {
        //            model = dynamicizeCommonObject(arr.Get(2));
        //            viewName = dynamicizeCommonObject(arr.Get(0));
        //            masterName = dynamicizeCommonObject(arr.Get(1));
        //        }
        //        else
        //        {
        //            throw new ArgumentOutOfRangeException(string.Format("Expecting ArrayObject of 1 or 2 in length. {0} length found.", arr.Length), "obj");
        //        }
                
        //        return JSView(viewName, masterName, model);
        //    }
        //    else // CommonObject
        //    {
        //        dynamic model = dynamicizeCommonObject(obj);
        //        return JSView(null, null, model);
        //    }
        //    throw new ArgumentException("Parameter mismatch. ArrayObject or CommonObject expected.", "obj");
        //}

        //private ActionResult JSView(string viewNameToUse, string masterNameToUse, dynamic model)
        //{
        //    var view = string.Format(
        //        "{0}/{1}",
        //        this.RouteData.Values["controllerName"],
        //        (string.IsNullOrWhiteSpace(viewNameToUse)) ? this.RouteData.Values["actionName"] : viewNameToUse
        //        );

        //    var master = string.Format(
        //        "{0}/{1}",
        //        this.RouteData.Values["controllerName"],
        //        (string.IsNullOrWhiteSpace(masterNameToUse)) ? "layout" : masterNameToUse
        //        );

        //    return View(view, master, model);
        //}

        //#endregion

        //private static dynamic dynamicizeCommonObject(dynamic obj)
        //{
        //    dynamic model = obj;
        //    if (model is StringObject)
        //    {
        //        model = model.Value.Value;
        //    }
        //    if (model is BoxedValue)
        //    {
        //        model = model.ClrBoxed;
        //    }
        //    if (model is CommonObject)
        //    {
        //        model = new DynamicCommonObject(model);
        //    }
        //    return model;
        //}
    }
}
