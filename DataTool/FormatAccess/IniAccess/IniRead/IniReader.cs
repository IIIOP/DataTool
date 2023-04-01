using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DataTool.FormatAccess.Auxiliary;
using DataTool.FormatAccess.IniAccess.Auxiliary;
using DataToolInterface.Data;
using DataToolInterface.Format.File;
using DataToolLog;

namespace DataTool.FormatAccess.IniAccess.IniRead
{
    [FormatAccess(format = FileFormatEnum.Ini)]
    public class IniReader:FormatReader
    {
        public IniReader(Type paramType, FileInfo paramFileInfo):base(paramType,paramFileInfo)
        {
            LogHelper.DefaultLog.WriteLine($@">>> Read Ini [{FileInfo.FullName}]");

            ReadFormat();
        }

        private void ReadFormat()
        {
            ReadIni(resultObject,new IniAccessor(FileInfo.FullName));
        }

        private void ReadIni(object paramObject, IniAccessor paramIniReader)
        {
            foreach (var fieldInfo in paramObject.GetType().GetFields())
            {
                var obj = fieldInfo.GetValue(paramObject);
                
                if (obj is IList list)
                {
                    var sections = paramIniReader.GetAllSections();
                    var matchSections = sections.Where(p => Regex.IsMatch(p.Trim(), $@"^{fieldInfo.Name}_\d+$"));
                    foreach (var matchSection in matchSections)
                    {
                        var tempObj = Activator.CreateInstance(fieldInfo.FieldType.GetGenericArguments().First());
                        DecodeIniField(paramIniReader, matchSection, tempObj);
                        list.Add(tempObj);
                    }
                }
                else
                {
                    DecodeIniField(paramIniReader, fieldInfo.Name, obj);
                }
            }
        }

        private static void DecodeIniField(IniAccessor paramIniReader, string paramSection, object obj)
        {
            foreach (var field in obj.GetType().GetFields())
            {
                var key = BasicDataType.BasicDataTypeNameConvertDictionary
                    .Single(item => item.Value == field.FieldType.Name).Key;
                switch (key)
                {
                    case BasicDataTypeEnum.INT8:
                        field.SetValue(obj, sbyte.Parse(paramIniReader.ReadValue(paramSection, field.Name)));
                        break;
                    case BasicDataTypeEnum.INT16:
                        field.SetValue(obj, Int16.Parse(paramIniReader.ReadValue(paramSection, field.Name)));
                        break;
                    case BasicDataTypeEnum.INT32:
                        field.SetValue(obj, Int32.Parse(paramIniReader.ReadValue(paramSection, field.Name)));
                        break;
                    case BasicDataTypeEnum.INT64:
                        field.SetValue(obj, Int64.Parse(paramIniReader.ReadValue(paramSection, field.Name)));
                        break;
                    case BasicDataTypeEnum.UINT8:
                        field.SetValue(obj, byte.Parse(paramIniReader.ReadValue(paramSection, field.Name)));
                        break;
                    case BasicDataTypeEnum.UINT16:
                        field.SetValue(obj, UInt16.Parse(paramIniReader.ReadValue(paramSection, field.Name)));
                        break;
                    case BasicDataTypeEnum.UINT32:
                        field.SetValue(obj, UInt32.Parse(paramIniReader.ReadValue(paramSection, field.Name)));
                        break;
                    case BasicDataTypeEnum.UINT64:
                        field.SetValue(obj, UInt64.Parse(paramIniReader.ReadValue(paramSection, field.Name)));
                        break;
                    case BasicDataTypeEnum.FLOAT:
                        field.SetValue(obj, Single.Parse(paramIniReader.ReadValue(paramSection, field.Name)));
                        break;
                    case BasicDataTypeEnum.DOUBLE:
                        field.SetValue(obj, Double.Parse(paramIniReader.ReadValue(paramSection, field.Name)));
                        break;
                    case BasicDataTypeEnum.DECIMAL:
                        field.SetValue(obj, Decimal.Parse(paramIniReader.ReadValue(paramSection, field.Name)));
                        break;
                    case BasicDataTypeEnum.String:
                        field.SetValue(obj, paramIniReader.ReadValue(paramSection, field.Name));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}