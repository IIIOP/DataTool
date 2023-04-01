using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using DataTool.Auxiliary;
using DataTool.InputWrite.InputPathWrite;
using DataTool.OutputWrite.OutputPathWrite;
using DataToolInterface.Format.Config.Input;

namespace DataTool.InputWrite
{
    public class InputWriter
    {
        private readonly object _object;
        private readonly DirectoryInfo _directoryInfo;
        public InputWriter(object paramObject,DirectoryInfo paramDirectoryInfo)
        {
            if (paramObject == null) throw new ArgumentNullException(nameof(paramObject));
            if (paramDirectoryInfo == null) throw new ArgumentNullException(nameof(paramDirectoryInfo));
            
            if (paramDirectoryInfo.Exists)
            {
                _object = paramObject;
                _directoryInfo = paramDirectoryInfo;
                
                WriteInput();
            }
            else
            {
                throw new Exception($@"<{GetType().Name}> Directory[{paramDirectoryInfo.FullName}] not exist");
            }
        }

        private void WriteInput()
        {
            DataToolLog.LogHelper.DefaultLog.WriteLine($@"[{GetType().Name}] Start Write Input...");
            foreach (var fieldInfo in _object.GetType().GetFields())
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
                        inputPathAttribute.path = inputPathAttribute.path.EncodeVariable(fieldInfo.GetValue(_object));
                        inputPathAttribute.path = inputPathAttribute.path.FormatToPath();
                        directoryInfo = new DirectoryInfo(Path.Combine(_directoryInfo.FullName, inputPathAttribute.path));
                    }

                    if (!directoryInfo.Exists)
                    {
                        directoryInfo.Create();
                        directoryInfo.Refresh();
                    }
                    var inputPathWriter = new InputPathWriter(fieldInfo.GetValue(_object),directoryInfo);
                }
                else
                {
                    Debug.Assert(false);
                }
            }
        }
    }
}