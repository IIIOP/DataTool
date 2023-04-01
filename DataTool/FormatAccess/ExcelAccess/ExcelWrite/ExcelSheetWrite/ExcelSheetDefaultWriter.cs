using System.Linq;
using System.Reflection;
using DataTool.Auxiliary;
using DataTool.FormatAccess.ExcelAccess.Auxiliary;
using DataTool.FormatAccess.ExcelAccess.ExcelWrite.ExcelSheetContentWrite;
using DataToolInterface.Format.File.Excel;
using Microsoft.Office.Interop.Excel;

namespace DataTool.FormatAccess.ExcelAccess.ExcelWrite.ExcelSheetWrite
{
    public class ExcelSheetDefaultWriter
    {
        private readonly object _object;
        private readonly Workbook _workbook;
        private readonly ExcelSheetDefaultAttribute _excelSheetDefaultAttribute;
        private readonly string _name;

        private System.Data.DataTable _dataTable;
        public ExcelSheetDefaultWriter(object paramObject,Workbook paramWorkbook,ExcelSheetDefaultAttribute paramExcelSheetDefaultAttribute)
        {
            _object = paramObject;
            _workbook = paramWorkbook;
            _excelSheetDefaultAttribute = paramExcelSheetDefaultAttribute;

            _name = _excelSheetDefaultAttribute.name.EncodeVariable(_object);

            _dataTable = paramObject as System.Data.DataTable;

            WriteSheetDefault();
        }

        private void WriteSheetDefault()
        {
            var sheetContent = new object[_dataTable.Rows.Count, _dataTable.Columns.Count];
            for (var i = 0; i < _dataTable.Rows.Count; i++)
            {
                for (var j = 0; j < _dataTable.Columns.Count; j++)
                {
                    sheetContent[i, j] = _dataTable.Rows[i][j];
                }
            }

            bool hasTemplate = false;
            Worksheet sheet;
            if (_workbook.Worksheets.Cast<Worksheet>().Any(p=>p.Name==_name))
            {
                sheet = (Worksheet) _workbook.Worksheets[_name];
                hasTemplate = true;
            }
            else
            {
                sheet = (Worksheet)_workbook.Worksheets.Add(Missing.Value, _workbook.Worksheets[_workbook.Worksheets.Count], Missing.Value, Missing.Value);
                sheet.Name = _name;
            }
            
            var range = sheet.Cells.Range["A1", _dataTable.Columns.Count.GetColumnChar()+_dataTable.Rows.Count];
            range.EntireColumn.NumberFormat = "@";
            range.Value = sheetContent;
            if (!hasTemplate)
            {
                range.WrapText = true;
                range.ColumnWidth = 20;
                range.EntireRow.AutoFit();
                range.Borders.LineStyle = XlLineStyle.xlContinuous;
            }
        }
    }
}