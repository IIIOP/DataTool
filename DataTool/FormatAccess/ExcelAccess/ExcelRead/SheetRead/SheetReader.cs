using System;
using System.Data;
using System.Linq;
using System.Reflection;
using DataTool.Auxiliary;
using DataTool.CodeGenerate.Auxiliary;
using DataTool.FormatAccess.Auxiliary;
using DataTool.FormatAccess.ExcelAccess.ExcelRead.ContentRead;
using DataToolInterface.Data;
using DataToolInterface.Format.Config.Global;
using DataToolInterface.Format.File.Excel;
using DataToolLog;

namespace DataTool.FormatAccess.ExcelAccess.ExcelRead.SheetRead
{
    public class SheetReader
    {
        private readonly Type _type;
        private readonly DataTable _dataTable;
        private readonly ExcelSheetAttribute _excelSheetAttribute;

        public object resultObject { get; private set; }

        public SheetReader(Type paramType,DataTable paramDatable, ExcelSheetAttribute paramExcelSheetAttribute)
        {
            _type = paramType;
            _dataTable = paramDatable;
            _excelSheetAttribute = paramExcelSheetAttribute;

            ReadSheet();
        }

        private void ReadSheet()
        {
            LogHelper.DefaultLog.WriteLine($@"Read sheet [{_dataTable.TableName}]");
            
            var sheetContentAdvancedSingleAttribute = new SheetContentAdvancedSingleAttribute()
            {
                row = 1.ToString(),
                column = 1.ToString(),
                type = AdvancedDataTypeEnum.Single.ToString(),
                modelRowCount = _dataTable.Rows.Count.ToString(),
                modelColumnCount = _dataTable.Columns.Count.ToString(),
                key = _excelSheetAttribute.key,
                keyPattern = _excelSheetAttribute.keyPattern,
                describe = _excelSheetAttribute.describe,
            };
                
            var excelSheetContentReader = new ContentReader(_type,_dataTable,sheetContentAdvancedSingleAttribute);
            resultObject = excelSheetContentReader.resultObject;

            if (resultObject!=null)
            {
                var propertyInfo = resultObject.GetType().GetProperties().Single(p => p.Name == ExcelFieldNameEnum.sheetInfo.ToString());
                var target = propertyInfo.GetValue(resultObject);
                foreach (var fieldInfo in target.GetType().GetFields())
                {
                    var decodeResult = fieldInfo.Name.DecodeLocal(_excelSheetAttribute.name,_dataTable.TableName);
                    if (decodeResult!=null)
                    {
                        fieldInfo.SetValue(target,decodeResult);
                    }
                }
            }
        }
    }
}