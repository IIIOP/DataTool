using System;
using System.CodeDom;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using DataTool.CodeGenerate.Auxiliary;
using DataToolInterface.Data;
using DataToolInterface.Format.Config.Input;
using DataToolInterface.Format.Config.Model;
using DataToolInterface.Format.File;
using DataToolInterface.Format.File.Ini;

namespace DataTool.CodeGenerate.FileGenerate.IniGenerate.SectionGenerate
{
    [GenerateFileSubFormat(format = FileFormatEnum.Ini,subFormat = nameof(IniSubFormatEnum.Section))]
    public class SectionGenerator:FileGenerator
    {
        public SectionGenerator(XElement paramXmlElement, CodeNamespace paramCodeNamespace) : base(paramXmlElement, paramCodeNamespace)
        {
            GenerateSection();
        }

        private void GenerateSection()
        {
            var typeAttribute = XmlElement.Attribute(nameof(InputFileAttribute.type));
            if (XmlElement.HasElements)
            {
                if (typeAttribute==null)
                {
                    if (!CodeNamespace.HasCodeTypeDeclarationOfName(XmlElement.Name.LocalName))
                    {
                        CodeTypeDeclaration = new CodeTypeDeclaration
                        {
                            Name = CodeNamespace.GetUniqueNameByString(XmlElement.Name.LocalName),
                            IsClass = true,
                            TypeAttributes = TypeAttributes.Public
                        };
                    }
                    else
                    {
                        throw new Exception($@"类型{XmlElement.Name.LocalName}重复定义");
                    }
                }
                else
                {
                    if (Enum.TryParse(typeAttribute.Value,out AdvancedDataTypeEnum _))
                    {
                        CodeTypeDeclaration = new CodeTypeDeclaration
                        {
                            Name = CodeNamespace.GetUniqueNameByString(XmlElement.Name.LocalName),
                            IsClass = true,
                            TypeAttributes = TypeAttributes.Public
                        };
                    }
                    else
                    {
                        throw new Exception("类型错误");
                    }
                }
                CodeNamespace.Types.Add(CodeTypeDeclaration);
                
                var constructor = new CodeConstructor
                {
                    Attributes = MemberAttributes.Public
                };
                CodeTypeDeclaration.Members.Add(constructor);

                foreach (var xElement in XmlElement.Elements())
                {
                    var codeMemberField = new CodeMemberField()
                    {
                        Name = xElement.Name.LocalName,
                        Attributes = MemberAttributes.Public,
                    };
                    CodeTypeDeclaration.Members.Add(codeMemberField);
                    
                    if (xElement.Attribute(nameof(IniKeySingleAttribute.type)) is XAttribute xAttribute)
                    {
                        var type = xAttribute.Value;
                        if(type.IsArray())
                        {
                            codeMemberField.Type = new CodeTypeReference($"List<{type.BaseType().AsCodeType()}>");
                            constructor.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"{codeMemberField.Name} = new List<{type.BaseType().AsCodeType()}>()")));
                            codeMemberField.AppendCodeTypeAttribute<IniKeyMultipleAttribute>(xElement);
                        }
                        else if (!type.IsArray())
                        {
                            codeMemberField.Type = new CodeTypeReference(type.AsCodeType());
                            codeMemberField.AppendCodeTypeAttribute<IniKeySingleAttribute>(xElement);
                        }
                    }
                    else
                    {
                        throw new Exception($@"[Config Error] Struct without type");
                    }
                }
            }
            else
            {
                if (typeAttribute!=null)
                {
                    var type = typeAttribute.Value.BaseType();
                    if (!Enum.TryParse(type,out AdvancedDataTypeEnum _))
                    {
                        if (CodeNamespace.HasCodeTypeDeclarationOfName(type))
                        {
                            CodeTypeDeclaration = CodeNamespace.Types.Cast<CodeTypeDeclaration>()
                                .Single(p => p.Name == type);
                        }
                        else
                        {
                            throw new Exception($@"类型{type} 未定义");
                        }
                    }
                    else
                    {
                        throw new Exception($@"元素 {base.XmlElement.Name} 未定义子元素");
                    }
                }
                else
                {
                    throw new Exception("缺少 type 属性");
                }
            }
        }
    }
}