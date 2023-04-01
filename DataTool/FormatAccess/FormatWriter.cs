using System;
using System.IO;
using System.Linq;
using System.Reflection;
using DataTool.FormatAccess.Auxiliary;
using DataToolInterface.Format.File;

namespace DataTool.FormatAccess
{
    public abstract class FormatWriter
    {
        protected readonly object Object;
        protected readonly FileInfo FileInfo;
        protected FormatWriter(object paramObject,FileInfo paramFileInfo)
        {
            if (paramObject == null) throw new ArgumentNullException(nameof(paramObject));
            if (paramFileInfo == null) throw new ArgumentNullException(nameof(paramFileInfo));
            
            Object = paramObject;
            FileInfo = paramFileInfo;
        }

        public static FormatWriter GetFormatWriter(FileFormatEnum paramFileFormatEnum,object paramObject,FileInfo paramFileInfo)
        {
            var type = typeof(FormatWriter).Assembly.GetTypes()
                .Where(p => p.IsSubclassOf(typeof(FormatWriter)))
                .SingleOrDefault(p => p.GetCustomAttribute<FormatAccessAttribute>()?.format == paramFileFormatEnum);

            if (type!=null)
            {
                return Activator.CreateInstance(type, paramObject, paramFileInfo) as FormatWriter;
            }
            else
            {
                throw new Exception($@"format [{paramFileFormatEnum}] not support!");
            }
        }
    }
}