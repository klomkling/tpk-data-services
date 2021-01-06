using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Classes
{
    public static class ValidationExtensions
    {
        public static ValidationRequestColumn CreateValidationColumn<T>(this T model, string columnName,
            object columnValue)
        {
            return new ValidationRequestColumn
            {
                ColumnName = columnName,
                ColumnTypeName = model.GetType().GetProperty(columnName)?.PropertyType.Name,
                ColumnValue = columnValue
            };
        }
    }
}