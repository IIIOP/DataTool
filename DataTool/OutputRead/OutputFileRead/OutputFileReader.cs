using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using DataTool.Auxiliary;
using DataTool.CodeGenerate.Auxiliary;
using DataTool.FormatAccess;
using DataTool.FormatAccess.Auxiliary;
using DataToolInterface.Data;
using DataToolInterface.Format.Config.Global;
using DataToolInterface.Format.Config.Output;
using DataToolInterface.Format.File;
using DataToolLog;

namespace DataTool.OutputRead.OutputFileRead
{
    public class OutputFileReader
    {
        private readonly FieldInfo _fieldInfo;
        private readonly FileInfo _fileInfo;
        public object resultObject { get;private set; }

        public OutputFileReader(FieldInfo paramFieldInfo,FileInfo paramFileInfo)
        {
            if (paramFieldInfo == null) throw new ArgumentNullException(nameof(paramFieldInfo));
            if (paramFileInfo == null) throw new ArgumentNullException(nameof(paramFileInfo));
            
            if (paramFileInfo.Exists)
            {
                _fieldInfo = paramFieldInfo;
                _fileInfo = paramFileInfo;

                ReadOutputFile();
            }
            else
            {
                throw new Exception($@"<{GetType().Name}> File [{paramFileInfo.FullName} not exist!]");
            }
        }

        private void ReadOutputFile()
        {
            LogHelper.DefaultLog.WriteLine($@"[{GetType().Name}] Read Output File [{_fileInfo.FullName}]");

            if (_fieldInfo.GetCustomAttribute<OutputFileAttribute>() is OutputFileAttribute outputFileAttribute)
            {
                if (Enum.TryParse(outputFileAttribute.format,out FileFormatEnum format))
                {
                    var formatReader = FormatReader.GetFormatReader(format,_fieldInfo.FieldType.IsGenericType?_fieldInfo.FieldType.GetGenericArguments().First():_fieldInfo.FieldType, _fileInfo);
                    resultObject = formatReader.resultObject;
                    var propertyInfo = resultObject.GetType().GetProperties().Single(p => p.Name == FileInfoEnum.fileInfo.ToString());
                    var target = propertyInfo.GetValue(resultObject);
                    foreach (var fieldInfo in target.GetType().GetFields())
                    {
                        if (fieldInfo.Name==GlobalEnum.Global.ToString())
                        {
                            var subTarget = fieldInfo.GetValue(target);
                            foreach (var field in subTarget.GetType().GetFields())
                            {
                                var decodeResult = field.Name.DecodeGlobal(outputFileAttribute.path,_fileInfo.DirectoryName.FilterLayerByPath(outputFileAttribute.path));
                                if (decodeResult!=null)
                                {
                                    if (field.GetCustomAttribute<GlobalAttribute>() is GlobalAttribute globalAttribute)
                                    {
                                        if (globalAttribute.type.IsBasicType()&&!globalAttribute.type.IsArray())
                                        {
                                            if (Enum.TryParse(globalAttribute.type,out BasicDataTypeEnum basicDataType))
                                            {
                                                field.SetValue(subTarget,basicDataType.ReadBasicDataTypeByString(decodeResult));
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception("can not decode global");
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            var decodeResult = fieldInfo.Name.DecodeLocal(outputFileAttribute.path,_fileInfo.DirectoryName.FilterLayerByPath(outputFileAttribute.path));
                            if (decodeResult!=null)
                            {
                                fieldInfo.SetValue(target,decodeResult);
                            }
                        }
                    }
                    foreach (var fieldInfo in target.GetType().GetFields())
                    {
                        if (fieldInfo.Name==GlobalEnum.Global.ToString())
                        {
                            var subTarget = fieldInfo.GetValue(target);
                            foreach (var field in subTarget.GetType().GetFields())
                            {
                                var decodeResult = field.Name.DecodeGlobal(outputFileAttribute.name,_fileInfo.Name);
                                if (decodeResult!=null)
                                {
                                    if (field.GetCustomAttribute<GlobalAttribute>() is GlobalAttribute globalAttribute)
                                    {
                                        if (globalAttribute.type.IsBasicType()&&!globalAttribute.type.IsArray())
                                        {
                                            if (Enum.TryParse(globalAttribute.type,out BasicDataTypeEnum basicDataType))
                                            {
                                                field.SetValue(subTarget,basicDataType.ReadBasicDataTypeByString(decodeResult));
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception("can not decode global");
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            var decodeResult = fieldInfo.Name.DecodeLocal(outputFileAttribute.name,_fileInfo.Name);
                            if (decodeResult!=null)
                            {
                                fieldInfo.SetValue(target,decodeResult);
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception($@"[Error] 读取的输入文件的格式不支持 [{outputFileAttribute.format}][{_fileInfo.FullName}]");
                }
            }
            else
            {
                Debug.Assert(false);
            }
        }
    }
}