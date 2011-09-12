// IronJSMVC - https://github.com/crpietschmann/IronJSMVC
// Copyright (c) 2011 Chris Pietschmann (http://pietschsoft.com)
// This work is licensed under a Creative Commons Attribution 3.0 United States License, unless explicitly stated otherwise within the posted content.
// http://creativecommons.org/licenses/by/3.0/us/

using System.Web.Mvc;

namespace IronJSMVC
{
    public class IronJSMVCController : Controller
    {
        protected override IActionInvoker CreateActionInvoker()
        {
            return new IronJSControllerActionInvoker();
        }
    }
}
