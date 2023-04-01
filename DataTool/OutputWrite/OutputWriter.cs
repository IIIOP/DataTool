using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using DataTool.Auxiliary;
using DataTool.OutputWrite.OutputPathWrite;
using DataToolInterface.Format.Config.Output;
using DataToolLog;

namespace DataTool.OutputWrite
{
    public class OutputWriter
    {
        private readonly object _object;
        private readonly DirectoryInfo _directoryInfo;
        
        public OutputWriter(object paramObject,DirectoryInfo paramDirectoryInfo)
        {
            if (paramObject == null) throw new ArgumentNullException(nameof(paramObject));
            if (paramDirectoryInfo == null) throw new ArgumentNullException(nameof(paramDirectoryInfo));

            if (paramDirectoryInfo.Exists)
            {
                _object = paramObject;
                _directoryInfo = paramDirectoryInfo;
                
                WriteOutput();
            }
            else
            {
                throw new Exception($@"<{GetType().Name}> Directory[{paramDirectoryInfo.FullName}] not exist");
            }
        }

        private void WriteOutput()
        {
            LogHelper.DefaultLog.WriteLine($@"[{GetType().Name}] Start Write Output...");
            
            foreach (var fieldInfo in _object.GetType().GetFields())
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
                        outputPathAttribute.path = outputPathAttribute.path.EncodeVariable(fieldInfo.GetValue(_object));
                        outputPathAttribute.path = outputPathAttribute.path.FormatToPath();
                        directoryInfo = new DirectoryInfo(Path.Combine(_directoryInfo.FullName, outputPathAttribute.path));
                    }

                    if (!directoryInfo.Exists)
                    {
                        directoryInfo.Create();
                        directoryInfo.Refresh();
                    }
                    var outputPathReader = new OutputPathWriter(fieldInfo.GetValue(_object),directoryInfo);
                }
                else
                {
                    Debug.Assert(false);
                }
            }
        }
    }
}