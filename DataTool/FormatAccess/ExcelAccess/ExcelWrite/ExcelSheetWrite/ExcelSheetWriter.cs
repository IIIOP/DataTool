using System.Linq;
using System.Reflection;
using DataTool.Auxiliary;
using DataTool.FormatAccess.ExcelAccess.Auxiliary;
using DataTool.FormatAccess.ExcelAccess.ExcelWrite.ExcelSheetContentWrite;
using DataToolInterface.Format.File.Excel;
using DataToolLog;
using Microsoft.Office.Interop.Excel;

namespace DataTool.FormatAccess.ExcelAccess.ExcelWrite.ExcelSheetWrite
{
    public class ExcelSheetWriter
    {
        private readonly object _object;
        private readonly Workbook _workbook;
        private readonly ExcelSheetAttribute _excelSheetAttribute;
        private readonly string _name;

        private System.Data.DataTable _dataTable;
        public ExcelSheetWriter(object paramObject,Workbook paramWorkbook,ExcelSheetAttribute paramExcelSheetAttribute)
        {
            _object = paramObject;
            _workbook = paramWorkbook;
            _excelSheetAttribute = paramExcelSheetAttribute;

            _name = _excelSheetAttribute.name.EncodeVariable(_object);

            _dataTable = new System.Data.DataTable();

            DoWork();
        }

        private void DoWork()
        {
            LogHelper.DefaultLog.WriteLine($@"Write sheet [{_name}]");
            
            var excelSheetContentWriter = new ExcelSheetContentWriter(_object,_dataTable,_excelSheetAttribute);
            
            WriteSheet();
        }

        private void WriteSheet()
        {
            int headRow = int.MaxValue;
            int headColumn = int.MaxValue;
            foreach (var fieldInfo in _object.GetType().GetFields())
            {
                if (fieldInfo.GetCustomAttribute<SheetContentBasicSingleAttribute>() is SheetContentBasicSingleAttribute basicSingleAttribute)
                {
                    if (headRow > int.Parse(basicSingleAttribute.row))
                        headRow = int.Parse(basicSingleAttribute.row);

                    if (headColumn > int.Parse(basicSingleAttribute.column))
                        headColumn = int.Parse(basicSingleAttribute.column);
                }
                else if (fieldInfo.GetCustomAttribute<SheetContentBasicMultipleAttribute>() is SheetContentBasicMultipleAttribute basicMultiple)
                {
                    if (headRow > int.Parse(basicMultiple.row))
                        headRow = int.Parse(basicMultiple.row);

                    if (headColumn > int.Parse(basicMultiple.column))
                        headColumn = int.Parse(basicMultiple.column);
                }
                else if (fieldInfo.GetCustomAttribute<SheetContentAdvancedSingleAttribute>() is SheetContentAdvancedSingleAttribute advancedSingle)
                {
                    if (headRow > int.Parse(advancedSingle.row))
                        headRow = int.Parse(advancedSingle.row);

                    if (headColumn > int.Parse(advancedSingle.column))
                        headColumn = int.Parse(advancedSingle.column);
                }
                else if (fieldInfo.GetCustomAttribute<SheetContentAdvancedMultipleAttribute>() is SheetContentAdvancedMultipleAttribute advancedMultiple)
                {
                    if (headRow > int.Parse(advancedMultiple.row))
                        headRow = int.Parse(advancedMultiple.row);

                    if (headColumn > int.Parse(advancedMultiple.column))
                        headColumn = int.Parse(advancedMultiple.column);
                }
            }

            bool hasTemplate = false;
            Worksheet sheet;
            if (_workbook.Worksheets.Cast<Worksheet>().Any(p=>p.Name==_name))
            {
                sheet = (Worksheet) _workbook.Worksheets[_name];
                hasTemplate = true;
                
                var sheetContent = new object[sheet.UsedRange.Rows.Count-headRow+1, sheet.UsedRange.Columns.Count-headColumn+1];
                var range = sheet.Cells.Range[headColumn.GetColumnChar()+headRow, sheet.UsedRange.Columns.Count.GetColumnChar()+sheet.UsedRange.Rows.Count];
                //range.EntireColumn.NumberFormat = "@";
                range.Value = sheetContent;
            }
            else
            {
                sheet = (Worksheet)_workbook.Worksheets.Add(Missing.Value, _workbook.Worksheets[_workbook.Worksheets.Count], Missing.Value, Missing.Value);
                sheet.Name = _name;
            }
            
            if (_dataTable.Rows.Count>0&&_dataTable.Columns.Count>0)
            {
                var sheetContent = new object[_dataTable.Rows.Count-headRow+1, _dataTable.Columns.Count-headColumn+1];
                for (var i = 0; i < _dataTable.Rows.Count-headRow+1; i++)
                {
                    for (var j = 0; j < _dataTable.Columns.Count-headColumn+1; j++)
                    {
                        sheetContent[i, j] = _dataTable.Rows[i+headRow-1][j+headColumn-1];
                    }
                }
            
                var range = sheet.Cells.Range[headColumn.GetColumnChar()+headRow, _dataTable.Columns.Count.GetColumnChar()+_dataTable.Rows.Count];
                //range.EntireColumn.NumberFormat = "@";
                range.Value = sheetContent;
                if (!hasTemplate)
                {
                    range.WrapText = true;
                    range.ColumnWidth = 20;
                    range.EntireRow.AutoFit();
                    range.Borders.LineStyle = XlLineStyle.xlContinuous;
                }
            }
            else
            {
                LogHelper.DefaultLog.WriteLine($@"脚本没有向 [{_name}]sheet 里写数据哦!");
            }
        }
    }
}