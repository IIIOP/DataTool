using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using DataTool.Auxiliary;
using DataTool.InputRead.InputPathRead;
using DataTool.OutputRead.OutputPathRead;
using DataToolInterface.Format.Config.Input;
using DataToolLog;

namespace DataTool.InputRead
{
    public class InputReader
    {
        private readonly Type _type;
        private readonly DirectoryInfo _directoryInfo;

        public object resultObject { get; set; }

        public InputReader(Type paramType,DirectoryInfo paramDirectoryInfo)
        {
            if (paramType == null) throw new ArgumentNullException(nameof(paramType));
            if (paramDirectoryInfo == null) throw new ArgumentNullException(nameof(paramDirectoryInfo));

            if (paramDirectoryInfo.Exists)
            {
                _type = paramType;
                _directoryInfo = paramDirectoryInfo;
            
                ReadInput();
            }
            else
            {
                throw new Exception($@"[Error]############# 输入文件的路径不存在 [{paramDirectoryInfo.FullName}]");
            }
        }


        private void ReadInput()
        {
            LogHelper.DefaultLog.WriteLine($@"[{GetType().Name}] Read Input [{_directoryInfo.FullName}]");
            
            resultObject = Activator.CreateInstance(_type);
            foreach (var fieldInfo in resultObject.GetType().GetFields())
            {
                if (fieldInfo.FieldType.GetCustomAttribute<InputPathAttribute>() is InputPathAttribute inputPathAttribute)
                {
                    DirectoryInfo directoryInfo;
                    if (string.IsNullOrWhiteSpace(inputPathAttribute.path))
                    {
                        directoryInfo = _directoryInfo;
                    }
                    else
                    {
                        inputPathAttribute.path = inputPathAttribute.path.FormatToPath();
                        var parent = new DirectoryInfo(Path.Combine(_directoryInfo.FullName,inputPathAttribute.path.GetPathBeforeVariable()));
                        if (parent.Exists)
                        {
                            var regex = new Regex($@"^({Regex.Escape(_directoryInfo.FullName.FormatToPath())})?{inputPathAttribute.path.ReplaceVariableToRegex().TrimEnd('\\')}$");
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
                    var inputPathReader = new InputPathReader(fieldInfo.FieldType,directoryInfo);
                    fieldInfo.SetValue(resultObject,inputPathReader.resultObject);
                }
                else
                {
                    Debug.Assert(false);
                }
            }
        }
    }
}