using System;

namespace DataToolInterface.Format.File.Excel
{
    public class ExcelFileAttribute:FileFormatAttribute
    {
        public string template { get; set; }
    }
    
    public enum ExcelSubFormatEnum
    {
        Excel,
        Sheet,
        Content,
        Free//可以行列取值
    }
    public enum ExcelFieldNameEnum
    {
        sheets,
        content,
        sheetInfo,
    }

    public class ExcelDefaultAttribute:Attribute
    {
        
    }
    
    public class ExcelSheetAttribute:Attribute
    {
        public string type { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public string keyPattern { get; set; }
        public string describe { get; set; }
    }

    public class ExcelSheetDefaultAttribute : Attribute
    {
        public string type { get; set; }
        public string name { get; set; }
        public string rowCount { get; set; }
        public string columnCount { get; set; }
        public string describe { get; set; }
    }
    public class SheetContentBasicSingleAttribute:Attribute
    {
        public string row { get; set; }
        public string column { get; set; }
        public string type { get; set; }
        public string format { get; set; }
        public string mode { get; set; }
        public string pattern { get; set; }
        public string filter { get; set; }
        public string range { get; set; }
        public string allowNull { get; set; }
        public string nullValue { get; set; }
        public string describe { get; set; }
    }
    public class SheetContentBasicMultipleAttribute:Attribute
    {
        public string row { get; set; }
        public string column { get; set; }
        public string type { get; set; }
        public string format { get; set; }
        public string mode { get; set; }
        public string pattern { get; set; }
        public string filter { get; set; }
        public string spliter { get; set; }
        public string range { get; set; }
        public string allowNull { get; set; }
        public string nullValue { get; set; }
        public string describe { get; set; }
    }
    public class SheetContentAdvancedSingleAttribute:Attribute
    {
        public string row { get; set; }
        public string column { get; set; }
        public string type { get; set; }
        public string modelRowCount { get; set; }
        public string modelColumnCount { get; set; }
        public string key { get; set; }
        public string keyPattern { get; set; }
        public string describe { get; set; }
    }
    public class SheetContentAdvancedMultipleAttribute:Attribute
    {
        public string row { get; set; }
        public string column { get; set; }
        public string type { get; set; }
        public string totalRowCount { get; set; }
        public string modelRowCount { get; set; }
        public string totalColumnCount { get; set; }
        public string modelColumnCount { get; set; }
        public string key { get; set; }
        public string keyPattern { get; set; }
        public string describe { get; set; }
    }

    public enum BasicDataFormatEnum
    {
        x10,
        x16,
    }
    public enum RowColumnModeEnum
    {
        Non,
        Row,
        Column,
        RowColumn,
    }
    
    public enum ConfigRowColumnValueEnum
    {
        Auto,
    }
    
    public enum ConfigModeValueEnum
    {
        Non,
        EmptyFillPre,
    }
}