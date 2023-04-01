using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using DataTool.CodeGenerate.Auxiliary;
using DataTool.FormatAccess.Auxiliary;
using DataTool.FormatAccess.XmlAccess.XmlRead.Auxiliary;
using DataToolInterface.Data;
using DataToolInterface.Format.File.Xml;

namespace DataTool.FormatAccess.XmlAccess.XmlRead.AttributeRead
{
    public class AttributeReader:XmlReaderBase
    {
        public AttributeReader(Type paramType, XElement paramElement) : base(paramType, paramElement)
        {
            ReadAttribute();
        }
        
        private void ReadAttribute()
        {
            foreach (var fieldInfo in resultObject.GetType().GetFields())
            {
                if (fieldInfo.GetCustomAttribute<XmlAttributeAttribute>() is XmlAttributeAttribute attributeAttribute)
                {
                    var attributeValue = Element.Attribute(fieldInfo.Name);
                    if (attributeValue!=null)
                    {
                        var type = attributeAttribute.type;
                        if (Enum.TryParse(type.BaseType(),out BasicDataTypeEnum dataTypeEnum))
                        {
                            if (type.IsArray())
                            {
                                if (fieldInfo.GetValue(resultObject) is IList list)
                                {
                                    var contents = attributeValue.Value.Split(',');
                                    foreach (var content in contents)
                                    {
                                        try
                                        {
                                            list.Add(dataTypeEnum.ReadBasicDataTypeByString(content));
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine($"config data type not consistent with actual {e}");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                try
                                {
                                    fieldInfo.SetValue(resultObject,dataTypeEnum.ReadBasicDataTypeByString(attributeValue.Value));
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine($"config data type not consistent with actual {e}");
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
    }
}