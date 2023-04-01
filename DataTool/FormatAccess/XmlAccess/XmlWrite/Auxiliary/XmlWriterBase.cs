using System;
using System.Xml;

namespace DataTool.FormatAccess.XmlAccess.XmlWrite.Auxiliary
{
    public class XmlWriterBase
    {
        protected readonly object Object;
        protected readonly XmlElement Element;

        public XmlWriterBase(object paramObject,XmlElement paramElement)
        {
            if (paramObject == null) throw new ArgumentNullException(nameof(paramObject));
            if (paramElement == null) throw new ArgumentNullException(nameof(paramElement));
            
            Object = paramObject;
            Element = paramElement;
        }
    }
}