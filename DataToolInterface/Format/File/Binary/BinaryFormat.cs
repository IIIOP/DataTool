using System;
using DataToolInterface.Auxiliary;

namespace DataToolInterface.Format.File.Binary
{
    public class BinaryFileAttribute : FileFormatAttribute
    {
        [Require]
        public string endian { get; set; }
    }

    public enum EndianEnum
    {
        Big,
        Little,
    }

    public enum BinarySubFormatEnum
    {
        Binary,
        Content,
        Free,//可以按字节取值
    }
    
    public class BinaryBasicSingleAttribute:Attribute
    {
        [Require]
        public string type { get; set; }
        public string format { get; set; }
        public string range { get; set; }
        public string constValue { get; set; }
        public string alternative{ get; set; }
        public string describe { get; set; }
    }
    public class BinaryBasicMultipleAttribute:Attribute
    {
        [Require]
        public string type { get; set; }
        public string format { get; set; }
        public string range { get; set; }
        [Require]
        public string size { get; set; }
        public string describe { get; set; }
    }
    public class BinaryAdvancedSingleAttribute:Attribute
    {
        [Require]
        public string type { get; set; }
        public string alternative{ get; set; }
        public string describe { get; set; }
    }
    public class BinaryAdvancedMultipleAttribute:Attribute
    {
        [Require]
        public string type { get; set; }
        [Require]
        public string size { get; set; }
        public string describe { get; set; }
    }
}