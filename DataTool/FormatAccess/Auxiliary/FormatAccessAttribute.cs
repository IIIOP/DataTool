using System;
using DataToolInterface.Format.File;

namespace DataTool.FormatAccess.Auxiliary
{
    public class FormatAccessAttribute:Attribute
    {
        public FileFormatEnum format { get; set; }
    }
}