using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DataTool.FormatAccess.XmlAccess.XmlRead.Auxiliary;
using DataToolInterface.Format.File.Xml;

namespace DataTool.FormatAccess.XmlAccess.XmlRead.DefaultRead
{
    public class DefaultReader:XmlReaderBase
    {
        public DefaultReader(Type paramType, XElement paramElement) : base(paramType, paramElement)
        {
            ReadDefault();
        }

        private void ReadDefault()
        {
            resultObject = ReadDefaultType(Type,Element);
        }

        private object ReadDefaultType(Type paramType, XElement paramXElement)
        {
            var result = Activator.CreateInstance(paramType);
            foreach (var fieldInfo in result.GetType().GetFields())
            {
                if (fieldInfo.Name==XmlFieldNameEnum.attribute.ToString())
                {
                    if (fieldInfo.GetValue(result) is Dictionary<string,string> dictionary)
                    {
                        foreach (var xAttribute in paramXElement.Attributes())
                        {
                            dictionary.Add(xAttribute.Name.LocalName,xAttribute.Value);
                        }
                    }
                }
                else if (fieldInfo.Name==XmlFieldNameEnum.content.ToString())
                {
                    fieldInfo.SetValue(result,paramXElement.Value);
                }
                else if (fieldInfo.Name==XmlFieldNameEnum.name.ToString())
                {
                    fieldInfo.SetValue(result,paramXElement.Name.LocalName);
                }
                else if (fieldInfo.Name==XmlFieldNameEnum.elements.ToString())
                {
                    if (fieldInfo.GetValue(result) is IList list)
                    {
                        foreach (var xElement in paramXElement.Elements())
                        {
                            list.Add(ReadDefaultType(fieldInfo.FieldType.GetGenericArguments().First(),xElement));
                        }
                    }
                }
            }
            return result;
        }
    }
}