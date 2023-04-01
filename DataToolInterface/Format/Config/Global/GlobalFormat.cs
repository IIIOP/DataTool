using System;
using DataToolInterface.Auxiliary;

namespace DataToolInterface.Format.Config.Global
{
    public enum GlobalEnum
    {
        Global,
    }
    
    public class GlobalAttribute : Attribute
    {
        [Require]
        public string type { get; set; }
        public string initValue { get; set; }
        public string isRequire { get; set; }
        public string describe { get; set; }
    }
}