using System;
using DataToolInterface.Auxiliary;

namespace DataToolInterface.Format.Config.Input
{
    public enum InputEnum
    {
        Input,
        pathInfo,
    }
    public class InputAttribute : Attribute
    {
        public string path { get; set; }
    }
    
    public class InputPathAttribute:Attribute
    {
        public string path { get; set; }
        public string describe { get; set; }
    }
    
    public class InputFileAttribute:Attribute
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