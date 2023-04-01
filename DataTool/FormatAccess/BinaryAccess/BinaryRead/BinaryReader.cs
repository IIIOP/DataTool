using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using DataTool.Auxiliary;
using DataTool.CodeGenerate.Auxiliary;
using DataTool.FormatAccess.Auxiliary;
using DataTool.FormatAccess.BinaryAccess.Auxiliary;
using DataToolInterface.Data;
using DataToolInterface.Format.File;
using DataToolInterface.Format.File.Binary;
using DataToolLog;

namespace DataTool.FormatAccess.BinaryAccess.BinaryRead
{
    [FormatAccess(format = FileFormatEnum.Binary)]
    public class BinaryReader:FormatReader
    {
        private EndianEnum _endianEnum;
        public BinaryReader(Type paramType,FileInfo paramFileInfo) : base(paramType,paramFileInfo)
        {
            ReadBinary();
        }

        private void ReadBinary()
        {
            LogHelper.DefaultLog.WriteLine($@">>> Read Binary [{FileInfo.FullName}]");
            
            if (Enum.TryParse(Type.GetCustomAttribute<BinaryFileAttribute>().endian, out _endianEnum))
            {
                using (var file  = FileInfo.Open(FileMode.Open,FileAccess.Read))
                {
                    var binaryReader = new System.IO.BinaryReader(file);
                    try
                    {
                        ReadContent(resultObject,binaryReader);
                    }
                    finally
                    {
                        ExtentMethod.GenerateDescribeFile(resultObject,new FileInfo($@"{FileInfo.FullName}.xml"));
                        binaryReader.Close();
                    }
                }
            }
            else
            {
                throw new Exception("endain 异常");
            }
        }

        private void ReadContent(object paramObject, System.IO.BinaryReader paramBinaryReader)
        {
            foreach (var fieldInfo in paramObject.GetType().GetFields())
            {
                if (fieldInfo.GetCustomAttribute<BinaryBasicSingleAttribute>() is BinaryBasicSingleAttribute basicSingleAttribute)
                {
                    Debug.Assert(Enum.IsDefined(typeof(BasicDataTypeEnum), basicSingleAttribute.type));
                    var basicDataTypeEnum = (BasicDataTypeEnum)Enum.Parse(typeof(BasicDataTypeEnum), basicSingleAttribute.type);
                    fieldInfo.SetValue(paramObject,basicDataTypeEnum.ReadBasicDataTypeByBinary(paramBinaryReader, _endianEnum));
                }
                else if (fieldInfo.GetCustomAttribute<BinaryBasicMultipleAttribute>() is BinaryBasicMultipleAttribute basicMultipleAttribute)
                {
                    Debug.Assert(fieldInfo.GetValue(paramObject) is IList);
                    if (fieldInfo.GetValue(paramObject) is IList list)
                    {
                        if (!int.TryParse(basicMultipleAttribute.size,out var count))
                        {
                            count = int.Parse(paramObject.GetFieldValueByName(basicMultipleAttribute.size).ToString());
                        }

                        Debug.Assert(Enum.IsDefined(typeof(BasicDataTypeEnum),basicMultipleAttribute.type.BaseType()));
                        var basicDataTypeEnum = (BasicDataTypeEnum)Enum.Parse(typeof(BasicDataTypeEnum), basicMultipleAttribute.type.BaseType());
                        for (var i = 0; i < count; i++)
                        {
                            list.Add(basicDataTypeEnum.ReadBasicDataTypeByBinary(paramBinaryReader,_endianEnum));
                        }
                    }
                }
                else if (fieldInfo.GetCustomAttribute<BinaryAdvancedSingleAttribute>() is BinaryAdvancedSingleAttribute advancedSingleAttribute)
                {
                    ReadContent(fieldInfo.GetValue(paramObject),paramBinaryReader);
                }
                else if (fieldInfo.GetCustomAttribute<BinaryAdvancedMultipleAttribute>() is BinaryAdvancedMultipleAttribute advancedMultipleAttribute)
                {
                    if (!int.TryParse(advancedMultipleAttribute.size,out var count))
                    {
                        count = int.Parse(paramObject.GetFieldValueByName(advancedMultipleAttribute.size).ToString());
                    }

                    Debug.Assert(fieldInfo.GetValue(paramObject) is IList);
                    if (fieldInfo.GetValue(paramObject) is IList list)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var temp = Activator.CreateInstance(fieldInfo.FieldType.GetGenericArguments().First());
                            ReadContent(temp,paramBinaryReader);
                            list.Add(temp);
                        }
                    }
                }
                else
                {
                    Debug.Assert(false);
                }
            }
        }
    }
}