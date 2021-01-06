using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace Tpk.DataServices.Shared.Classes
{
    public static class PropertyInfoExtensions
    {
        public static object CreateValue(string typeName, object value, bool isNullable = false)
        {
            if (typeName.Equals("String", StringComparison.InvariantCultureIgnoreCase)) return value?.ToString();

            if (typeName.Equals("Int32", StringComparison.InvariantCultureIgnoreCase))
                return isNullable
                    ? int.TryParse(value.ToString(), out var result1) == false ? (int?) null : result1
                    : int.TryParse(value.ToString(), out var result2) == false
                        ? 0
                        : result2;

            if (typeName.Equals("Boolean", StringComparison.InvariantCultureIgnoreCase))
                return isNullable
                    ? bool.TryParse(value.ToString(), out var result1) == false ? (bool?) null : result1
                    : bool.TryParse(value.ToString(), out var result2) && result2;

            if (typeName.Equals("Decimal", StringComparison.InvariantCultureIgnoreCase))
                return isNullable
                    ? decimal.TryParse(value.ToString(), out var result1) == false ? (decimal?) null : result1
                    : decimal.TryParse(value.ToString(), out var result2) == false
                        ? 0m
                        : result2;

            if (typeName.Equals("DateTime", StringComparison.InvariantCultureIgnoreCase))
                return isNullable
                    ? DateTime.TryParse(value.ToString(), out var result1) == false ? (DateTime?) null : result1
                    : DateTime.TryParse(value.ToString(), out var result2) == false
                        ? DateTime.Now
                        : result2;

            return value;
        }

        public static object GetCurrentValue(this PropertyInfo propertyInfo, object value)
        {
            var propertyTypeName = propertyInfo.PropertyType.Name;

            return CreateValue(propertyTypeName, value);
        }

        public static void CopyPropertiesTo<T, TU>(this T source, TU dest)
        {
            var sourceProps = typeof(T).GetProperties().Where(x => x.CanRead).ToList();
            var destProps = typeof(TU).GetProperties()
                .Where(x => x.CanWrite)
                .ToList();

            try
            {
                foreach (var sourceProp in sourceProps)
                {
                    if (sourceProp.Name.Equals("DataItem", StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    // Check if property have NotMapped Attribute, then not include into parameters
                    if (sourceProp.GetCustomAttributes(true)
                        .FirstOrDefault(a => a.GetType() == typeof(NotMappedAttribute)) != null)
                        continue;

                    if (destProps.Any(x => x.Name == sourceProp.Name) == false) continue;

                    var p = destProps.First(x => x.Name == sourceProp.Name);
                    if (p.CanWrite)
                        // check if the property can be set or no.
                        p.SetValue(dest, sourceProp.GetValue(source, null), null);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public static void FromViewModel<TTarget, TSource>(this TTarget target, TSource source)
        {
            var properties = target.GetType().GetProperties();
            foreach (var property in properties)
            {
                // Check if property have NotMapped Attribute, then not include into parameters
                if (property.GetCustomAttributes(true)
                    .FirstOrDefault(a => a.GetType() == typeof(NotMappedAttribute)) != null)
                    continue;

                var sourceProp = source.GetType().GetProperty(property.Name);
                if (sourceProp == null) continue;

                property.SetValue(target, sourceProp.GetValue(source));
            }
        }

        public static T ToModel<T>(this string source, string separator = ";")
        {
            var valueCollection = source.Split(separator);

            var model = (T) Activator.CreateInstance(typeof(T));
            var properties = model?.GetType().GetProperties();

            var propertyCount = properties?.Count(p => p.GetCustomAttributes(true)
                .FirstOrDefault(a => a.GetType() == typeof(NotMappedAttribute)) == null);
            var valueCount = valueCollection.Length;

            if (propertyCount != valueCount) return model;

            var i = 0;
            foreach (var property in properties)
            {
                if (property.GetCustomAttributes(true)
                    .FirstOrDefault(a => a.GetType() == typeof(NotMappedAttribute)) != null)
                    continue;

                property.SetValue(model, CreateValue(property.PropertyType.Name, valueCollection[i]));
                i++;
            }

            return model;
        }
    }
}