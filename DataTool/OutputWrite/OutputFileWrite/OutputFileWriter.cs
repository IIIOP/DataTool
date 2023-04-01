using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using DataTool.FormatAccess;
using DataTool.FormatAccess.Auxiliary;
using DataToolInterface.Format.Config.Output;
using DataToolInterface.Format.File;
using DataToolLog;

namespace DataTool.OutputWrite.OutputFileWrite
{
    public class OutputFileWriter
    {
        private readonly object _object;
        private readonly FieldInfo _fieldInfo;
        private readonly FileInfo _fileInfo;
        
        public OutputFileWriter(object paramObject,FieldInfo paramFieldInfo,FileInfo paramFileInfo)
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

            WriteOutputFile();
        }

        private void WriteOutputFile()
        {
            LogHelper.DefaultLog.WriteLine($@"[{GetType().Name}] Write Output File [{_fileInfo.FullName}]");
            
            if (_fieldInfo.GetCustomAttribute<OutputFileAttribute>() is OutputFileAttribute outputFileAttribute)
            {
                if (Enum.TryParse(outputFileAttribute.format,out FileFormatEnum format))
                {
                    var formatWriter = FormatWriter.GetFormatWriter(format, _object, _fileInfo);
                }
                else
                {
                    throw new Exception($@"[Error] 读取的输入文件的格式不支持 [{outputFileAttribute.format}][{_fileInfo.FullName}]");
                }
            }
            else
            {
                Debug.Assert(false);
            }
        }
    }
}