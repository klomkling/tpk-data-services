using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Tpk.DataServices.Shared.Interfaces;

namespace Tpk.DataServices.Shared.Classes
{
    public static class DataEditContextExtensions
    {
        public static void UpdateDataItem<T>(this IEditContextBase<T> model)
        {
            var properties = model.GetType().GetProperties()
                .Where(p => p.Name.Equals("DataItem", StringComparison.InvariantCultureIgnoreCase) == false)
                .Where(p => p.CanWrite);

            foreach (var propertyInfo in properties)
            {
                // Check if property have NotMapped Attribute, then not include into parameters
                if (propertyInfo.GetCustomAttributes(true)
                    .FirstOrDefault(a => a.GetType() == typeof(NotMappedAttribute)) != null) continue;

                var prop = model.DataItem.GetType().GetProperty(propertyInfo.Name);
                if (null == prop || prop.CanWrite == false) continue;

                var value = propertyInfo.GetValue(model);
                prop.SetValue(model.DataItem, value);
            }
        }

        public static T EditContext<T>(this T model, bool isEdit = false)
        {
            var properties = model.GetType().GetProperties()
                .Where(m => m.Name.StartsWith("Edit", StringComparison.InvariantCultureIgnoreCase));

            foreach (var property in properties)
            {
                var name = property.Name.Replace("Edit", string.Empty);
                var sourceProp = model.GetType().GetProperty(name);
                if (sourceProp != null) property.SetValue(model, sourceProp.GetValue(model));
            }

            return model;
        }

        public static void ApplyChanges<T>(this T model)
        {
            var properties = model.GetType().GetProperties()
                .Where(m => m.Name.StartsWith("Edit", StringComparison.InvariantCultureIgnoreCase));

            foreach (var property in properties)
            {
                var name = property.Name.Replace("Edit", string.Empty);
                var targetProp = model.GetType().GetProperty(name);
                if (targetProp == null) continue;

                targetProp.SetValue(model, property.GetValue(model));
            }
        }
    }
}