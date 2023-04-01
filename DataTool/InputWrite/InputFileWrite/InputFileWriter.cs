using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using DataTool.FormatAccess;
using DataTool.FormatAccess.Auxiliary;
using DataToolInterface.Format.Config.Input;
using DataToolInterface.Format.Config.Output;
using DataToolInterface.Format.File;
using DataToolLog;

namespace DataTool.InputWrite.InputFileWrite
{
    public class InputFileWriter
    {
        private readonly object _object;
        private readonly FieldInfo _fieldInfo;
        private readonly FileInfo _fileInfo;

        public InputFileWriter(object paramObject,FieldInfo paramFieldInfo,FileInfo paramFileInfo)
        {
            if (paramObject == null) throw new ArgumentNullException(nameof(paramObject));
            if (paramFieldInfo == null) throw new ArgumentNullException(nameof(paramFieldInfo));
            if (paramFileInfo == null) throw new ArgumentNullException(nameof(paramFileInfo));

            if (paramFileInfo.Exists)
            {
                paramFileInfo.Delete();
            }
            _object = paramObject;
            _fieldInfo = paramFieldInfo;
            _fileInfo = paramFileInfo;

            WriteInputFile();
        }

        private void WriteInputFile()
        {
            LogHelper.DefaultLog.WriteLine($@"[{GetType().Name}] Write Input File [{_fileInfo.FullName}]");
            
            if (_fieldInfo.GetCustomAttribute<InputFileAttribute>() is InputFileAttribute inputFileAttribute)
            {
                if (Enum.TryParse(inputFileAttribute.format,out FileFormatEnum format))
                {
                    var formatWriter = FormatWriter.GetFormatWriter(format, _object, _fileInfo);
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