using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using DataTool.CodeGenerate.Auxiliary;
using DataToolInterface.Data;
using DataToolInterface.Format.Config.Model;
using DataToolInterface.Format.File.Binary;
using DataToolInterface.Format.File.Excel;

namespace DataTool.FormatAccess.BinaryAccess.Auxiliary
{
    public static class ExtentMethod
    {
        private enum DescribeAttributeEnum
        {
            Type,
            Enum,
            EnumDescribe,
            Count,
            Hex,
            Dec,
            Value,
            Content,
            Describe,
        }
        public static Int16 Reverse(this Int16 param)
        {
            return (Int16) ((param & 0xFFU) << 8 | (param & 0xFF00U) >> 8);
        }
        public static UInt16 Reverse(this UInt16 param)
        {
            return (UInt16) ((param & 0xFFU) << 8 | (param & 0xFF00U) >> 8);
        }
        public static Int32 Reverse(this Int32 param)
        {
            return (Int32) ((param & 0x000000FFU) << 24 |
                            (param & 0x0000FF00U) << 8 | 
                            (param & 0x00FF0000U) >> 8 |
                            (param & 0xFF000000U) >> 24);
        }
        public static UInt32 Reverse(this UInt32 param)
        {
            // ReSharper disable once RedundantCast
            return (UInt32) ((param & 0x000000FFU) << 24 |
                             (param & 0x0000FF00U) << 8 | 
                             (param & 0x00FF0000U) >> 8 |
                             (param & 0xFF000000U) >> 24);
        }
        public static UInt64 Reverse(this UInt64 param)
        {
            // ReSharper disable once BuiltInTypeReferenceStyle
            // ReSharper disable once RedundantCast
            return (UInt64) ((param & 0x00000000000000FFU) << 56 | 
                             (param & 0x000000000000FF00U) << 40 | 
                             (param & 0x0000000000FF0000U) << 24 |
                             (param & 0x00000000FF000000U) << 8 |
                             (param & 0x000000FF00000000U) >> 8 |
                             (param & 0x0000FF0000000000U) >> 24 |
                             (param & 0x00FF000000000000U) >> 40 |
                             (param & 0xFF00000000000000U) >> 56);
        }

        public static void GenerateDescribeFile(object paramObject,FileInfo paramFileInfo)
        {
            if (paramObject == null) throw new ArgumentNullException(nameof(paramObject));
            if (paramFileInfo == null) throw new ArgumentNullException(nameof(paramFileInfo));
            if (paramObject.GetType().GetCustomAttribute<BinaryFileAttribute>()!=null)
            {
                var xmlDocument = new XmlDocument();
                XmlNode xmlNode = xmlDocument.CreateXmlDeclaration("1.0", "utf-8", "");
                xmlDocument.AppendChild(xmlNode);
                var root = xmlDocument.CreateElement(paramObject.GetType().Name);
                xmlDocument.AppendChild(root);
                GenerateDescribeFileContent(paramObject,root);
                xmlDocument.Save(paramFileInfo.FullName);
            }
            else
            {
                throw new Exception();
            }
        }

        private static void GenerateDescribeFileContent(object paramObject,XmlElement paramXmlElement)
        {
            foreach (var fieldInfo in paramObject.GetType().GetFields())
            {
                var node = paramXmlElement.OwnerDocument.CreateElement(fieldInfo.Name);
                paramXmlElement.AppendChild(node);

                if (fieldInfo.GetCustomAttribute<BinaryBasicSingleAttribute>() is BinaryBasicSingleAttribute basicSingleAttribute)
                {
                    node.SetAttribute(DescribeAttributeEnum.Type.ToString(), basicSingleAttribute.type);
                    if (basicSingleAttribute.range!=null)
                    {
                        var regex = new Regex(@"^\w+$");
                        if (regex.IsMatch(basicSingleAttribute.range))
                        {
                            var enumType = paramObject.GetType().Assembly.GetTypes().SingleOrDefault(item => item.Name == basicSingleAttribute.range);
                            if (enumType != null && enumType.IsEnum)
                            {
                                try
                                {
                                    var enumValue = Enum.Parse(enumType, fieldInfo.GetValue(paramObject).ToString()).ToString();
                                    node.SetAttribute(DescribeAttributeEnum.Enum.ToString(),enumValue);
                                    var field = enumType.GetFields().FirstOrDefault(item => item.Name == enumValue);
                                    if (field?.GetCustomAttribute<ModelEnumElementItemAttribute>() is ModelEnumElementItemAttribute attribute&&attribute.describe!=null)
                                    {
                                        node.SetAttribute(DescribeAttributeEnum.EnumDescribe.ToString(),attribute.describe);
                                    }
                                }
                                catch (Exception e)
                                {
                                    // ignored
                                }
                            }
                        }
                    }
                    GenerateBasicType((BasicDataTypeEnum) Enum.Parse(typeof(BasicDataTypeEnum),basicSingleAttribute.type),fieldInfo.GetValue(paramObject),node);
        
                    if (basicSingleAttribute.describe!=null)
                    {
                        node.SetAttribute(DescribeAttributeEnum.Describe.ToString(), basicSingleAttribute.describe);
                    }
                }
                else if (fieldInfo.GetCustomAttribute<BinaryBasicMultipleAttribute>() is BinaryBasicMultipleAttribute basicMultipleAttribute)
                {
                    if (fieldInfo.GetValue(paramObject) is IList list)
                    {
                        node.SetAttribute(DescribeAttributeEnum.Type.ToString(), basicMultipleAttribute.type);
                        
                        for (int index = 0; index < list.Count; index++)
                        {
                            var subNode = node.OwnerDocument.CreateElement($@"item_{index}");
                            node.AppendChild(subNode);
                            subNode.SetAttribute(DescribeAttributeEnum.Type.ToString(), basicMultipleAttribute.type.TrimEnd('[',']'));
                            
                            if (basicMultipleAttribute.range!=null)
                            {
                                var regex = new Regex(@"^\w+$");
                                if (regex.IsMatch(basicMultipleAttribute.range))
                                {
                                    var enumType = paramObject.GetType().Assembly.GetTypes()
                                        .SingleOrDefault(item => item.Name == basicMultipleAttribute.range);
                                    if (enumType != null && enumType.IsEnum)
                                    {
                                        var enumValue = Enum.Parse(enumType, list[index].ToString()).ToString();
                                        subNode.SetAttribute(DescribeAttributeEnum.Enum.ToString(),enumValue);
                                        var field = enumType.GetFields().First(item => item.Name == enumValue);
                                        if (field.GetCustomAttribute<ModelEnumElementItemAttribute>() is ModelEnumElementItemAttribute attribute&&attribute.describe!=null)
                                        {
                                            subNode.SetAttribute(DescribeAttributeEnum.EnumDescribe.ToString(),attribute.describe);
                                        }
                                    }
                                }
                            }
                            GenerateBasicType((BasicDataTypeEnum) Enum.Parse(typeof(BasicDataTypeEnum),basicMultipleAttribute.type.BaseType()),list[index],subNode);
                            if (basicMultipleAttribute.describe!=null)
                            {
                                node.SetAttribute(DescribeAttributeEnum.Describe.ToString(), basicMultipleAttribute.describe);
                            }
                        }
                        node.SetAttribute(DescribeAttributeEnum.Count.ToString(), list.Count.ToString());
                    }
                }
                else if (fieldInfo.GetCustomAttribute<BinaryAdvancedSingleAttribute>() is BinaryAdvancedSingleAttribute advancedSingleAttribute)
                {
                    node.SetAttribute(DescribeAttributeEnum.Type.ToString(), advancedSingleAttribute.type);
                    if (advancedSingleAttribute.describe!=null)
                    {
                        node.SetAttribute(DescribeAttributeEnum.Describe.ToString(), advancedSingleAttribute.describe);
                    }
                    GenerateDescribeFileContent(fieldInfo.GetValue(paramObject),node);
                }
                else if (fieldInfo.GetCustomAttribute<BinaryAdvancedMultipleAttribute>() is BinaryAdvancedMultipleAttribute advancedMultipleAttribute)
                {
                    node.SetAttribute(DescribeAttributeEnum.Type.ToString(), advancedMultipleAttribute.type);
                    if (fieldInfo.GetValue(paramObject) is IList list)
                    {
                        for (int index = 0; index < list.Count; index++)
                        {
                            var subNode = node.OwnerDocument.CreateElement($@"item_{index}");
                            node.AppendChild(subNode);
                            subNode.SetAttribute(DescribeAttributeEnum.Type.ToString(), advancedMultipleAttribute.type.TrimEnd('[',']'));
                            if (advancedMultipleAttribute.describe!=null)
                            {
                                node.SetAttribute(DescribeAttributeEnum.Describe.ToString(), advancedMultipleAttribute.describe);
                            }
                            GenerateDescribeFileContent(list[index],subNode);
                        }
                        node.SetAttribute(DescribeAttributeEnum.Count.ToString(), list.Count.ToString());
                    }
                }
            }
        }

        private static void GenerateBasicType(BasicDataTypeEnum paramBasicDataType,object paramObject,XmlElement paramXmlElement)
        {
            switch (paramBasicDataType)
            {
                case BasicDataTypeEnum.INT8:
                    paramXmlElement.SetAttribute(DescribeAttributeEnum.Hex.ToString(), $"{paramObject:x2}");
                    paramXmlElement.SetAttribute(DescribeAttributeEnum.Dec.ToString(), $"{paramObject}");
                    break;
                case BasicDataTypeEnum.INT16:
                    paramXmlElement.SetAttribute(DescribeAttributeEnum.Hex.ToString(), $"{paramObject:x4}");
                    paramXmlElement.SetAttribute(DescribeAttributeEnum.Dec.ToString(), $"{paramObject}");
                    break;
                case BasicDataTypeEnum.INT32:
                    paramXmlElement.SetAttribute(DescribeAttributeEnum.Hex.ToString(), $"{paramObject:x8}");
                    paramXmlElement.SetAttribute(DescribeAttributeEnum.Dec.ToString(), $"{paramObject}");
                    break;
                case BasicDataTypeEnum.INT64:
                    paramXmlElement.SetAttribute(DescribeAttributeEnum.Hex.ToString(), $"{paramObject:x16}");
                    paramXmlElement.SetAttribute(DescribeAttributeEnum.Dec.ToString(), $"{paramObject}");
                    break;
                case BasicDataTypeEnum.UINT8:
                    paramXmlElement.SetAttribute(DescribeAttributeEnum.Hex.ToString(), $"{paramObject:x2}");
                    paramXmlElement.SetAttribute(DescribeAttributeEnum.Dec.ToString(), $"{paramObject}");
                    break;
                case BasicDataTypeEnum.UINT16:
                    paramXmlElement.SetAttribute(DescribeAttributeEnum.Hex.ToString(), $"{paramObject:x4}");
                    paramXmlElement.SetAttribute(DescribeAttributeEnum.Dec.ToString(), $"{paramObject}");
                    break;
                case BasicDataTypeEnum.UINT32:
                    paramXmlElement.SetAttribute(DescribeAttributeEnum.Hex.ToString(), $"{paramObject:x8}");
                    paramXmlElement.SetAttribute(DescribeAttributeEnum.Dec.ToString(), $"{paramObject}");
                    break;
                case BasicDataTypeEnum.UINT64:
                    paramXmlElement.SetAttribute(DescribeAttributeEnum.Hex.ToString(), $"{paramObject:x16}");
                    paramXmlElement.SetAttribute(DescribeAttributeEnum.Dec.ToString(), $"{paramObject}");
                    break;
                case BasicDataTypeEnum.FLOAT:
                    paramXmlElement.SetAttribute(DescribeAttributeEnum.Value.ToString(), $"{paramObject}");
                    break;
                case BasicDataTypeEnum.DOUBLE:
                    paramXmlElement.SetAttribute(DescribeAttributeEnum.Value.ToString(), $"{paramObject}");
                    break;
                case BasicDataTypeEnum.DECIMAL:
                    paramXmlElement.SetAttribute(DescribeAttributeEnum.Value.ToString(), $"{paramObject}");
                    break;
                case BasicDataTypeEnum.String:
                    paramXmlElement.SetAttribute(DescribeAttributeEnum.Content.ToString(), $"{paramObject}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(paramBasicDataType), paramBasicDataType, null);
            }
        }
    }
}