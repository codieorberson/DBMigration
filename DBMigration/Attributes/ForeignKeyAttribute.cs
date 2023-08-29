using System;

namespace SSOModels.Attributes
{
    public class ForeignKeyAttribute : Attribute
    {
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public ForeignKeyAttribute(string tableName, string columnName)
        {
            TableName = tableName;
            ColumnName = columnName;
        }
    }
}
