using System.Data;
using DataTool.FormatAccess.ExcelAccess.ExcelWrite.ExcelSheetContentWrite.ContentAdvancedSingleWrite;
using DataToolInterface.Format.File.Excel;

namespace DataTool.FormatAccess.ExcelAccess.ExcelWrite.ExcelSheetContentWrite
{
    public class ExcelSheetContentWriter
    {
        private readonly object _object;
        private readonly DataTable _dataTable;
        private readonly ExcelSheetAttribute _excelSheetAttribute;
        public ExcelSheetContentWriter(object paramObject, DataTable paramDataTable,ExcelSheetAttribute paramExcelSheetAttribute)
        {
            _object = paramObject;
            _dataTable = paramDataTable;
            _excelSheetAttribute = paramExcelSheetAttribute;
            
            DoWork();
        }

        private void DoWork()
        {
            var attribute = new SheetContentAdvancedSingleAttribute()
            {
                row = "1",
                column = "1",
                type = "Single",
                modelRowCount = "Auto",
                modelColumnCount = "Auto",
                key = _excelSheetAttribute.key,
                keyPattern = _excelSheetAttribute.keyPattern,
                describe = _excelSheetAttribute.describe,
            };
            
            var contentAdvancedSingleWriter = new ContentAdvancedSingleWriter(0,0,_object,_dataTable,attribute);
        }
    }
}