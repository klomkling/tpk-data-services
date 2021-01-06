using System;
using System.Collections.Specialized;
using System.Web;
using Microsoft.AspNetCore.Components;

namespace Tpk.DataServices.Shared.Classes
{
    public static class NavigationManagerExtensions
    {
        // get entire querystring name/value collection
        private static NameValueCollection QueryString(this NavigationManager navigationManager)
        {
            return HttpUtility.ParseQueryString(new Uri(navigationManager.Uri).Query);
        }

        // get single querystring value with specified key
        private static string QueryString(this NavigationManager navigationManager, string key)
        {
            return navigationManager.QueryString()[key];
        }

        public static bool TryGetQueryString<T>(this NavigationManager navManager, string key, out T value)
        {
            var valueString = navManager.QueryString(key);
            if (string.IsNullOrEmpty(valueString))
            {
                value = default;
                return false;
            }

            if (typeof(T) == typeof(int) && int.TryParse(valueString, out var valueAsInt))
            {
                value = (T) (object) valueAsInt;
                return true;
            }

            if (typeof(T) == typeof(string))
            {
                value = (T) (object) valueString;
                return true;
            }

            if (typeof(T) == typeof(decimal) && decimal.TryParse(valueString, out var valueAsDecimal))
            {
                value = (T) (object) valueAsDecimal;
                return true;
            }

            value = default;
            return false;
        }
    }
}