using System;
using System.CodeDom;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using DataTool.CodeGenerate.Auxiliary;
using DataTool.CodeGenerate.FileGenerate.XmlGenerate.AttributeGenerate;
using DataTool.CodeGenerate.FileGenerate.XmlGenerate.DefaultGenerate;
using DataToolInterface.Data;
using DataToolInterface.Format.File;
using DataToolInterface.Format.File.Xml;
using DataToolLog;

namespace DataTool.CodeGenerate.FileGenerate.XmlGenerate.ContentGenerate
{
    [GenerateFileSubFormat(format = FileFormatEnum.Xml,subFormat = nameof(XmlSubFormatEnum.Content))]
    public class ContentGenerator:FileGenerator
    {
        public ContentGenerator(XElement paramXmlElement, CodeNamespace paramCodeNamespace) : base(paramXmlElement, paramCodeNamespace)
        {
            GenerateContent();
        }

        private void GenerateContent()
        {
            CodeTypeDeclaration = new CodeTypeDeclaration
            {
                Name = CodeNamespace.GetUniqueNameByString(XmlElement.Name.LocalName),
                IsClass = true,
                TypeAttributes = TypeAttributes.Public
            };
            CodeNamespace.Types.Add(CodeTypeDeclaration);

            var constructor = new CodeConstructor
            {
                Attributes = MemberAttributes.Public
            };
            CodeTypeDeclaration.Members.Add(constructor);
                
            var attributeGenerator = new AttributeGenerator(XmlElement, CodeNamespace);
            var codeMemberField = new CodeMemberField()
            {
                Name = XmlFieldNameEnum.attribute.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference(attributeGenerator.generatedTypeName),
            };
            CodeTypeDeclaration.Members.Add(codeMemberField);
            var codeAttributeDeclaration = new CodeAttributeDeclaration(nameof(XmlAttributeAttribute));
            codeMemberField.CustomAttributes.Add(codeAttributeDeclaration);
            
            constructor.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($@"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));

            if (!string.IsNullOrWhiteSpace(XmlElement.Value))
            {
                var type = XmlElement.Value.Trim();
                if (Enum.TryParse(type.BaseType(),out BasicDataTypeEnum _))
                {
                    codeMemberField = new CodeMemberField()
                    {
                        Name = XmlFieldNameEnum.content.ToString(),
                        Attributes = MemberAttributes.Public,
                    };
                    CodeTypeDeclaration.Members.Add(codeMemberField);

                    if (type.IsArray())
                    {
                        codeMemberField.Type = new CodeTypeReference($"List<{type.BaseType().AsCodeType()}>");
                        constructor.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}>()")));
                        codeAttributeDeclaration = new CodeAttributeDeclaration(nameof(XmlContentSingleAttribute));
                        codeMemberField.CustomAttributes.Add(codeAttributeDeclaration);
                        codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument(nameof(XmlContentSingleAttribute.type), new CodeSnippetExpression("@\"" + type + "\"")));
                    }
                    else
                    {
                        codeMemberField.Type = new CodeTypeReference($"List<{type.AsCodeType()}>");
                        codeAttributeDeclaration = new CodeAttributeDeclaration(nameof(XmlContentMultipleAttribute));
                        codeMemberField.CustomAttributes.Add(codeAttributeDeclaration);
                        codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument(nameof(XmlContentMultipleAttribute.type), new CodeSnippetExpression("@\"" + type + "\"")));
                    }
                }
                else
                {
                    //throw new Exception($@"<{GetType().Name}> Xml Content type can only be basic");
                    LogHelper.DefaultLog.WriteLine($@"<{GetType().Name}> Xml Content type can only be basic");
                }
            }
            
            if (XmlElement.HasElements)
            {
                foreach (var xElement in XmlElement.Elements())
                {
                    codeMemberField = new CodeMemberField()
                    {
                        Name = xElement.Name.LocalName,
                        Attributes = MemberAttributes.Public,
                    };
                    CodeTypeDeclaration.Members.Add(codeMemberField);

                    if (xElement.Attribute(nameof(XmlElementSingleAttribute.type)) is XAttribute xAttribute)
                    {
                        var contentGenerator = new ContentGenerator(xElement, CodeNamespace);
                        
                        var type = xAttribute.Value.Split('|').First();
                        if (type==AdvancedDataTypeEnum.Multiple.ToString())
                        {
                            codeMemberField.Type = new CodeTypeReference($"List<{contentGenerator.generatedTypeName}>");
                            constructor.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"{codeMemberField.Name} = new List<{contentGenerator.generatedTypeName}>()")));
                            codeMemberField.AppendCodeTypeAttribute<XmlElementMultipleAttribute>(xElement);
                        }
                        else if(type==AdvancedDataTypeEnum.Single.ToString())
                        {
                            codeMemberField.Type = new CodeTypeReference($"{contentGenerator.generatedTypeName}");
                            constructor.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"{codeMemberField.Name} = new {contentGenerator.generatedTypeName}()")));
                            codeMemberField.AppendCodeTypeAttribute<XmlElementSingleAttribute>(xElement);
                        }
                        else if(type.IsArray())
                        {
                            codeMemberField.Type = new CodeTypeReference($"List<{contentGenerator.generatedTypeName}>");
                            constructor.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"{codeMemberField.Name} = new List<{contentGenerator.generatedTypeName}>()")));
                            codeMemberField.AppendCodeTypeAttribute<XmlElementMultipleAttribute>(xElement);
                                
                        }
                        else if (!type.IsArray())
                        {
                            codeMemberField.Type = new CodeTypeReference(contentGenerator.generatedTypeName);
                            constructor.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"{codeMemberField.Name} = new {contentGenerator.generatedTypeName}()")));
                            codeMemberField.AppendCodeTypeAttribute<XmlElementSingleAttribute>(xElement);
                        }
                    }
                    else
                    {
                        throw new Exception($@"<{GetType().Name}> Xml Element[{xElement.Name.LocalName}] lack of [{nameof(XmlElementSingleAttribute.type)}]");
                    }
                }
            }
            else
            {
                var defaultGenerator = new DefaultGenerator(XmlElement, CodeNamespace);
                codeMemberField = new CodeMemberField()
                {
                    Name = XmlFieldNameEnum.elements.ToString(),
                    Attributes = MemberAttributes.Public,
                    Type = new CodeTypeReference($@"List<{defaultGenerator.generatedTypeName}>"),
                };
                CodeTypeDeclaration.Members.Add(codeMemberField);
                constructor.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));
                codeAttributeDeclaration = new CodeAttributeDeclaration(nameof(XmlElementDefaultAttribute));
                codeMemberField.CustomAttributes.Add(codeAttributeDeclaration);
            }
        }
    }
}