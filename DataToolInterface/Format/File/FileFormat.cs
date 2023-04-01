using System;

namespace DataToolInterface.Format.File
{
    public enum FileFormatEnum
    {
        Excel,
        Binary,
        Xml,
        Ini,
        Lua,
        CSharp,
    }

    public enum FileInfoEnum
    {
        fileInfo,
    }
    public abstract class FileFormatAttribute : Attribute
    {
        public string format { get; set; }
    }
}