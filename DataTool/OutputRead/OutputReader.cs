using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using DataTool.Auxiliary;
using DataTool.OutputRead.OutputPathRead;
using DataToolInterface.Format.Config.Output;
using DataToolLog;

namespace DataTool.OutputRead
{
    public class OutputReader
    {
        private readonly Type _type;
        private readonly DirectoryInfo _directoryInfo;
        public object resultObject { get;private set; }

        public OutputReader(Type paramType,DirectoryInfo paramDirectoryInfo)
        {
            if (paramType == null) throw new ArgumentNullException(nameof(paramType));
            if (paramDirectoryInfo == null) throw new ArgumentNullException(nameof(paramDirectoryInfo));
            
            if (paramDirectoryInfo.Exists)
            {
                _type = paramType;
                _directoryInfo = paramDirectoryInfo;
                
                ReadOutput();
            }
            else
            {
                throw new Exception($@"<{GetType().Name}> Directory[{paramDirectoryInfo.FullName}] not exist");
            }
        }
        
        private void ReadOutput()
        {
            LogHelper.DefaultLog.WriteLine($@"[{GetType().Name}] Read Output [{_directoryInfo.FullName}]");
            
            resultObject = Activator.CreateInstance(_type);
            foreach (var fieldInfo in resultObject.GetType().GetFields())
            {
                if (fieldInfo.FieldType.GetCustomAttribute<OutputPathAttribute>() is OutputPathAttribute outputPathAttribute)
                {
                    DirectoryInfo directoryInfo;
                    if (string.IsNullOrWhiteSpace(outputPathAttribute.path))
                    {
                        directoryInfo = _directoryInfo;
                    }
                    else
                    {
                        outputPathAttribute.path = outputPathAttribute.path.FormatToPath();
                        var parent = new DirectoryInfo(Path.Combine(_directoryInfo.FullName,outputPathAttribute.path.GetPathBeforeVariable()));
                        if (parent.Exists)
                        {
                            var regex = new Regex($@"^({Regex.Escape(_directoryInfo.FullName.FormatToPath())})?{outputPathAttribute.path.ReplaceVariableToRegex().TrimEnd('\\')}$");
                            var directoryInfos = parent.GetDirectories("*", SearchOption.AllDirectories).Append(parent).Where(item => regex.IsMatch(item.FullName)).ToArray();
                            if (directoryInfos.Length==1)
                            {
                                directoryInfo = directoryInfos.First();
                            }
                            else
                            {
                                throw new Exception($@"<{GetType().Name}> There are {directoryInfos.Length} match Item");
                            }
                        }
                        else
                        {
                            throw new Exception($@"<{GetType().Name}> parent Directory[{parent.FullName}] not exist");
                        }
                    }
                    var outputPathReader = new OutputPathReader(fieldInfo.FieldType,directoryInfo);
                    fieldInfo.SetValue(resultObject,outputPathReader.resultObject);
                }
                else
                {
                    Debug.Assert(false);
                }
            }
        }
    }
}