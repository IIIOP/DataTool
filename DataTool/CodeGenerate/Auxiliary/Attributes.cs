using System;
using DataToolInterface.Format.File;

namespace DataTool.CodeGenerate.Auxiliary
{
    public class GenerateFileFormatAttribute:Attribute
    {
        public FileFormatEnum format { get; set; }
    }
    
    public class GenerateFileSubFormatAttribute:Attribute
    {
        public FileFormatEnum format { get; set; }
        public string subFormat { get; set; }
    }
}