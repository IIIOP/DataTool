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
using DataToolInterface.Format.Config.Input;
using DataToolInterface.Format.File;
using DataToolLog;

namespace DataTool.InputRead.InputFileRead
{
    public class InputFileReader
    {
        private readonly FieldInfo _fieldInfo;
        private readonly FileInfo _fileInfo;

        public object resultObject { get; private set; }

        public InputFileReader(FieldInfo paramFieldInfo,FileInfo paramFileInfo)
        {
            if (paramFieldInfo == null) throw new ArgumentNullException(nameof(paramFieldInfo));
            if (paramFileInfo == null) throw new ArgumentNullException(nameof(paramFileInfo));

            if (paramFileInfo.Exists)
            {
                _fieldInfo = paramFieldInfo;
                _fileInfo = paramFileInfo;
            
                ReadInputFile();
            }
            else
            {
                throw new Exception($@"<{GetType().Name}> File [{paramFileInfo.FullName} not exist!]");
            }
        }

        private void ReadInputFile()
        {
            LogHelper.DefaultLog.WriteLine($@"[{GetType().Name}] Read Input File [{_fileInfo.FullName}]");
            
            if (_fieldInfo.GetCustomAttribute<InputFileAttribute>() is InputFileAttribute inputFileAttribute)
            {
                if (Enum.TryParse(inputFileAttribute.format,out FileFormatEnum format))
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
                                var decodeResult = field.Name.DecodeGlobal(inputFileAttribute.path,_fileInfo.DirectoryName.FilterLayerByPath(inputFileAttribute.path));
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
                            var decodeResult = fieldInfo.Name.DecodeLocal(inputFileAttribute.path,_fileInfo.DirectoryName.FilterLayerByPath(inputFileAttribute.path));
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
                                var decodeResult = field.Name.DecodeGlobal(inputFileAttribute.name,_fileInfo.Name);
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
                            var decodeResult = fieldInfo.Name.DecodeLocal(inputFileAttribute.name,_fileInfo.Name);
                            if (decodeResult!=null)
                            {
                                fieldInfo.SetValue(target,decodeResult);
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception($@"[Error] 读取的输入文件的格式不支持 [{inputFileAttribute.format}][{_fileInfo.FullName}]");
                }
            }
            else
            {
                Debug.Assert(false);
            }
        }
    }
}