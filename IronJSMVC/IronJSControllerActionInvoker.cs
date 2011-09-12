using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using IronJS;
using IronJS.Hosting;
using IronJS.Native;

namespace IronJSMVC
{
    public class IronJSControllerActionInvoker : IActionInvoker
    {
        public bool InvokeAction(ControllerContext controllerContext, string actionName)
        {
            string controllerName = controllerContext.RouteData.Values["controllerName"] as string;

            var actionResult = this.InvokeAction(controllerContext, controllerName, actionName);

            if (actionResult == null)
            {
                return false;
            }

            actionResult.ExecuteResult(controllerContext);

            return true;
        }

        protected static void InvokeActionResult(ControllerContext controllerContext, ActionResult actionResult)
        {
            actionResult.ExecuteResult(controllerContext);
        }

        protected ActionResult InvokeAction(ControllerContext controllerContext, string controllerName, string actionName)
        {
            var scriptPath = controllerContext.HttpContext.Server.MapPath(string.Format("~/Controllers/{0}.js", controllerName));

            var context = IronJSControllerActionInvoker.initializeIronJSContext(controllerContext);
            CommonObject jscontroller = null;

            try
            {
                jscontroller = context.ExecuteFile(scriptPath) as CommonObject;
                RegisterControllerMembers(context, controllerContext, jscontroller);
            }
            catch (FileNotFoundException ex)
            {
                throw new Exception(string.Format("IronJSMVC: Controller '{0}' not found.", controllerName), ex);
            }

            if (!jscontroller.Members.Keys.Contains(actionName))
            {
                throw new Exception(string.Format("IronJSMVC: Action '{1}' does not exist within Controller '{0}'", controllerName, actionName));
            }

            BoxedValue rawModel;
            var actionFunc = jscontroller.Members[actionName] as FunctionObject;
            var id = controllerContext.RouteData.Values["id"] as string;
            if (id == null)
            {
                rawModel = actionFunc.Call(jscontroller);
            }
            else
            {
                rawModel = actionFunc.Call<string>(jscontroller, id);
            }
            
            dynamic model = rawModel;
            if (model is BoxedValue)
            {
                model = model.ClrBoxed;
            }
            if (model is CommonObject)
            {
                model = new DynamicCommonObject(model);
            }

          

            ViewResult viewResult = null;
            if (model is ViewResult)
            {
                // the action method returned an ActionResult, so just use that
                viewResult = (ViewResult)model;
            }
            else
            {
                if (model != null)
                {
                    controllerContext.Controller.ViewData.Model = model;
                }
                viewResult = this.JSView(controllerContext, actionName, null, model);
            }

            if (string.IsNullOrWhiteSpace(viewResult.MasterName))
            {
                // Get the Controller's Default Master Layout
                if (jscontroller.Members.ContainsKey("_Layout"))
                {
                    viewResult.MasterName = jscontroller.Members["_Layout"] as String;
                }
            }

            return viewResult;
        }

        public static IronJS.Hosting.CSharp.Context initializeIronJSContext(ControllerContext controllerContext)
        {
            var context = new IronJS.Hosting.CSharp.Context();

            IronJSGlobalMethods.RegisterGlobals(context, controllerContext.HttpContext);

            return context;
        }

        public void RegisterControllerMembers(CSharp.Context context, ControllerContext controllerContext, CommonObject jscontroller)
        {
            jscontroller.Prototype.Put("View", Utils.CreateFunction<Func<CommonObject, ActionResult>>(context.Environment, 1, (obj) => this.JSView(controllerContext, obj)));
        }

        #region "IronJS Interop Methods"

        private ActionResult JSView(ControllerContext controllerContext, CommonObject obj)
        {
            if (obj is ArrayObject)
            {
                var arr = (ArrayObject)obj;
                dynamic model = null;
                string viewName = null;
                string masterName = null;

                if (arr.Length == 1)
                {
                    model = dynamicizeCommonObject(arr.Get(0));
                }
                else if (arr.Length == 2)
                {
                    model = dynamicizeCommonObject(arr.Get(1));
                    viewName = dynamicizeCommonObject(arr.Get(0));
                }
                else if (arr.Length == 3)
                {
                    model = dynamicizeCommonObject(arr.Get(2));
                    viewName = dynamicizeCommonObject(arr.Get(0));
                    masterName = dynamicizeCommonObject(arr.Get(1));
                }
                else
                {
                    throw new ArgumentOutOfRangeException(string.Format("Expecting ArrayObject of 1 or 2 in length. {0} length found.", arr.Length), "obj");
                }

                return JSView(controllerContext, viewName, masterName, model);
            }
            else // CommonObject
            {
                dynamic model = dynamicizeCommonObject(obj);
                return JSView(controllerContext, null, null, model);
            }
            throw new ArgumentException("Parameter mismatch. ArrayObject or CommonObject expected.", "obj");
        }

        private ActionResult JSView(ControllerContext controllerContext, string viewNameToUse, string masterNameToUse, dynamic model)
        {
            //var view = string.Format(
            //    "{0}/{1}",
            //    controllerContext.RouteData.Values["controllerName"],
            //    (string.IsNullOrWhiteSpace(viewNameToUse)) ? controllerContext.RouteData.Values["actionName"] : viewNameToUse
            //    );

            //var master = string.Format(
            //    "{0}/{1}",
            //    controllerContext.RouteData.Values["controllerName"],
            //    (string.IsNullOrWhiteSpace(masterNameToUse)) ? "layout" : masterNameToUse
            //    );

            //return View(view, master, model);

            if (model != null)
            {
                controllerContext.Controller.ViewData.Model = model;
            }

            return new ViewResult
            {
                ViewName = viewNameToUse,
                MasterName = masterNameToUse,
                ViewData = controllerContext.Controller.ViewData,
                TempData = controllerContext.Controller.TempData
            };
        }

        #endregion

        protected static dynamic dynamicizeCommonObject(dynamic obj)
        {
            dynamic model = obj;
            if (model is StringObject)
            {
                model = model.Value.Value;
            }
            if (model is BoxedValue)
            {
                model = model.ClrBoxed;
            }
            if (model is CommonObject)
            {
                model = new DynamicCommonObject(model);
            }
            return model;
        }
    }
}
