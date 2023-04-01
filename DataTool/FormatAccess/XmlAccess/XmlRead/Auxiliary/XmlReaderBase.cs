using System;
using System.Xml.Linq;

namespace DataTool.FormatAccess.XmlAccess.XmlRead.Auxiliary
{
    public class XmlReaderBase
    {
        protected readonly Type Type;
        protected readonly XElement Element;
        
        public object resultObject { get; protected set; }

        protected XmlReaderBase(Type paramType,XElement paramElement)
        {
            if (paramType == null) throw new ArgumentNullException(nameof(paramType));
            if (paramElement == null) throw new ArgumentNullException(nameof(paramElement));
            
            Type = paramType;
            Element = paramElement;
            
            resultObject = Activator.CreateInstance(Type);
        }
    }
}