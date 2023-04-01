using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using DataTool.CodeGenerate.Auxiliary;
using DataTool.FormatAccess.Auxiliary;
using DataTool.FormatAccess.BinaryAccess.Auxiliary;
using DataToolInterface.Data;
using DataToolInterface.Format.File;
using DataToolInterface.Format.File.Binary;
using DataToolLog;

namespace DataTool.FormatAccess.BinaryAccess.BinaryWrite
{
    [FormatAccess(format = FileFormatEnum.Binary)]
    public class BinaryWriter:FormatWriter
    {
        private EndianEnum _endianEnum;
        
        public BinaryWriter(object paramObject,FileInfo paramFileInfo):base(paramObject,paramFileInfo)
        {
            WriteBinary();
        }

        private void WriteBinary()
        {
            LogHelper.DefaultLog.WriteLine($@"Write Binary [{FileInfo.FullName}]");

            Debug.Assert(Object.GetType().GetCustomAttribute<BinaryFileAttribute>()!=null);
            if (Enum.TryParse(Object.GetType().GetCustomAttribute<BinaryFileAttribute>().endian, out _endianEnum))
            {
                var binaryWriter = new System.IO.BinaryWriter(new FileStream(FileInfo.FullName, FileMode.Create));
                try
                {
                    WriteContent(Object,binaryWriter);
                }
                finally
                {
                    binaryWriter.Flush();
                    binaryWriter.Close();
                    ExtentMethod.GenerateDescribeFile(Object,new FileInfo($@"{FileInfo.FullName}.xml"));
                }
            }
            else
            {
                throw new Exception("endian abnormal");
            }
        }

        private void WriteContent(object paramObject, System.IO.BinaryWriter paramBinaryWriter)
        {
            foreach (var fieldInfo in paramObject.GetType().GetFields())
            {
                if (fieldInfo.GetCustomAttribute<BinaryBasicSingleAttribute>() is BinaryBasicSingleAttribute basicSingleAttribute)
                {
                    Debug.Assert(Enum.IsDefined(typeof(BasicDataTypeEnum), basicSingleAttribute.type));
                    var basicDataTypeEnum = (BasicDataTypeEnum)Enum.Parse(typeof(BasicDataTypeEnum), basicSingleAttribute.type);
                    basicDataTypeEnum.WriteBasicDataTypeByBinary(fieldInfo.GetValue(paramObject),paramBinaryWriter,_endianEnum);
                }
                else if (fieldInfo.GetCustomAttribute<BinaryBasicMultipleAttribute>() is BinaryBasicMultipleAttribute basicMultipleAttribute)
                {
                    Debug.Assert(fieldInfo.GetValue(paramObject) is IList);
                    if (fieldInfo.GetValue(paramObject) is IList list)
                    {
                        Debug.Assert(Enum.IsDefined(typeof(BasicDataTypeEnum),basicMultipleAttribute.type.BaseType()));
                        var basicDataTypeEnum = (BasicDataTypeEnum)Enum.Parse(typeof(BasicDataTypeEnum), basicMultipleAttribute.type.BaseType());
                        foreach (var item in list)
                        {
                            basicDataTypeEnum.WriteBasicDataTypeByBinary(item,paramBinaryWriter,_endianEnum);
                        }
                    }
                }
                else if (fieldInfo.GetCustomAttribute<BinaryAdvancedSingleAttribute>() is BinaryAdvancedSingleAttribute advancedSingleAttribute)
                {
                    WriteContent(fieldInfo.GetValue(paramObject), paramBinaryWriter);
                }
                else if (fieldInfo.GetCustomAttribute<BinaryAdvancedMultipleAttribute>() is BinaryAdvancedMultipleAttribute advancedMultipleAttribute)
                {
                    Debug.Assert(fieldInfo.GetValue(paramObject) is IList);
                    if (fieldInfo.GetValue(paramObject) is IList list)
                    {
                        foreach (var item in list)
                        {
                            WriteContent(item, paramBinaryWriter);
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