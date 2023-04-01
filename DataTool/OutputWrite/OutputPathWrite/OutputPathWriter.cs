using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using DataTool.Auxiliary;
using DataTool.OutputWrite.OutputFileWrite;
using DataToolInterface.Format.Config.Output;

namespace DataTool.OutputWrite.OutputPathWrite
{
    public class OutputPathWriter
    {
        private readonly object _object;
        private readonly DirectoryInfo _directoryInfo;
        
        public OutputPathWriter(object paramObject,DirectoryInfo paramDirectoryInfo)
        {
            if (paramObject == null) throw new ArgumentNullException(nameof(paramObject));
            if (paramDirectoryInfo == null) throw new ArgumentNullException(nameof(paramDirectoryInfo));

            if (paramDirectoryInfo.Exists)
            {
                _object = paramObject;
                _directoryInfo = paramDirectoryInfo;

                WriteOutputPath();
            }
            else
            {
                throw new Exception($@"<{GetType().Name}> Directory[{paramDirectoryInfo.FullName}] not exist");
            }
        }

        private void WriteOutputPath()
        {
            DataToolLog.LogHelper.DefaultLog.WriteLine($@"[{GetType().Name}] Write Output Path [{_directoryInfo.FullName}]");
            
            foreach (var fieldInfo in _object.GetType().GetFields())
            {
                if (fieldInfo.GetCustomAttribute<OutputFileAttribute>() is OutputFileAttribute outputFileAttribute)
                {
                    if (fieldInfo.FieldType.IsGenericType)
                    {
                        if (fieldInfo.GetValue(_object) is IList list)
                        {
                            foreach (var item in list)
                            {
                                DirectoryInfo directoryInfo;
                                if (string.IsNullOrWhiteSpace(outputFileAttribute.path))
                                {
                                    directoryInfo = _directoryInfo;
                                }
                                else
                                {
                                    outputFileAttribute.path = outputFileAttribute.path.EncodeVariable(item);
                                    outputFileAttribute.path = outputFileAttribute.path.FormatToPath();
                        
                                    directoryInfo = new DirectoryInfo(Path.Combine(_directoryInfo.FullName, outputFileAttribute.path));
                                }

                                if (!directoryInfo.Exists)
                                {
                                    directoryInfo.Create();
                                    directoryInfo.Refresh();
                                }
                                outputFileAttribute.name = outputFileAttribute.name.EncodeVariable(item);

                                var outputFileWriter = new OutputFileWriter(item,fieldInfo, new FileInfo(Path.Combine(directoryInfo.FullName, outputFileAttribute.name)));
                            }
                        }
                    }
                    else
                    {
                        if (fieldInfo.GetValue(_object)!=null)
                        {
                            DirectoryInfo directoryInfo;
                            if (string.IsNullOrWhiteSpace(outputFileAttribute.path))
                            {
                                directoryInfo = _directoryInfo;
                            }
                            else
                            {
                                outputFileAttribute.path = outputFileAttribute.path.EncodeVariable(fieldInfo.GetValue(_object));
                                outputFileAttribute.path = outputFileAttribute.path.FormatToPath();
                        
                                directoryInfo = new DirectoryInfo(Path.Combine(_directoryInfo.FullName, outputFileAttribute.path));
                            }
                            if (!directoryInfo.Exists)
                            {
                                directoryInfo.Create();
                                directoryInfo.Refresh();
                            }
                            outputFileAttribute.name = outputFileAttribute.name.EncodeVariable(fieldInfo.GetValue(_object));
                        
                            var outputFileWriter = new OutputFileWriter(fieldInfo.GetValue(_object),fieldInfo, new FileInfo(Path.Combine(directoryInfo.FullName, outputFileAttribute.name)));
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