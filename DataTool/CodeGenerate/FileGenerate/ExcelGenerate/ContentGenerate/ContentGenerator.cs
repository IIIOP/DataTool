using System;
using System.CodeDom;
using System.Reflection;
using System.Xml.Linq;
using DataTool.CodeGenerate.Auxiliary;
using DataToolInterface.Data;
using DataToolInterface.Format.File;
using DataToolInterface.Format.File.Excel;

namespace DataTool.CodeGenerate.FileGenerate.ExcelGenerate.ContentGenerate
{
    [GenerateFileSubFormat(format = FileFormatEnum.Excel,subFormat = nameof(ExcelSubFormatEnum.Content))]
    public class ContentGenerator:FileGenerator
    {
        public ContentGenerator(XElement paramXmlElement,CodeNamespace paramCodeNamespace):base(paramXmlElement, paramCodeNamespace)
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
            
            foreach (var xElement in XmlElement.Elements())
            {
                var codeMemberField = new CodeMemberField
                {
                    Name = xElement.Name.LocalName,
                    Attributes = MemberAttributes.Public
                };
                CodeTypeDeclaration.Members.Add(codeMemberField);

                if (xElement.Attribute(nameof(SheetContentBasicSingleAttribute.type)) is XAttribute xAttribute)
                {
                    var type = xAttribute.Value;

                    if (type==AdvancedDataTypeEnum.Multiple.ToString())
                    {
                        var contentGenerator = new ContentGenerator(xElement, CodeNamespace);
                        codeMemberField.Type = new CodeTypeReference($"List<{contentGenerator.generatedTypeName}>");
                        constructor.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"{xElement.Name.LocalName} = new List<{contentGenerator.generatedTypeName}>()")));
                        codeMemberField.AppendCodeTypeAttribute<SheetContentAdvancedMultipleAttribute>(xElement);
                    }
                    else if(type==AdvancedDataTypeEnum.Single.ToString())
                    {
                        var contentGenerator = new ContentGenerator(xElement, CodeNamespace);
                        codeMemberField.Type = new CodeTypeReference($"{contentGenerator.generatedTypeName}");
                        constructor.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"{xElement.Name.LocalName} = new {contentGenerator.generatedTypeName}()")));
                        codeMemberField.AppendCodeTypeAttribute<SheetContentAdvancedSingleAttribute>(xElement);
                    }
                    else if(type.IsArray()&&type.IsBasicType())
                    {
                        codeMemberField.Type = new CodeTypeReference($"List<{type.BaseType().AsCodeType()}>");
                        constructor.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"{xElement.Name.LocalName} = new List<{type.BaseType().AsCodeType()}>()")));
                        codeMemberField.AppendCodeTypeAttribute<SheetContentBasicMultipleAttribute>(xElement);
                    }
                    else if (type.IsArray()&&!type.IsBasicType())
                    {
                        if (CodeNamespace.HasCodeTypeDeclarationOfName(type.BaseType()))
                        {
                            codeMemberField.Type = new CodeTypeReference($"List<{type.BaseType()}>");
                            constructor.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"{xElement.Name.LocalName} = new List<{type.BaseType()}>()")));
                            codeMemberField.AppendCodeTypeAttribute<SheetContentAdvancedMultipleAttribute>(xElement);
                        }
                        else
                        {
                            throw new Exception("Type not found!");
                        }
                    }
                    else if (!type.IsArray()&&type.IsBasicType())
                    {
                        codeMemberField.Type = new CodeTypeReference(type.AsCodeType());
                        codeMemberField.AppendCodeTypeAttribute<SheetContentBasicSingleAttribute>(xElement);
                    }
                    else if(!type.IsArray()&&!type.IsBasicType())
                    {
                        if (CodeNamespace.HasCodeTypeDeclarationOfName(type))
                        {
                            codeMemberField.Type = new CodeTypeReference($"{type}");
                            constructor.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"{xElement.Name.LocalName} = new {type}()")));
                            codeMemberField.AppendCodeTypeAttribute<SheetContentAdvancedSingleAttribute>(xElement);
                        }
                        else
                        {
                            throw new Exception("Type not found!");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($@"[Config Error] Excel without type attribute");
                }
            }
        }
    }
}