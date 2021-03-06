﻿// IronJSMVC - https://github.com/crpietschmann/IronJSMVC
// Copyright (c) 2011 Chris Pietschmann (http://pietschsoft.com)
// This work is licensed under a Creative Commons Attribution 3.0 United States License, unless explicitly stated otherwise within the posted content.
// http://creativecommons.org/licenses/by/3.0/us/

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
