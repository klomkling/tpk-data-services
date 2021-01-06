using System;
using System.Collections.Generic;
using System.Linq;

namespace Tpk.DataServices.Shared.Classes
{
    public static class SearchExtensions
    {
        public static string SearchKeyName(this Type type)
        {
            var model = Activator.CreateInstance(type, null);
            return model.SearchKeyName();
        }

        public static string SearchKeyName<T>(this T model)
        {
            var m = Equals(model, default) ? (T) Activator.CreateInstance(typeof(T), null) : model;
            return $"Search_{m?.GetType().Name}";
        }

        public static string ToSearchString<T>(this T model)
        {
            if (Equals(model, default)) return null;

            var result = string.Empty;
            var properties = model.GetType().GetProperties()
                .Where(p => p.Name.StartsWith("Search", StringComparison.InvariantCultureIgnoreCase));

            return (from propertyInfo in properties
                let value = propertyInfo.GetValue(model)
                where Equals(value, default) == false
                select $"{propertyInfo.PropertyType.Name}_{propertyInfo.Name}_{value}"
                into condition
                where condition.EndsWith("_") == false
                select condition).Aggregate(result,
                (current, condition) => string.IsNullOrEmpty(current) == false ? $"{current}__{condition}" : condition);
        }

        public static void FromSearchString<T>(this T model, string searchString)
        {
            if (string.IsNullOrEmpty(searchString)) return;

            var searchFields = searchString.Split("__");
            foreach (var field in searchFields)
            {
                var details = field.Split("_");

                var prop = model.GetType().GetProperty(details[1]);
                if (prop == null) continue;

                var value = PropertyInfoExtensions.CreateValue(details[0], details[2], true);
                if (value == null) continue;

                prop.SetValue(model, value);
            }
        }

        public static void ClearSearchModel<T>(this T model)
        {
            if (Equals(model, default)) return;

            var properties = model.GetType().GetProperties()
                .Where(p => p.Name.StartsWith("Search", StringComparison.InvariantCultureIgnoreCase));

            foreach (var property in properties) property.SetValue(model, null);
        }

        public static string ToSearchParameters(this string source, Type type)
        {
            if (string.IsNullOrEmpty(source)) return null;

            // Search parameter pattern :
            // Type_Field_Filter__Type_Field_Filter__Type_Field_Filter

            var conditions = new List<string>();
            var columns = source.Split("__");
            foreach (var column in columns)
            {
                var detail = column.Split("_");
                if (detail.Length != 3) continue;

                try
                {
                    var prop = type.GetProperty(detail[1]);
                    if (prop == null) continue;

                    var condition = BuildCondition(detail[0], detail[1].Replace("Search", string.Empty), detail[2]);
                    if (string.IsNullOrEmpty(condition)) continue;

                    conditions.Add(condition);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            return conditions.Count > 0 ? string.Join(" AND ", conditions) : null;
        }

        private static string BuildCondition(string type, string field, string value)
        {
            string condition;

            switch (type.ToLower())
            {
                case "boolean":
                    condition =
                        $"{field} = {(value.Equals("true", StringComparison.InvariantCultureIgnoreCase) ? 1 : 0)}";
                    break;

                case "string":
                    condition = $"{field} LIKE '%{value}%'";
                    break;

                case "datetime":
                    if (value.Contains(">="))
                        condition = $"{field} >= '{value.Replace(">=", string.Empty).Trim()}'";
                    else if (value.Contains("<="))
                        condition = $"{field} <= '{value.Replace("<=", string.Empty).Trim()}'";
                    else if (value.Contains(">"))
                        condition = $"{field} > '{value.Replace(">", string.Empty).Trim()}'";
                    else if (value.Contains("<"))
                        condition = $"{field} < '{value.Replace("<", string.Empty)}'";
                    else if (value.Contains("="))
                        condition = $"{field} = '{value.Replace("=", string.Empty).Trim()}'";
                    else
                        condition = $"{field} = '{value}'";

                    break;

                case "int32":
                case "decimal":
                    if (value.Contains(">="))
                        condition = $"{field}{value.Replace(">=", " >= ")}";
                    else if (value.Contains("<="))
                        condition = $"{field}{value.Replace("<=", " <= ")}";
                    else if (value.Contains(">"))
                        condition = $"{field}{value.Replace(">", " > ")}";
                    else if (value.Contains("<"))
                        condition = $"{field}{value.Replace("<", " < ")}";
                    else if (value.Contains("="))
                        condition = $"{field}{value.Replace("=", " = ")}";
                    else
                        condition = $"{field} = {value}";

                    break;

                default:
                    condition = string.Empty;
                    break;
            }

            return condition;
        }
    }
}