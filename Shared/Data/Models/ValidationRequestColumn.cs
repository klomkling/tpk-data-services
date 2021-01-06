using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class ValidationRequestColumn
    {
        public ValidationRequestColumn()
        {
        }
        
        public ValidationRequestColumn(string columnName, string columnTypeName, object columnValue)
        {
            ColumnName = columnName;
            ColumnTypeName = columnTypeName;
            ColumnValue = columnValue;
        }

        public string ColumnName { get; set; }
        public string ColumnTypeName { get; set; }
        public object ColumnValue { get; set; }
    }
}