using System;
using DataToolInterface.Auxiliary;

namespace DataToolInterface.Format.File.Xml
{
    public class XmlFileAttribute : FileFormatAttribute
    {
        
    }

    public enum XmlFieldNameEnum
    {
        name,
        attribute,
        content,
        elements,
        root,
    }

    public enum XmlTypeNameEnum
    {
        XmlDefaultType,
    }
    public enum XmlSubFormatEnum
    {
        Xml,
        Content,
    }

    public class XmlAttributeAttribute : Attribute
    {
        [Require]
        public string type { get; set; }
    }
    public class XmlContentSingleAttribute : Attribute
    {
        [Require]
        public string type { get; set; }
    }
    public class XmlContentMultipleAttribute : Attribute
    {
        [Require]
        public string type { get; set; }
    }
    public class XmlElementSingleAttribute:Attribute
    {
        [Require]
        public string type { get; set; }
    }
    public class XmlElementMultipleAttribute:Attribute
    {
        [Require]
        public string type { get; set; }
    }
    public class XmlElementDefaultAttribute:Attribute
    {
        
    }
}