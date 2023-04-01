using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using DataTool.Auxiliary;
using DataTool.CodeGenerate.Auxiliary;
using DataTool.FormatAccess.Auxiliary;
using DataTool.InputRead.InputFileRead;
using DataToolInterface.Data;
using DataToolInterface.Format.Config.Global;
using DataToolInterface.Format.Config.Input;
using DataToolLog;

namespace DataTool.InputRead.InputPathRead
{
    public class InputPathReader
    {
        private readonly Type _type;
        private readonly DirectoryInfo _directoryInfo;
        private readonly InputPathAttribute _inputPathAttribute;

        public object resultObject { get; set; }

        public InputPathReader(Type paramType,DirectoryInfo paramDirectoryInfo)
        {
            if (paramType == null) throw new ArgumentNullException(nameof(paramType));
            if (paramDirectoryInfo == null) throw new ArgumentNullException(nameof(paramDirectoryInfo));
            
            if (paramDirectoryInfo.Exists)
            {
                _type = paramType;
                _directoryInfo = paramDirectoryInfo;
                _inputPathAttribute = _type.GetCustomAttribute<InputPathAttribute>();
                
                ReadInputPath();
            }
            else
            {
                throw new Exception($@"<{GetType().Name}> Directory[{paramDirectoryInfo.FullName}] not exist");
            }
            
        }
        
        private void ReadInputPath()
        {
            LogHelper.DefaultLog.WriteLine($@"[{GetType().Name}] Read Input Path [{_directoryInfo.FullName}]");
            
            resultObject = Activator.CreateInstance(_type);
            foreach (var fieldInfo in resultObject.GetType().GetFields())
            {
                if (fieldInfo.GetCustomAttribute<InputFileAttribute>() is InputFileAttribute inputFileAttribute)
                {
                    var directoryInfos = new List<DirectoryInfo>();
                    if (string.IsNullOrWhiteSpace(inputFileAttribute.path))
                    {
                        directoryInfos.Add(_directoryInfo);
                    }
                    else
                    {
                        inputFileAttribute.path = inputFileAttribute.path.FormatToPath();
                        var parent = new DirectoryInfo(Path.Combine(_directoryInfo.FullName,inputFileAttribute.path.GetPathBeforeVariable()));
                        if (parent.Exists)
                        {
                            var regex = new Regex($@"^({Regex.Escape(_directoryInfo.FullName.FormatToPath())})?{inputFileAttribute.path.ReplaceVariableToRegex().TrimEnd('\\')}$");
                            directoryInfos.AddRange(parent.GetDirectories("*", SearchOption.AllDirectories).Append(parent).Where(item => regex.IsMatch(item.FullName)));
                        }
                        else
                        {
                            throw new Exception($@"<{GetType().Name}> parent Directory[{parent.FullName}] not exist");
                        }
                    }
                    
                    
                    var nameRegex = new Regex($@"^{inputFileAttribute.name.ReplaceVariableToRegex()}$");
                    var fileInfos = new List<FileInfo>();
                    if (inputFileAttribute.type==AdvancedDataTypeEnum.Single.ToString()||(inputFileAttribute.type!=AdvancedDataTypeEnum.Multiple.ToString()&&!inputFileAttribute.type.IsArray()))
                    {
                        if (directoryInfos.Count==1)
                        {
                            var files = directoryInfos.First().GetFiles().Where(item => nameRegex.IsMatch(item.Name)).ToList();
                            if (files.Count==1)
                            {
                                fileInfos.Add(files.First());
                            }
                            else
                            {
                                LogHelper.DefaultLog.WriteLine($@"[Error]########### 输入文件存在多个匹配项 [{inputFileAttribute.name}]");
                            }
                        }
                        else
                        {
                            throw new Exception("未找到目录");
                        }
                    }
                    else
                    {
                        if (directoryInfos.Count>=1)
                        {
                            foreach (var directoryInfo in directoryInfos)
                            {
                                var files = directoryInfo.GetFiles().Where(p => nameRegex.IsMatch(p.Name)).ToList();
                                if (files.Any())
                                {
                                    fileInfos.AddRange(files);
                                }
                                else
                                {
                                    LogHelper.DefaultLog.WriteLine($@"<{GetType().Name}> no match file");
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            LogHelper.DefaultLog.WriteLine($@"<{GetType().Name}> no match directory");
                            continue;
                        }
                    }
                    if (fieldInfo.FieldType.IsGenericType)
                    {
                        if (fieldInfo.GetValue(resultObject) is IList list)
                        {
                            foreach (var fileInfo in fileInfos)
                            {
                                var inputFileReader = new InputFileReader(fieldInfo, fileInfo);
                                list.Add(inputFileReader.resultObject);
                            }
                        }
                    }
                    else
                    {
                        if (fileInfos.Count==1)
                        {
                            var inputFileReader = new InputFileReader(fieldInfo, fileInfos.First());
                            fieldInfo.SetValue(resultObject,inputFileReader.resultObject);
                        }
                        else
                        {
                            throw new Exception($@"<{GetType().Name}> multi match file");
                        }
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(_inputPathAttribute.path))
            {
                var propertyInfo = resultObject.GetType().GetProperties().Single(p => p.Name == InputEnum.pathInfo.ToString());
                var target = propertyInfo.GetValue(resultObject);
                foreach (var subFieldInfo in target.GetType().GetFields())
                {
                    if (subFieldInfo.Name==GlobalEnum.Global.ToString())
                    {
                        var subTarget = subFieldInfo.GetValue(target);
                        foreach (var field in subTarget.GetType().GetFields())
                        {
                            var decodeResult = field.Name.DecodeGlobal(_inputPathAttribute.path,_directoryInfo.FullName.FilterLayerByPath(_inputPathAttribute.path));
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
                        var decodeResult = subFieldInfo.Name.DecodeLocal(_inputPathAttribute.path,_directoryInfo.FullName.FilterLayerByPath(_inputPathAttribute.path));
                        if (decodeResult!=null)
                        {
                            subFieldInfo.SetValue(target,decodeResult);
                        }
                    }
                }
            }
        }
    }
}