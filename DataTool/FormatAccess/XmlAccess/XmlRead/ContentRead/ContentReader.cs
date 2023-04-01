using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using DataTool.FormatAccess.Auxiliary;
using DataTool.FormatAccess.XmlAccess.XmlRead.AttributeRead;
using DataTool.FormatAccess.XmlAccess.XmlRead.Auxiliary;
using DataTool.FormatAccess.XmlAccess.XmlRead.DefaultRead;
using DataToolInterface.Data;
using DataToolInterface.Format.File.Xml;
using DataToolLog;

namespace DataTool.FormatAccess.XmlAccess.XmlRead.ContentRead
{
    public class ContentReader:XmlReaderBase
    {
        public ContentReader(Type paramType, XElement paramElement) : base(paramType, paramElement)
        {
            ReadContent();
        }
        
        private void ReadContent()
        {
            foreach (var fieldInfo in resultObject.GetType().GetFields())
            {
                if (fieldInfo.GetCustomAttribute<XmlAttributeAttribute>() is XmlAttributeAttribute _)
                {
                    fieldInfo.SetValue(resultObject,new AttributeReader(fieldInfo.FieldType,Element).resultObject);
                }
                else if (fieldInfo.GetCustomAttribute<XmlContentSingleAttribute>() is XmlContentSingleAttribute contentSingle)
                {
                    if (Enum.TryParse(contentSingle.type,out BasicDataTypeEnum basicDataTypeEnum))
                    {
                        try
                        {
                            fieldInfo.SetValue(resultObject,basicDataTypeEnum.ReadBasicDataTypeByString(Element.Value));
                        }
                        catch (Exception e)
                        {
                            LogHelper.DefaultLog.WriteLine($@"The data type in config is not consistent with actual fail!!{e}");
                        }
                    }
                    else
                    {
                        throw new Exception("代码有bug");
                    }
                }
                else if (fieldInfo.GetCustomAttribute<XmlContentMultipleAttribute>() is XmlContentMultipleAttribute contentMultiple)
                {
                    if (Enum.TryParse(contentMultiple.type,out BasicDataTypeEnum basicDataTypeEnum))
                    {
                        if (fieldInfo.GetValue(resultObject) is IList list)
                        {
                            var contents = Element.Value.Split(',');
                            foreach (var content in contents)
                            {
                                try
                                {
                                    list.Add(basicDataTypeEnum.ReadBasicDataTypeByString(Element.Value));
                                }
                                catch (Exception e)
                                {
                                    LogHelper.DefaultLog.WriteLine($@"The data type in config is not consistent with actual fail!!{e}");
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("代码有bug");
                    }
                }
                else if (fieldInfo.GetCustomAttribute<XmlElementSingleAttribute>() is XmlElementSingleAttribute elementSingle)
                {
                    if (Element.Element(fieldInfo.Name) is XElement xElement)
                    {
                        fieldInfo.SetValue(resultObject,new ContentReader(fieldInfo.FieldType,xElement).resultObject);
                    }
                    else
                    {
                        fieldInfo.SetValue(resultObject,null);
                    }
                }
                else if (fieldInfo.GetCustomAttribute<XmlElementMultipleAttribute>() is XmlElementMultipleAttribute _)
                {
                    if (fieldInfo.GetValue(resultObject) is IList list)
                    {
                        foreach (var xElement in Element.Elements(fieldInfo.Name))
                        {
                            var contentReader = new ContentReader(fieldInfo.FieldType.GetGenericArguments().First(), xElement);
                            list.Add(contentReader.resultObject);
                        }
                    }
                }
                else if (fieldInfo.GetCustomAttribute<XmlElementDefaultAttribute>() is XmlElementDefaultAttribute _)
                {
                    if (fieldInfo.GetValue(resultObject) is IList list)
                    {
                        foreach (var xElement in Element.Elements())
                        {
                            list.Add(new DefaultReader(fieldInfo.FieldType.GetGenericArguments().First(), xElement)
                                .resultObject);
                        }
                    }
                }
            }
        }
    }
}