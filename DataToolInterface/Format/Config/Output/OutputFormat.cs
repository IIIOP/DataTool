using System;
using DataToolInterface.Auxiliary;

namespace DataToolInterface.Format.Config.Output
{
    public enum OutputEnum
    {
        Output,
        pathInfo,
    }
    
    public class OutputAttribute : Attribute
    {
        public string path { get; set; }
    }
    
    public class OutputPathAttribute:Attribute
    {
        public string path { get; set; }
        public string describe { get; set; }
    }
    
    public class OutputFileAttribute:Attribute
    {
        [Require]
        public string type { get; set; }
        [Require]
        public string name { get; set; }
        [Require]
        public string format { get; set; }
        [Require]
        public string path { get; set; }
        public string describe { get; set; }
    }
}