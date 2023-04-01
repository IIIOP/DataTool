using System;
using System.CodeDom;
using System.Reflection;
using System.Xml.Linq;
using DataTool.CodeGenerate.Auxiliary;
using DataTool.CodeGenerate.ConfigGenerate.Auxiliary;
using DataToolInterface.Data;
using DataToolInterface.Format.Config.Global;

namespace DataTool.CodeGenerate.ConfigGenerate.GlobalGenerate
{
    public class GlobalGenerator:ConfigGeneratorBase
    {
        public GlobalGenerator(XElement paramXmlElement,CodeNamespace paramCodeNamespace):base(paramXmlElement, paramCodeNamespace)
        {
            if (paramXmlElement.Name.LocalName!=GlobalEnum.Global.ToString())
                throw new Exception("输入element必须为 Global!");
            
            GenerateGlobal();
        }

        private void GenerateGlobal()
        {
            CodeTypeDeclaration = new CodeTypeDeclaration
            {
                Name = XmlElement.Name.LocalName,
                IsClass = true,
                TypeAttributes = TypeAttributes.Public
            };
            CodeNamespace.Types.Add(CodeTypeDeclaration);

            foreach (var xmlElement in XmlElement.Elements())
            {
                var codeMemberField = new CodeMemberField()
                {
                    Name = xmlElement.Name.LocalName,
                    Attributes = MemberAttributes.Public
                };
                codeMemberField.AppendCodeTypeAttribute<GlobalAttribute>(xmlElement);
                CodeTypeDeclaration.Members.Add(codeMemberField);
                if (xmlElement.Attribute(nameof(GlobalAttribute.type)) is XAttribute xAttribute)
                {
                    var type = xAttribute.Value;
                
                    if (type.IsBasicType())
                    {
                        if (type.IsArray())
                        {
                            codeMemberField.Type = new CodeTypeReference($"List<{type.BaseType().AsCodeType()}>");
                            if (xmlElement.Attribute(nameof(GlobalAttribute.initValue)) is XAttribute subAttribute)
                            {
                                if (type.BaseType() == BasicDataTypeEnum.String.ToString())
                                {
                                    var valueString = subAttribute.Value;
                                    var valueList = valueString.Split(',');
                                    foreach (var valueItem in valueList)
                                    {
                                        valueString = valueString.Replace(valueItem, $"$@\"{valueItem}\"");
                                    }
                                    codeMemberField.InitExpression =
                                        new CodeSnippetExpression($"new List<{type.BaseType().AsCodeType()}>(){{{valueString}}}");
                                }
                                else
                                {
                                    codeMemberField.InitExpression =
                                        new CodeSnippetExpression($"new List<{type.BaseType().AsCodeType()}>(){{{subAttribute.Value}}}");
                                }
                            }
                            else
                            {
                                if (type.BaseType() == BasicDataTypeEnum.String.ToString())
                                {
                                    codeMemberField.InitExpression = new CodeSnippetExpression($"new List<{type.BaseType().AsCodeType()}>()");
                                }
                                else
                                {
                                    codeMemberField.InitExpression = new CodeSnippetExpression($"new List<{type.BaseType().AsCodeType()}>()");
                                }
                            }
                        }
                        else
                        {
                            codeMemberField.Type = new CodeTypeReference(type.AsCodeType());
                            if (xmlElement.Attribute(nameof(GlobalAttribute.initValue)) is XAttribute subAttribute)
                            {
                                if (type.AsCodeType()== BasicDataTypeEnum.String.ToString())
                                {
                                    codeMemberField.InitExpression = new CodeSnippetExpression("$@\"" + subAttribute.Value + "\"");
                                }
                                else
                                {
                                    codeMemberField.InitExpression = new CodeSnippetExpression(subAttribute.Value);
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("[Config Error] Global 仅支持基本类型!");
                    }
                }
                else
                {
                    throw new Exception($@"[Config Error] Global without type!!");
                }
            }
        }
    }
}