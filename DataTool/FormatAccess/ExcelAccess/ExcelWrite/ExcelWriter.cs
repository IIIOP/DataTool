using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using DataTool.FormatAccess.Auxiliary;
using DataTool.FormatAccess.ExcelAccess.Auxiliary;
using DataTool.FormatAccess.ExcelAccess.ExcelWrite.ExcelSheetWrite;
using DataToolInterface.Data;
using DataToolInterface.Format.File;
using DataToolInterface.Format.File.Excel;
using Microsoft.Office.Interop.Excel;
using DataTable = System.Data.DataTable;

namespace DataTool.FormatAccess.ExcelAccess.ExcelWrite
{
    [FormatAccess(format = FileFormatEnum.Excel)]
    public class ExcelWriter:FormatWriter
    {
        public ExcelWriter(object paramObject,FileInfo paramFileInfo):base(paramObject,paramFileInfo)
        {
            WriteFormat();
        }

        private void WriteFormat()
        {
            var application = new ApplicationClass();
            ExcelHelper.GetWindowThreadProcessId(application.Hwnd, out var pId);
            var process = Process.GetProcessById(pId);
            try
            {
                application.Visible = false;
                application.UserControl = true;
                application.DisplayAlerts = false;
                application.AlertBeforeOverwriting = false;

                var excelFileAttribute = Object.GetType().GetCustomAttribute<ExcelFileAttribute>();

                bool hasTemplate = false;
                Workbook workbook;
                if (FileInfo.Name.StartsWith(@"writeInput")&&File.Exists(Path.Combine(FileInfo.DirectoryName,FileInfo.Name.Substring(@"writeInput".Length))))
                {
                    workbook = application.Application.Workbooks.Add(Path.Combine(FileInfo.DirectoryName,FileInfo.Name.Substring(@"writeInput".Length)));
                    hasTemplate = true;
                }
                else if (!string.IsNullOrWhiteSpace(excelFileAttribute.template)&&File.Exists(Path.Combine(Environment.CurrentDirectory,"DefaultConfig",excelFileAttribute.template)))
                {
                    workbook = application.Application.Workbooks.Add(Path.Combine(Environment.CurrentDirectory,"DefaultConfig",excelFileAttribute.template));
                    hasTemplate = true;
                }
                else
                {
                    workbook = application.Application.Workbooks.Add(Type.Missing);
                }
                
                foreach (var fieldInfo in Object.GetType().GetFields())
                {
                    if (fieldInfo.GetCustomAttribute<ExcelSheetAttribute>() is ExcelSheetAttribute sheetAttribute)
                    {
                        var subObject = fieldInfo.GetValue(Object);
                        if (subObject is IList list)
                        {
                            for (var i = 0; i < list.Count; i++)
                            {
                                var _ = new ExcelSheetWriter(list[i],workbook,sheetAttribute);
                            }
                        }
                        else
                        {
                            var _ = new ExcelSheetWriter(subObject,workbook,sheetAttribute);
                        }
                    }
                    else if (fieldInfo.GetCustomAttribute<ExcelSheetDefaultAttribute>() is ExcelSheetDefaultAttribute sheetDefaultAttribute)
                    {
                        var subObject = fieldInfo.GetValue(Object);
                        if (subObject is IList list)
                        {
                            for (var i = 0; i < list.Count; i++)
                            {
                                var _ = new ExcelSheetDefaultWriter(list[i],workbook,sheetDefaultAttribute);
                            }
                        }
                        else
                        {
                            var _ = new ExcelSheetDefaultWriter(subObject,workbook,sheetDefaultAttribute);
                        }
                    }
                    else if (fieldInfo.GetCustomAttribute<ExcelDefaultAttribute>() is ExcelDefaultAttribute excelDefault)
                    {
                        if (fieldInfo.GetValue(Object) is List<DataTable> dataTables)
                        {
                            foreach (var dataTable in dataTables)
                            {
                                var sheetContent = new object[dataTable.Rows.Count, dataTable.Columns.Count];
                                for (var i = 0; i < dataTable.Rows.Count; i++)
                                {
                                    for (var j = 0; j < dataTable.Columns.Count; j++)
                                    {
                                        sheetContent[i, j] = dataTable.Rows[i][j];
                                    }
                                }
                                
                                bool hasSheetTemplate = false;
                                Worksheet sheet;
                                if (workbook.Worksheets.Cast<Worksheet>().Any(p=>p.Name==dataTable.TableName))
                                {
                                    sheet = (Worksheet) workbook.Worksheets[dataTable.TableName];
                                    hasSheetTemplate = true;
                                }
                                else
                                {
                                    sheet = (Worksheet)workbook.Worksheets.Add(Missing.Value, workbook.Worksheets[workbook.Worksheets.Count], Missing.Value, Missing.Value);
                                    sheet.Name = dataTable.TableName;
                                }
            
                                var range = sheet.Cells.Range["A1",dataTable.Columns.Count.GetColumnChar()+dataTable.Rows.Count];
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
                }

                if (!hasTemplate)
                {
                    var defaultSheet = (Worksheet)workbook.Worksheets[1];
                    defaultSheet.Delete();
                }
                
                workbook.SaveAs(FileInfo.FullName);
                workbook.Close(Missing.Value,Missing.Value,Missing.Value);
            }
            finally
            {
                application.Quit();
                application = null;
                process.Kill();
            }
        }
    }
}