using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using DataTool.Auxiliary;
using DataTool.CodeGenerate.Auxiliary;
using DataTool.FormatAccess.Auxiliary;
using DataTool.OutputRead.OutputFileRead;
using DataToolInterface.Data;
using DataToolInterface.Format.Config.Global;
using DataToolInterface.Format.Config.Output;
using DataToolLog;

namespace DataTool.OutputRead.OutputPathRead
{
    public class OutputPathReader
    {
        private readonly Type _type;
        private readonly DirectoryInfo _directoryInfo;
        private readonly OutputPathAttribute _outputPathAttribute;
        public object resultObject { get;private set; }

        public OutputPathReader(Type paramType, DirectoryInfo paramDirectoryInfo)
        {
            if (paramType == null) throw new ArgumentNullException(nameof(paramType));
            if (paramDirectoryInfo == null) throw new ArgumentNullException(nameof(paramDirectoryInfo));

            if (paramDirectoryInfo.Exists)
            {
                _type = paramType;
                _directoryInfo = paramDirectoryInfo;
                _outputPathAttribute = _type.GetCustomAttribute<OutputPathAttribute>();
                
                ReadOutputPath();
            }
            else
            {
                throw new Exception($@"<{GetType().Name}> Directory[{paramDirectoryInfo.FullName}] not exist");
            }
        }

        private void ReadOutputPath()
        {
            LogHelper.DefaultLog.WriteLine($@"[{GetType().Name}] Read Output Path [{_directoryInfo.FullName}]");
            
            resultObject = Activator.CreateInstance(_type);
            foreach (var fieldInfo in resultObject.GetType().GetFields())
            {
                if (fieldInfo.GetCustomAttribute<OutputFileAttribute>() is OutputFileAttribute outputFileAttribute)
                {
                    var directoryInfos = new List<DirectoryInfo>();
                    if (string.IsNullOrWhiteSpace(outputFileAttribute.path))
                    {
                        directoryInfos.Add(_directoryInfo);
                    }
                    else
                    {
                        outputFileAttribute.path = outputFileAttribute.path.FormatToPath();
                        var parent = new DirectoryInfo(Path.Combine(_directoryInfo.FullName,outputFileAttribute.path.GetPathBeforeVariable()));
                        if (parent.Exists)
                        {
                            var regex = new Regex($@"^({Regex.Escape(_directoryInfo.FullName.FormatToPath())})?{outputFileAttribute.path.ReplaceVariableToRegex().TrimEnd('\\')}$");
                            directoryInfos.AddRange(parent.GetDirectories("*", SearchOption.AllDirectories).Append(parent).Where(item => regex.IsMatch(item.FullName)));
                        }
                        else
                        {
                            throw new Exception($@"<{GetType().Name}> parent Directory[{parent.FullName}] not exist");
                        }
                    }

                    var nameRegex = new Regex($@"^{outputFileAttribute.name.ReplaceVariableToRegex()}$");
                    var fileInfos = new List<FileInfo>();
                    if (outputFileAttribute.type==AdvancedDataTypeEnum.Single.ToString()||(outputFileAttribute.type!=AdvancedDataTypeEnum.Multiple.ToString()&&!outputFileAttribute.type.IsArray()))
                    {
                        if (directoryInfos.Count==1)
                        {
                            var files = directoryInfos.First().GetFiles().Where(p => nameRegex.IsMatch(p.Name)).ToList();
                            if (files.Count==1)
                            {
                                fileInfos.Add(files.First());
                            }
                            else
                            {
                                LogHelper.DefaultLog.WriteLine($@"<{GetType().Name}> expect 1 match file");
                                continue;
                            }
                        }
                        else
                        {
                            throw new Exception($@"<{GetType().Name}> expect 1 match directory");
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
                                var outputFileReader = new OutputFileReader(fieldInfo, fileInfo);
                                list.Add(outputFileReader.resultObject);
                            }
                        }
                    }
                    else
                    {
                        if (fileInfos.Count==1)
                        {
                            var outputFileReader = new OutputFileReader(fieldInfo, fileInfos.First());
                            fieldInfo.SetValue(resultObject,outputFileReader.resultObject);
                        }
                        else
                        {
                            throw new Exception($@"<{GetType().Name}> multi match file");
                        }
                    }
                }
                else
                {
                    Debug.Assert(false);
                }
            }

            if (!string.IsNullOrWhiteSpace(_outputPathAttribute.path))
            {
                var propertyInfo = resultObject.GetType().GetProperties().Single(p => p.Name == OutputEnum.pathInfo.ToString());
                var target = propertyInfo.GetValue(resultObject);
                foreach (var subFieldInfo in target.GetType().GetFields())
                {
                    if (subFieldInfo.Name==GlobalEnum.Global.ToString())
                    {
                        var subTarget = subFieldInfo.GetValue(target);
                        foreach (var field in subTarget.GetType().GetFields())
                        {
                            var decodeResult = field.Name.DecodeGlobal(_outputPathAttribute.path,_directoryInfo.FullName.FilterLayerByPath(_outputPathAttribute.path));
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
                        var decodeResult = subFieldInfo.Name.DecodeLocal(_outputPathAttribute.path,_directoryInfo.FullName.FilterLayerByPath(_outputPathAttribute.path));
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
