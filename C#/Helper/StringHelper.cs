/*
Helper for handling strings
*/
using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace Hitachi.Core.Helpers
{
    public static class StringHelper
    {
        private static readonly string[] Units = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        public static string GetReadableFileSize(long size) 
        {
            var unitIndex = 0;
            while (size >= 1024)
            {
                size /= 1024;
                ++unitIndex;
            }

            var unit = Units[unitIndex];
            return string.Format("{0:0.#} {1}", size, unit);
        }
		
        public static string GetControllerName<T>() where T : Controller
        {
            const string suffix = "Controller";
            var controllerName = typeof(T).Name;
            return controllerName.Substring(0, controllerName.Length - suffix.Length);
        }

        public static string GetDateStartToEnd(DateTime start, DateTime end)
        {
            if (start.Date == end.Date)
            {
                return $"{start.ToString("D")}";
            }
            else
            {
                return $"{start.ToString("D")} - {end.ToString("D")}";
            }
        }

        public static string GetDateTimeStartToEnd(DateTime start, DateTime end, string timeZoneDescription = "")
        {
            if (start.Date == end.Date)
            {
                if (start.TimeOfDay == end.TimeOfDay)
                {
                    return $"{start.ToString("D")}";
                }
                else
                {
                    return $"{start.ToString("f")} - {end.ToString("t")}" + (!string.IsNullOrWhiteSpace(timeZoneDescription) ? $" ({timeZoneDescription})" : "");
                }
            }
            else
            {
                return $"{start.ToString("f")} - {end.ToString("f")}" + (!string.IsNullOrWhiteSpace(timeZoneDescription) ? $" ({timeZoneDescription})" : "");
            }
        }

        public static string GetKebabCase(string value)
        {
            return value.ToLower().Replace(" ", "-");
        }

        public static string GetCamelCase(string value)
        {
            return value.Substring(0, 1).ToLower() + value.Substring(1);
        }

        public static string GetSentenceCase(string value)
        {
            return value.Substring(0, 1).ToUpper() + value.Substring(1);
        }

        public static bool HasSpecialChars(string yourString)
        {
            return yourString.Any(ch => ch != ' ' && !char.IsLetterOrDigit(ch));
        }

    }
}
