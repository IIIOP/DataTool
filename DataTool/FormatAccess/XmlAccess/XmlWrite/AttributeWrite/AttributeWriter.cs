using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using DataTool.CodeGenerate.Auxiliary;
using DataTool.FormatAccess.XmlAccess.XmlWrite.Auxiliary;
using DataToolInterface.Format.File.Xml;

namespace DataTool.FormatAccess.XmlAccess.XmlWrite.AttributeWrite
{
    public class AttributeWriter:XmlWriterBase
    {
        public AttributeWriter(object paramObject, XmlElement paramElement) : base(paramObject, paramElement)
        {
            WriteAttribute();
        }

        private void WriteAttribute()
        {
            foreach (var fieldInfo in Object.GetType().GetFields())
            {
                if (fieldInfo.GetCustomAttribute<XmlAttributeAttribute>() is XmlAttributeAttribute attribute)
                {
                    if (attribute.type.IsArray())
                    {
                        if (fieldInfo.GetValue(Object) is IList list)
                        {
                            var stringBuilder = new StringBuilder("");
                            for (int i = 0; i < list.Count; i++)
                            {
                                stringBuilder.Append($@"{list[i]},");
                            }
                            Element.SetAttribute(fieldInfo.Name, stringBuilder.ToString().TrimEnd(','));
                        }
                    }
                    else
                    {
                        Element.SetAttribute(fieldInfo.Name,fieldInfo.GetValue(Object)?.ToString());
                    }
                }
            }
        }
    }
}