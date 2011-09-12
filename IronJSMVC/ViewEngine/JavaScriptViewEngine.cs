// IronJSMVC - https://github.com/crpietschmann/IronJSMVC
// Copyright (c) 2011 Chris Pietschmann (http://pietschsoft.com)
// This work is licensed under a Creative Commons Attribution 3.0 United States License, unless explicitly stated otherwise within the posted content.
// http://creativecommons.org/licenses/by/3.0/us/

using System;
using System.IO;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

// Credited to: http://haacked.com/archive/2009/04/22/scripted-db-views.aspx

namespace IronJSMVC.ViewEngine
{
    public class JavaScriptViewEngine : VirtualPathProviderViewEngine
    {
        private void SetupLocationFormats()
        {
            this.PartialViewLocationFormats = new string[] {
                "~/Views/{1}/_{0}.jshtml",
                "~/Views/Shared/_{0}.jshtml"
            };
            this.ViewLocationFormats = new string[] {
                "~/Views/{1}/{0}.jshtml",
                "~/Views/Shared/{0}.jshtml"
            };
            this.MasterLocationFormats = new string[]{
                "~/Views/{1}/{0}.jshtml",
                "~/Views/Shared/{0}.jshtml"
            };
        }

        public JavaScriptViewEngine(IJavaScriptViewSource jsViewSource) //CSharp.Context context, IJavaScriptViewSource jsViewSource)
        {
            this.SetupLocationFormats();

            //this.Context = context;
            this.JavaScriptViewSource = jsViewSource;
        }

        public JavaScriptViewEngine()
            :this(null) //new CSharp.Context(), null)
        { }

        protected IJavaScriptViewSource JavaScriptViewSource { get; private set; }
        //protected CSharp.Context Context { get; private set; }

        string GetContents(ControllerContext controllerContext, string path)
        {
            if (this.JavaScriptViewSource != null)
            {
                var contents = this.JavaScriptViewSource.GetViewContents(controllerContext, path);
                if (!String.IsNullOrEmpty(contents))
                {
                    return contents;
                }
            }

            using (var stream = VirtualPathProvider.GetFile(path).Open())
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            string[] strArray;
            string[] strArray2;
            if (controllerContext == null)
            {
                throw new ArgumentNullException("controllerContext");
            }
            if (string.IsNullOrEmpty(viewName))
            {
                throw new ArgumentException("MvcResources.Common_NullOrEmpty", "viewName");
            }
            string requiredString = controllerContext.RouteData.Values["controllerName"] as string ?? controllerContext.RouteData.GetRequiredString("controller");
            string str2 = this.GetPath(controllerContext, this.ViewLocationFormats, this.AreaViewLocationFormats, "ViewLocationFormats", viewName, requiredString, "View", useCache, out strArray);
            string str3 = this.GetPath(controllerContext, this.MasterLocationFormats, this.AreaMasterLocationFormats, "MasterLocationFormats", masterName, requiredString, "Master", useCache, out strArray2);
            if (!string.IsNullOrEmpty(str2) && (!string.IsNullOrEmpty(str3) || string.IsNullOrEmpty(masterName)))
            {
                return new ViewEngineResult(this.CreateView(controllerContext, str2, str3), this);
            }

            
            return base.FindView(controllerContext, viewName, masterName, useCache);
        }

        private static readonly string[] _emptyLocations = new string[0];
        private string GetPath(ControllerContext controllerContext, string[] locations, string[] areaLocations, string locationsPropertyName, string name, string controllerName, string cacheKeyPrefix, bool useCache, out string[] searchedLocations)
        {
            searchedLocations = _emptyLocations;
            if (string.IsNullOrEmpty(name))
            {
                return string.Empty;
            }
            string areaName = null;// AreaHelpers.GetAreaName(controllerContext.RouteData);
            bool flag = false; // !string.IsNullOrEmpty(areaName);
            List<ViewLocation> viewLocations = GetViewLocations(locations, flag ? areaLocations : null);
            if (viewLocations.Count == 0)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "MvcResources.Common_PropertyCannotBeNullOrEmpty", new object[] { locationsPropertyName }));
            }
            bool flag2 = IsSpecificPath(name);
            string key = this.CreateCacheKey(cacheKeyPrefix, name, flag2 ? string.Empty : controllerName, areaName);
            if (useCache)
            {
                return this.ViewLocationCache.GetViewLocation(controllerContext.HttpContext, key);
            }
            if (!flag2)
            {
                return this.GetPathFromGeneralName(controllerContext, viewLocations, name, controllerName, areaName, key, ref searchedLocations);
            }
            return this.GetPathFromSpecificName(controllerContext, name, key, ref searchedLocations);
        }

        private string CreateCacheKey(string prefix, string name, string controllerName, string areaName)
        {
            return string.Format(CultureInfo.InvariantCulture, ":ViewCacheEntry:{0}:{1}:{2}:{3}:{4}:", new object[] { base.GetType().AssemblyQualifiedName, prefix, name, controllerName, areaName });
        }

        private string GetPathFromSpecificName(ControllerContext controllerContext, string name, string cacheKey, ref string[] searchedLocations)
        {
            string virtualPath = name;
            if (!this.FilePathIsSupported(name) || !this.FileExists(controllerContext, name))
            {
                virtualPath = string.Empty;
                searchedLocations = new string[] { name };
            }
            this.ViewLocationCache.InsertViewLocation(controllerContext.HttpContext, cacheKey, virtualPath);
            return virtualPath;
        }

        private static bool IsSpecificPath(string name)
        {
            char ch = name[0];
            if (ch != '~')
            {
                return (ch == '/');
            }
            return true;
        }

        private bool FilePathIsSupported(string virtualPath)
        {
            if (this.FileExtensions == null)
            {
                return true;
            }
            
            string str = VirtualPathUtility.GetExtension(virtualPath).TrimStart(new char[] { '.' });
            return this.FileExtensions.Contains(str, StringComparer.OrdinalIgnoreCase);
        }

        private string GetPathFromGeneralName(ControllerContext controllerContext, List<ViewLocation> locations, string name, string controllerName, string areaName, string cacheKey, ref string[] searchedLocations)
        {
            string virtualPath = string.Empty;
            searchedLocations = new string[locations.Count];
            for (int i = 0; i < locations.Count; i++)
            {
                string str2 = locations[i].Format(name, controllerName, areaName);
                if (this.FileExists(controllerContext, str2))
                {
                    searchedLocations = _emptyLocations;
                    virtualPath = str2;
                    this.ViewLocationCache.InsertViewLocation(controllerContext.HttpContext, cacheKey, virtualPath);
                    return virtualPath;
                }
                searchedLocations[i] = str2;
            }
            return virtualPath;
        }

        private static List<ViewLocation> GetViewLocations(string[] viewLocationFormats, string[] areaViewLocationFormats)
        {
            List<ViewLocation> list = new List<ViewLocation>();
            if (areaViewLocationFormats != null)
            {
                foreach (string str in areaViewLocationFormats)
                {
                    list.Add(new AreaAwareViewLocation(str));
                }
            }
            if (viewLocationFormats != null)
            {
                foreach (string str2 in viewLocationFormats)
                {
                    list.Add(new ViewLocation(str2));
                }
            }
            return list;
        }

        private class ViewLocation
        {
            // Fields
            protected string _virtualPathFormatString;

            // Methods
            public ViewLocation(string virtualPathFormatString)
            {
                this._virtualPathFormatString = virtualPathFormatString;
            }

            public virtual string Format(string viewName, string controllerName, string areaName)
            {
                return string.Format(CultureInfo.InvariantCulture, this._virtualPathFormatString, new object[] { viewName, controllerName });
            }
        }

        private class AreaAwareViewLocation : JavaScriptViewEngine.ViewLocation
        {
            // Methods
            public AreaAwareViewLocation(string virtualPathFormatString)
                : base(virtualPathFormatString)
            {
            }

            public override string Format(string viewName, string controllerName, string areaName)
            {
                return string.Format(CultureInfo.InvariantCulture, base._virtualPathFormatString, new object[] { viewName, controllerName, areaName });
            }
        }

 





 




        protected override bool FileExists(ControllerContext controllerContext, string virtualPath)
        {
            if (this.JavaScriptViewSource != null && this.JavaScriptViewSource.FileExists(controllerContext, virtualPath))
            {
                return true;
            }
            return base.FileExists(controllerContext, virtualPath);
        }

        JavaScriptView GetJavaScriptMasterView(ControllerContext controllerContext, string virtualPath)
        {
            if (String.IsNullOrEmpty(virtualPath))
            {
                return null;
            }

            var viewContents = this.GetContents(controllerContext, virtualPath);
            return new JavaScriptView(viewContents, null); //this.Context, viewContents, null);
        }

        IView GetView(ControllerContext controllerContext, string virtualPath, JavaScriptView masterView)
        {
            if (String.IsNullOrEmpty(virtualPath))
            {
                return null;
            }

            var viewContents = this.GetContents(controllerContext, virtualPath);

            return new JavaScriptView(viewContents, masterView); //this.Context, viewContents, masterView);
        }

        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            return this.GetView(controllerContext, partialPath, null);
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            var masterView = this.GetJavaScriptMasterView(controllerContext, masterPath);
            return this.GetView(controllerContext, viewPath, masterView);
        }
    }
}
