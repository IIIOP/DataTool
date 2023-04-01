using System;
using DataToolInterface.Auxiliary;

namespace DataToolInterface.Format.Config.Model
{
    public enum ModelEnum
    {
        Model,
    }

    public enum ModelElementEnum
    {
        Enum,
        Struct,
    }
    
    public class ModelEnumElementItemAttribute:Attribute
    {
        public string value { get; set; }
        public string describe { get; set; }
    }
    
    public class ModelStructElementAttribute:Attribute
    {
        [Require]
        public string format { get; set; }
        [Require]
        public string subFormat { get; set; }
    }
}