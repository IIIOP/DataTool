using System;
using DataToolInterface.Auxiliary;

namespace DataToolInterface.Format.File.Ini
{
    public class IniFileAttribute : FileFormatAttribute
    {
        
    }
    
    public enum IniSubFormatEnum
    {
        Ini,
        Section,
    }

    public class IniSectionSingleAttribute : Attribute
    {
        [Require]
        public string type { get; set; }
        public string alternative{ get; set; }
        public string describe { get; set; }
    }
    
    public class IniSectionMultipleAttribute : Attribute
    {
        [Require]
        public string type { get; set; }
        public string pattern{ get; set; }
        public string alternative{ get; set; }
        public string describe { get; set; }
    }
    
    public class IniKeySingleAttribute : Attribute
    {
        [Require]
        public string type { get; set; }
        public string format { get; set; }
        public string range { get; set; }
        public string constValue { get; set; }
        public string alternative{ get; set; }
        public string describe { get; set; }
    }
    
    public class IniKeyMultipleAttribute : Attribute
    {
        [Require]
        public string type { get; set; }
        public string pattern{ get; set; }
        public string format { get; set; }
        public string range { get; set; }
        public string describe { get; set; }
    }
}