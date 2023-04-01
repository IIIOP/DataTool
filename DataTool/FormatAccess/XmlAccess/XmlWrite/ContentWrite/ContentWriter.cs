using System;
using System.Collections;
using System.Reflection;
using System.Text;
using System.Xml;
using DataTool.FormatAccess.XmlAccess.XmlWrite.AttributeWrite;
using DataTool.FormatAccess.XmlAccess.XmlWrite.Auxiliary;
using DataTool.FormatAccess.XmlAccess.XmlWrite.DefaultWrite;
using DataToolInterface.Format.File.Xml;

namespace DataTool.FormatAccess.XmlAccess.XmlWrite.ContentWrite
{
    public class ContentWriter:XmlWriterBase
    {
        public ContentWriter(object paramObject, XmlElement paramElement) : base(paramObject, paramElement)
        {
            WriteContent();
        }
        private void WriteContent()
        {
            foreach (var fieldInfo in Object.GetType().GetFields())
            {
                if(fieldInfo.GetCustomAttribute<XmlAttributeAttribute>() is XmlAttributeAttribute _)
                {
                    var _ = new AttributeWriter(fieldInfo.GetValue(Object), Element);
                }
                else if(fieldInfo.GetCustomAttribute<XmlContentSingleAttribute>() is XmlContentSingleAttribute _)
                {
                    Element.InnerText = fieldInfo.GetValue(Object).ToString();
                }
                else if (fieldInfo.GetCustomAttribute<XmlContentMultipleAttribute>() is XmlContentMultipleAttribute _)
                {
                    if (fieldInfo.GetValue(Object) is IList list)
                    {
                        var stringBuilder = new StringBuilder("");
                        foreach (var t in list)
                        {
                            stringBuilder.Append($@"{t},");
                        }
                        Element.InnerText = stringBuilder.ToString().TrimEnd(',');
                    }
                }
                else if (fieldInfo.GetCustomAttribute<XmlElementSingleAttribute>() is XmlElementSingleAttribute _)
                {
                    var element = Element.OwnerDocument.CreateElement(fieldInfo.Name);
                    var _ = new ContentWriter(fieldInfo.GetValue(Object), element);
                    Element.AppendChild(element);
                }
                else if (fieldInfo.GetCustomAttribute<XmlElementMultipleAttribute>() is XmlElementMultipleAttribute _)
                {
                    if (fieldInfo.GetValue(Object) is IList list)
                    {
                        foreach (var item in list)
                        {
                            var element = Element.OwnerDocument.CreateElement(fieldInfo.Name);
                            var _ = new ContentWriter(item, element);
                            Element.AppendChild(element);
                        }
                    }
                }
                else if (fieldInfo.GetCustomAttribute<XmlElementDefaultAttribute>() is XmlElementDefaultAttribute elementDefault)
                {
                    if (fieldInfo.GetValue(Object) is IList list)
                    {
                        foreach (var item in list)
                        {
                            var element = Element.OwnerDocument.CreateElement(item.GetType().GetField(XmlFieldNameEnum.name.ToString()).GetValue(item).ToString());
                            var _ = new DefaultWriter(item, element);
                            Element.AppendChild(element);
                        }
                    }
                }
                else
                {
                    throw new Exception("code has bug");
                }
            }
        }
    }
}