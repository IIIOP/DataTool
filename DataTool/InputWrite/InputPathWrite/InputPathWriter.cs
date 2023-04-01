using System;
using System.Collections;
using System.IO;
using System.Reflection;
using DataTool.Auxiliary;
using DataTool.InputWrite.InputFileWrite;
using DataToolInterface.Format.Config.Input;
using DataToolLog;

namespace DataTool.InputWrite.InputPathWrite
{
    public class InputPathWriter
    {
        private readonly object _object;
        private readonly DirectoryInfo _directoryInfo;
        public InputPathWriter(object paramObject,DirectoryInfo paramDirectoryInfo)
        {
            if (paramObject == null) throw new ArgumentNullException(nameof(paramObject));
            if (paramDirectoryInfo == null) throw new ArgumentNullException(nameof(paramDirectoryInfo));
            
            if (paramDirectoryInfo.Exists)
            {
                _object = paramObject;
                _directoryInfo = paramDirectoryInfo;

                WriteInputPath();
            }
            else
            {
                throw new Exception($@"<{GetType().Name}> Directory[{paramDirectoryInfo.FullName}] not exist");
            }
        }

        private void WriteInputPath()
        {
            LogHelper.DefaultLog.WriteLine($@"[{GetType().Name}] Write Input Path [{_directoryInfo.FullName}]");
            foreach (var fieldInfo in _object.GetType().GetFields())
            {
                if (fieldInfo.GetCustomAttribute<InputFileAttribute>() is InputFileAttribute inputFileAttribute)
                {
                    if (fieldInfo.FieldType.IsGenericType)
                    {
                        if (fieldInfo.GetValue(_object) is IList list)
                        {
                            foreach (var item in list)
                            {
                                DirectoryInfo directoryInfo;
                                if (string.IsNullOrWhiteSpace(inputFileAttribute.path))
                                {
                                    directoryInfo = _directoryInfo;
                                }
                                else
                                {
                                    inputFileAttribute.path = inputFileAttribute.path.EncodeVariable(item);
                                    inputFileAttribute.path = inputFileAttribute.path.FormatToPath();
                        
                                    directoryInfo = new DirectoryInfo(Path.Combine(_directoryInfo.FullName, inputFileAttribute.path));
                                }

                                if (!directoryInfo.Exists)
                                {
                                    directoryInfo.Create();
                                    directoryInfo.Refresh();
                                }
                                inputFileAttribute.name = inputFileAttribute.name.EncodeVariable(item);

                                var outputFileWriter = new InputFileWriter(item,fieldInfo, new FileInfo(Path.Combine(directoryInfo.FullName, "writeInput"+inputFileAttribute.name)));
                            }
                        }
                    }
                    else
                    {
                        if (fieldInfo.GetValue(_object)!=null)
                        {
                            DirectoryInfo directoryInfo;
                            if (string.IsNullOrWhiteSpace(inputFileAttribute.path))
                            {
                                directoryInfo = _directoryInfo;
                            }
                            else
                            {
                                inputFileAttribute.path = inputFileAttribute.path.EncodeVariable(fieldInfo.GetValue(_object));
                                inputFileAttribute.path = inputFileAttribute.path.FormatToPath();
                        
                                directoryInfo = new DirectoryInfo(Path.Combine(_directoryInfo.FullName, inputFileAttribute.path));
                            }
                            if (!directoryInfo.Exists)
                            {
                                directoryInfo.Create();
                                directoryInfo.Refresh();
                            }
                            inputFileAttribute.name = inputFileAttribute.name.EncodeVariable(fieldInfo.GetValue(_object));
                        
                            var outputFileWriter = new InputFileWriter(fieldInfo.GetValue(_object),fieldInfo, new FileInfo(Path.Combine(directoryInfo.FullName,"writeInput"+ inputFileAttribute.name)));
                        }
                    }
                }
            }
        }
    }
}