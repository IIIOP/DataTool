using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using DataTool.Auxiliary;
using DataTool.CodeGenerate.Auxiliary;
using DataTool.FormatAccess.Auxiliary;
using DataTool.FormatAccess.ExcelAccess.Auxiliary;
using DataTool.FormatAccess.ExcelAccess.ExcelRead.SheetRead;
using DataToolInterface.Data;
using DataToolInterface.Format.File;
using DataToolInterface.Format.File.Excel;
using DataToolLog;

namespace DataTool.FormatAccess.ExcelAccess.ExcelRead
{
    [FormatAccess(format = FileFormatEnum.Excel)]
    public class ExcelReader:FormatReader
    {
        public ExcelReader(Type paramType,FileInfo paramFileInfo):base(paramType,paramFileInfo)
        {
            ReadExcel();
        }

        private void ReadExcel()
        {
            LogHelper.DefaultLog.WriteLine($@">>> Read Excel [{FileInfo.FullName}]");
            
            var dataSet = ExcelHelper.GetDataSet(FileInfo);

            foreach (var fieldInfo in resultObject.GetType().GetFields())
            {
                if (fieldInfo.GetCustomAttribute<ExcelSheetAttribute>() is ExcelSheetAttribute sheetAttribute)
                {
                    var sheets = dataSet.Tables.Cast<DataTable>()
                        .Where(item => Regex.IsMatch(item.TableName,$@"^{sheetAttribute.name.ReplaceVariableToRegex()}$")).ToArray();

                    if (sheets.Any())
                    {
                        if (sheetAttribute.type==AdvancedDataTypeEnum.Single.ToString())
                        {
                            if (sheets.Length==1)
                            {
                                var excelSheetReader = new SheetReader(fieldInfo.FieldType,sheets.First(),sheetAttribute);
                                fieldInfo.SetValue(resultObject,excelSheetReader.resultObject);
                            }
                            else
                            {
                                throw new Exception($@"[Error] excel multi match [{sheetAttribute.name}]");
                            }
                        }
                        else if (sheetAttribute.type==AdvancedDataTypeEnum.Multiple.ToString())
                        {
                            if (fieldInfo.GetValue(resultObject) is IList list)
                            {
                                foreach (var sheet in sheets)
                                {
                                    var excelSheetReader = new SheetReader(fieldInfo.FieldType.GetGenericArguments().First(),sheet,sheetAttribute);
                                    if (excelSheetReader.resultObject!=null)
                                    {
                                        list.Add(excelSheetReader.resultObject);
                                    }
                                }
                            }
                        }
                        else if (sheetAttribute.type.IsArray())
                        {
                            if (fieldInfo.GetValue(resultObject) is IList list)
                            {
                                foreach (var sheet in sheets)
                                {
                                    var excelSheetReader = new SheetReader(fieldInfo.FieldType.GetGenericArguments().First(),sheet,sheetAttribute);
                                    if (excelSheetReader.resultObject!=null)
                                    {
                                        list.Add(excelSheetReader.resultObject);
                                    }
                                }
                            }
                        }
                        else if (!sheetAttribute.type.IsArray())
                        {
                            if (sheets.Length==1)
                            {
                                var excelSheetReader = new SheetReader(fieldInfo.FieldType,sheets.First(),sheetAttribute);
                                fieldInfo.SetValue(resultObject,excelSheetReader.resultObject);
                            }
                            else
                            {
                                throw new Exception($@"[Error] excel multi match [{sheetAttribute.name}]");
                            }
                        }
                    }
                    else
                    {
                        throw new Exception($@"[Error] excel not found [{sheetAttribute.name}]");
                    }
                }
                else if (fieldInfo.GetCustomAttribute<ExcelSheetDefaultAttribute>() is ExcelSheetDefaultAttribute sheetDefaultAttribute)
                {
                    var sheets = dataSet.Tables.Cast<DataTable>()
                        .Where(item => Regex.IsMatch(item.TableName,$@"^{sheetDefaultAttribute.name.ReplaceVariableToRegex()}$")).ToArray();

                    if (sheets.Any())
                    {
                        if (sheetDefaultAttribute.type==AdvancedDataTypeEnum.Single.ToString())
                        {
                            if (sheets.Length==1)
                            {
                                fieldInfo.SetValue(resultObject,sheets.First());
                            }
                            else
                            {
                                throw new Exception($@"[Error] excel multi match [{sheetDefaultAttribute.name}]");
                            }
                        }
                        else if (sheetDefaultAttribute.type==AdvancedDataTypeEnum.Multiple.ToString())
                        {
                            if (fieldInfo.GetValue(resultObject) is IList list)
                            {
                                foreach (var sheet in sheets)
                                {
                                    list.Add(sheet);
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new Exception($@"[Error] excel not found [{sheetDefaultAttribute.name}]");
                    }
                }
                else if (fieldInfo.GetCustomAttribute<ExcelDefaultAttribute>() is ExcelDefaultAttribute _)
                {
                    if (fieldInfo.GetValue(resultObject) is IList list)
                    {
                        foreach (var dataTable in dataSet.Tables.Cast<DataTable>())
                        {
                            list.Add(dataTable);
                        }
                    }
                }
            }
        }
    }
}