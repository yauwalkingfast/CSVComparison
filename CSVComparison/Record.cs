using System;
using System.Collections.Generic;
using System.Text;

namespace ExcelPOC
{
    class Record
    {
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string ColumnNumber { get; set; }
        public string DataType { get; set; }
        public string Precision { get; set; }

        public Record(string tableName, string columnName, string columnNumber, string dataType, string precision)
        {
            TableName = tableName;
            ColumnName = columnName;
            ColumnNumber = columnNumber;
            DataType = dataType;
            Precision = precision;
        }

        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Record p = (Record)obj;
                return TableName == p.TableName && ColumnName == p.ColumnName && ColumnNumber == p.ColumnNumber && DataType == p.DataType && Precision == p.Precision;
            }
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(TableName, ColumnName, ColumnNumber, DataType, Precision);
        }

        public override string ToString()
        {
            return $"TableName: {TableName}, ColumnName: {ColumnName}, ColumnNumber: {ColumnNumber}, DataType: {DataType}, Precision: {Precision}\n";
        }
    }
}
