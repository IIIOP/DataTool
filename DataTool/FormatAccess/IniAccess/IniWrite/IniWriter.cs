using System.Collections;
using System.IO;
using System.Linq;
using DataTool.FormatAccess.Auxiliary;
using DataTool.FormatAccess.IniAccess.Auxiliary;
using DataToolInterface.Format.File;

namespace DataTool.FormatAccess.IniAccess.IniWrite
{
    [FormatAccess(format = FileFormatEnum.Ini)]
    public class IniWriter:FormatWriter
    {

        public IniWriter(object paramObject,FileInfo paramFileInfo):base(paramObject,paramFileInfo)
        {
            WriteFormat();
        }

        private void WriteFormat()
        {
            
            var iniWriter = new IniAccessor(FileInfo.FullName);

            GenerateIni(Object,iniWriter);
        }

        private void GenerateIni(object paramObject, IniAccessor paramIniAccessor)
        {
            foreach (var fieldInfo in paramObject.GetType().GetFields())
            {
                var temp = fieldInfo.GetValue(paramObject);
                if (temp is IList list)
                {
                    for (var i = 0; i < list.Count; i++)
                    {
                        foreach (var field in temp.GetType().GetGenericArguments().First().GetFields())
                        {
                            paramIniAccessor.WriteValue($@"{fieldInfo.Name}_{i}",field.Name,field.GetValue(list[i])?.ToString());
                        }
                    }
                }
                else
                {
                    foreach (var field in temp.GetType().GetFields())
                    {
                        paramIniAccessor.WriteValue(fieldInfo.Name,field.Name,field.GetValue(temp)?.ToString());
                    }
                }
            }
        }
    }
}