using System;
using System.IO;
using System.Linq;
using System.Reflection;
using DataTool.FormatAccess.Auxiliary;
using DataToolInterface.Format.File;

namespace DataTool.FormatAccess
{
    public abstract class FormatReader
    {
        protected readonly Type Type;
        protected readonly FileInfo FileInfo;
        
        public object resultObject { get; protected set; }

        protected FormatReader(Type paramType,FileInfo paramFileInfo)
        {
            if (paramType == null) throw new ArgumentNullException(nameof(paramType));
            if (paramFileInfo == null) throw new ArgumentNullException(nameof(paramFileInfo));

            if (paramFileInfo.Exists)
            {
                Type = paramType;
                FileInfo = paramFileInfo;
            
                resultObject = Activator.CreateInstance(Type);
            }
            else
            {
                throw new Exception("file not exist");
            }
        }
        
        public static FormatReader GetFormatReader(FileFormatEnum paramFileFormatEnum,Type paramType,FileInfo paramFileInfo)
        {
            var type = typeof(FormatReader).Assembly.GetTypes()
                .Where(p => p.IsSubclassOf(typeof(FormatReader)))
                .SingleOrDefault(p => p.GetCustomAttribute<FormatAccessAttribute>()?.format == paramFileFormatEnum);

            if (type!=null)
            {
                return Activator.CreateInstance(type, paramType, paramFileInfo) as FormatReader;
            }
            else
            {
                throw new Exception($@"format [{paramFileFormatEnum}] not support!");
            }
        }
    }
}