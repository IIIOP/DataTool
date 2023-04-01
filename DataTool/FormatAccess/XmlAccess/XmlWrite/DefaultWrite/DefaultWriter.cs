using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;
using DataTool.FormatAccess.XmlAccess.XmlWrite.AttributeWrite;
using DataTool.FormatAccess.XmlAccess.XmlWrite.Auxiliary;
using DataToolInterface.Format.File.Xml;

namespace DataTool.FormatAccess.XmlAccess.XmlWrite.DefaultWrite
{
    public class DefaultWriter:XmlWriterBase
    {
        public DefaultWriter(object paramObject, XmlElement paramElement) : base(paramObject, paramElement)
        {
            WriteDefault();
        }

        private void WriteDefault()
        {
            WriteDefaultType(Object,Element);
        }

        private void WriteDefaultType(object paramObject,XmlElement paramElement)
        {
            foreach (var fieldInfo in paramObject.GetType().GetFields())
            {
                if (fieldInfo.Name==XmlFieldNameEnum.attribute.ToString())
                {
                    if (fieldInfo.GetValue(paramObject) is Dictionary<string,string> dictionary)
                    {
                        foreach (var item in dictionary)
                        {
                            paramElement.SetAttribute(item.Key, item.Value);
                        }
                    }
                }
                else if (fieldInfo.Name==XmlFieldNameEnum.content.ToString())
                {
                    paramElement.InnerText = fieldInfo.GetValue(paramObject).ToString();
                }
                else if (fieldInfo.Name==XmlFieldNameEnum.elements.ToString())
                {
                    if (fieldInfo.GetValue(paramObject) is IList list)
                    {
                        foreach (var item in list)
                        {
                            var elementName = item.GetType().GetField(XmlFieldNameEnum.name.ToString()).GetValue(item).ToString();
                            var element = paramElement.OwnerDocument.CreateElement(elementName);
                            paramElement.AppendChild(element);
                            WriteDefaultType(item,element);
                        }
                    }
                }
                else if(fieldInfo.Name!=XmlFieldNameEnum.name.ToString())
                {
                    throw new Exception("Code has bug");
                }
            }
        }
    }
}