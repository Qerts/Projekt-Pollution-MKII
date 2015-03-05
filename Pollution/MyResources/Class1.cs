using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.ApplicationModel.Resources;

namespace MyResources
{
    public static class LocalizedNamesLibrary
    {
        static ResourceLoader resourceLoader = null;

        public static string LibraryName
        {
            get
            {
                String name;
                GetLibraryName("string1", out name);
                return name;
            }
        }

        private static void GetLibraryName(string resourceName, out string resourceValue)
        {
            if (resourceLoader == null)
            {
                resourceLoader = ResourceLoader.GetForCurrentView("MyResources/Resources");
            }
            resourceValue = resourceLoader.GetString(resourceName);
        }

    }
}