using System;
using System.CodeDom;
using System.Reflection;
using System.Xml.Linq;
using DataTool.CodeGenerate.Auxiliary;
using DataTool.CodeGenerate.FileGenerate.ExcelGenerate.DefaultGenerate;
using DataTool.CodeGenerate.FileGenerate.ExcelGenerate.SheetGenerate;
using DataToolInterface.Data;
using DataToolInterface.Format.File;
using DataToolInterface.Format.File.Excel;

namespace DataTool.CodeGenerate.FileGenerate.ExcelGenerate
{
    [GenerateFileFormat(format = FileFormatEnum.Excel)]
    [GenerateFileSubFormat(format = FileFormatEnum.Excel,subFormat = nameof(ExcelSubFormatEnum.Excel))]
    public class ExcelGenerator:FileGenerator
    {
        public ExcelGenerator(XElement paramXmlElement,CodeNamespace paramCodeNamespace):base(paramXmlElement, paramCodeNamespace)
        {
            GenerateExcel();
        }

        private void GenerateExcel()
        {
            CodeTypeDeclaration = new CodeTypeDeclaration
            {
                Name = CodeNamespace.GetUniqueNameByString(XmlElement.Name.LocalName),
                IsClass = true,
                TypeAttributes = TypeAttributes.Public
            };
            CodeNamespace.Types.Add(CodeTypeDeclaration);
            CodeTypeDeclaration.AppendCodeTypeAttribute<ExcelFileAttribute>(XmlElement);
            
            var constructor = new CodeConstructor
            {
                Attributes = MemberAttributes.Public
            };
            CodeTypeDeclaration.Members.Add(constructor);
            
            if (XmlElement.HasElements)
            {
                foreach (var xElement in XmlElement.Elements())
                {
                    var codeMemberField = new CodeMemberField
                    {
                        Name = xElement.Name.LocalName,
                        Attributes = MemberAttributes.Public
                    };
                    CodeTypeDeclaration.Members.Add(codeMemberField);

                    if (xElement.Attribute(nameof(ExcelSheetAttribute.type)) is XAttribute attribute)
                    {
                        if (attribute.Value==AdvancedDataTypeEnum.Single.ToString())
                        {
                            if (xElement.HasElements)
                            {
                                codeMemberField.AppendCodeTypeAttribute<ExcelSheetAttribute>(xElement);
                                var sheetGenerator = new SheetGenerator(xElement, CodeNamespace);
                                codeMemberField.Type = new CodeTypeReference($"{sheetGenerator.generatedTypeName}");
                                constructor.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"{codeMemberField.Name} = new {sheetGenerator.generatedTypeName}()")));
                            }
                            else
                            {
                                codeMemberField.AppendCodeTypeAttribute<ExcelSheetDefaultAttribute>(xElement);
                                var defaultGenerator = new DefaultGenerator(xElement, CodeNamespace);
                                codeMemberField.Type = new CodeTypeReference($"{defaultGenerator.generatedTypeName}");
                                constructor.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"{codeMemberField.Name} = new {defaultGenerator.generatedTypeName}()")));
                            }
                        }
                        else if (attribute.Value==AdvancedDataTypeEnum.Multiple.ToString())
                        {
                            if (xElement.HasElements)
                            {
                                codeMemberField.AppendCodeTypeAttribute<ExcelSheetAttribute>(xElement);
                                var sheetGenerator = new SheetGenerator(xElement, CodeNamespace);
                                codeMemberField.Type = new CodeTypeReference($"List<{sheetGenerator.generatedTypeName}>");
                                constructor.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"{codeMemberField.Name} = new List<{sheetGenerator.generatedTypeName}>()")));
                            }
                            else
                            {
                                codeMemberField.AppendCodeTypeAttribute<ExcelSheetDefaultAttribute>(xElement);
                                var defaultGenerator = new DefaultGenerator(xElement, CodeNamespace);
                                codeMemberField.Type = new CodeTypeReference($"List<{defaultGenerator.generatedTypeName}>");
                                constructor.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"{codeMemberField.Name} = new List<{defaultGenerator.generatedTypeName}>()")));
                            }
                        }
                        else if (attribute.Value.IsArray())
                        {
                            codeMemberField.AppendCodeTypeAttribute<ExcelSheetAttribute>(xElement);
                            if (CodeNamespace.HasCodeTypeDeclarationOfName(attribute.Value.BaseType()))
                            {
                                codeMemberField.Type = new CodeTypeReference($"List<{attribute.Value.BaseType()}>");
                                constructor.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"{codeMemberField.Name} = new List<{attribute.Value.BaseType()}>()")));
                            }
                            else
                            {
                                throw new Exception($@"type is not defined");
                            }
                        }
                        else
                        {
                            codeMemberField.AppendCodeTypeAttribute<ExcelSheetAttribute>(xElement);
                            if (CodeNamespace.HasCodeTypeDeclarationOfName(attribute.Value))
                            {
                                codeMemberField.Type = new CodeTypeReference($"{attribute.Value}");
                                constructor.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"{codeMemberField.Name} = new {attribute.Value}()")));
                            }
                            else
                            {
                                throw new Exception($@"type is not defined");
                            }
                        }
                    }
                }
            }
            else
            {
                var defaultGenerator = new DefaultGenerator(XmlElement, CodeNamespace);
                var codeMemberField = new CodeMemberField
                {
                    Name = ExcelFieldNameEnum.sheets.ToString(),
                    Attributes = MemberAttributes.Public,
                    Type = new CodeTypeReference($@"List<{defaultGenerator.generatedTypeName}>")
                };
                CodeTypeDeclaration.Members.Add(codeMemberField);
                codeMemberField.AppendCodeTypeAttribute<ExcelDefaultAttribute>(XmlElement);
                constructor.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));
            }
        }
    }
}