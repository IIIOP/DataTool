using System;
using System.CodeDom;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using DataTool.CodeGenerate.Auxiliary;
using DataTool.CodeGenerate.ConfigGenerate.Auxiliary;
using DataTool.CodeGenerate.FileGenerate;
using DataToolInterface.Data;
using DataToolInterface.Format.Config;
using DataToolInterface.Format.Config.Global;
using DataToolInterface.Format.Config.Input;
using DataToolInterface.Format.File;

namespace DataTool.CodeGenerate.ConfigGenerate.InputGenerate.InputFileGenerate
{
    public class InputFileGenerator:ConfigGeneratorBase
    {
        public InputFileGenerator(XElement paramXmlElement, CodeNamespace paramCodeNamespace) : base(paramXmlElement, paramCodeNamespace)
        {
            GenerateInputFile();
        }

        private void GenerateInputFile()
        {
            if (XmlElement.Attribute(nameof(InputFileAttribute.type)) is XAttribute typeAttribute)
            {
                if (typeAttribute.Value==AdvancedDataTypeEnum.Single.ToString()||typeAttribute.Value==AdvancedDataTypeEnum.Multiple.ToString())
                {
                    var fileGenerator = FileGenerator.GenerateFile(XmlElement, CodeNamespace);
            
                    CodeTypeDeclaration = fileGenerator.CodeTypeDeclaration;
                    
                    var codeTypeDeclaration = new CodeTypeDeclaration()
                    {
                        Name = CodeNamespace.GetUniqueNameByString($@"{CodeTypeDeclaration.Name}_{FileInfoEnum.fileInfo}"),
                        IsClass = true,
                        TypeAttributes = TypeAttributes.Public
                    };
                    CodeNamespace.Types.Add(codeTypeDeclaration);
                    
                    var codeMemberField = new CodeMemberField()
                    {
                        Name = GlobalEnum.Global.ToString(),
                        Attributes = MemberAttributes.Public,
                        Type = new CodeTypeReference(GlobalEnum.Global.ToString())
                    };
                    codeTypeDeclaration.Members.Add(codeMemberField);
                    codeMemberField.InitExpression = new CodeSnippetExpression($@"{CodeNamespace.Types.Cast<CodeTypeDeclaration>()
                        .Single(p => p.CustomAttributes.Cast<CodeAttributeDeclaration>()
                            .Any(a=>a.Name == nameof(DataToolAttribute))).Name}.{GlobalEnum.Global.ToString()}");
                    
                    var regex = new Regex(@"(?<={local:)\w+(?=})");
                    if (XmlElement.Attribute(nameof(InputFileAttribute.path)) is XAttribute attribute)
                    {
                        foreach (Match match in regex.Matches(attribute.Value))
                        {
                            codeMemberField = new CodeMemberField()
                            {
                                Name = match.Value,
                                Attributes = MemberAttributes.Public,
                                Type = new CodeTypeReference("String"),
                                InitExpression = new CodeSnippetExpression($"\"{match.Value}\""),
                            };
                            if (codeTypeDeclaration.Members.OfType<CodeMemberField>().All(p => p.Name != codeMemberField.Name))
                            {
                                codeTypeDeclaration.Members.Add(codeMemberField);
                            }
                        }
                    }
                    if (XmlElement.Attribute(nameof(InputFileAttribute.name)) is XAttribute cattribute)
                    {
                        foreach (Match match in regex.Matches(cattribute.Value))
                        {
                            codeMemberField = new CodeMemberField()
                            {
                                Name = match.Value,
                                Attributes = MemberAttributes.Public,
                                Type = new CodeTypeReference("String"),
                                InitExpression = new CodeSnippetExpression($"\"{match.Value}\""),
                            };
                            if (codeTypeDeclaration.Members.OfType<CodeMemberField>().All(p => p.Name != codeMemberField.Name))
                            {
                                codeTypeDeclaration.Members.Add(codeMemberField);
                            }
                        }
                    }
                    
                    CodeTypeDeclaration.AddPropertyDeclaration(codeTypeDeclaration.Name,FileInfoEnum.fileInfo.ToString());
                    var codeConstructor = CodeTypeDeclaration.Members.Cast<CodeTypeMember>().OfType<CodeConstructor>().Single();
                    codeConstructor.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"{FileInfoEnum.fileInfo.ToString()} = new {codeTypeDeclaration.Name}()")));
                }
                else
                {
                    CodeTypeDeclaration codeTypeDeclaration = null;
                    if (CodeNamespace.HasCodeTypeDeclarationOfName(typeAttribute.Value.BaseType()))
                    {
                        CodeTypeDeclaration = CodeNamespace.GetCodeTypeDeclarationByName(typeAttribute.Value.BaseType());
                        if (CodeNamespace.HasCodeTypeDeclarationOfName($@"{CodeTypeDeclaration.Name}_{FileInfoEnum.fileInfo}"))
                        {
                            codeTypeDeclaration = CodeNamespace.GetCodeTypeDeclarationByName($@"{CodeTypeDeclaration.Name}_{FileInfoEnum.fileInfo}");
                        }
                    }
                    else if (CodeGenerator.DefaultCodeNamespace.HasCodeTypeDeclarationOfName(typeAttribute.Value.BaseType()))
                    {
                        CodeTypeDeclaration = CodeGenerator.DefaultCodeNamespace.GetCodeTypeDeclarationByName(typeAttribute.Value.BaseType());
                        if (CodeGenerator.DefaultCodeNamespace.HasCodeTypeDeclarationOfName($@"{CodeTypeDeclaration.Name}_{FileInfoEnum.fileInfo}"))
                        {
                            codeTypeDeclaration = CodeGenerator.DefaultCodeNamespace.GetCodeTypeDeclarationByName($@"{CodeTypeDeclaration.Name}_{FileInfoEnum.fileInfo}");
                        }
                    }
                    else
                    {
                        throw new Exception($@"type [{typeAttribute.Value.BaseType()}] is not defined!");
                    }

                    if (codeTypeDeclaration==null)
                    {
                        codeTypeDeclaration = new CodeTypeDeclaration()
                        {
                            Name = CodeNamespace.GetUniqueNameByString($@"{CodeTypeDeclaration.Name}_{FileInfoEnum.fileInfo}"),
                            IsClass = true,
                            TypeAttributes = TypeAttributes.Public
                        };
                        CodeNamespace.Types.Add(codeTypeDeclaration);
                        
                        var codeMemberField = new CodeMemberField()
                        {
                            Name = GlobalEnum.Global.ToString(),
                            Attributes = MemberAttributes.Public,
                            Type = new CodeTypeReference(GlobalEnum.Global.ToString())
                        };
                        codeTypeDeclaration.Members.Add(codeMemberField);
                        codeMemberField.InitExpression = new CodeSnippetExpression($@"{CodeNamespace.Types.Cast<CodeTypeDeclaration>()
                            .Single(p => p.CustomAttributes.Cast<CodeAttributeDeclaration>()
                                .Any(a=>a.Name == nameof(DataToolAttribute))).Name}.{GlobalEnum.Global.ToString()}");
                        
                        var regex = new Regex(@"(?<={local:)\w+(?=})");
                        if (XmlElement.Attribute(nameof(InputFileAttribute.path)) is XAttribute attribute)
                        {
                            foreach (Match match in regex.Matches(attribute.Value))
                            {
                                codeMemberField = new CodeMemberField()
                                {
                                    Name = match.Value,
                                    Attributes = MemberAttributes.Public,
                                    Type = new CodeTypeReference("String"),
                                    InitExpression = new CodeSnippetExpression($"\"{match.Value}\""),
                                };
                                if (codeTypeDeclaration.Members.OfType<CodeMemberField>().All(p => p.Name != codeMemberField.Name))
                                {
                                    codeTypeDeclaration.Members.Add(codeMemberField);
                                }
                            }
                        }
                        if (XmlElement.Attribute(nameof(InputFileAttribute.name)) is XAttribute cattribute)
                        {
                            foreach (Match match in regex.Matches(cattribute.Value))
                            {
                                codeMemberField = new CodeMemberField()
                                {
                                    Name = match.Value,
                                    Attributes = MemberAttributes.Public,
                                    Type = new CodeTypeReference("String"),
                                    InitExpression = new CodeSnippetExpression($"\"{match.Value}\""),
                                };
                                if (codeTypeDeclaration.Members.OfType<CodeMemberField>().All(p => p.Name != codeMemberField.Name))
                                {
                                    codeTypeDeclaration.Members.Add(codeMemberField);
                                }
                            }
                        }
                        
                        CodeTypeDeclaration.AddPropertyDeclaration(codeTypeDeclaration.Name,FileInfoEnum.fileInfo.ToString());
                        var codeConstructor = CodeTypeDeclaration.Members.Cast<CodeTypeMember>().OfType<CodeConstructor>().Single();
                        codeConstructor.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"{FileInfoEnum.fileInfo.ToString()} = new {codeTypeDeclaration.Name}()")));
                    }
                    else
                    {
                        var regex = new Regex(@"(?<={local:)\w+(?=})");
                        if (XmlElement.Attribute(nameof(InputFileAttribute.path)) is XAttribute attribute)
                        {
                            foreach (Match match in regex.Matches(attribute.Value))
                            {
                                var codeMemberField = new CodeMemberField()
                                {
                                    Name = match.Value,
                                    Attributes = MemberAttributes.Public,
                                    Type = new CodeTypeReference("String"),
                                    InitExpression = new CodeSnippetExpression($"\"{match.Value}\""),
                                };
                                if (codeTypeDeclaration.Members.OfType<CodeMemberField>().All(p => p.Name != codeMemberField.Name))
                                {
                                    codeTypeDeclaration.Members.Add(codeMemberField);
                                }
                            }
                        }
                        if (XmlElement.Attribute(nameof(InputFileAttribute.name)) is XAttribute cattribute)
                        {
                            foreach (Match match in regex.Matches(cattribute.Value))
                            {
                                var codeMemberField = new CodeMemberField()
                                {
                                    Name = match.Value,
                                    Attributes = MemberAttributes.Public,
                                    Type = new CodeTypeReference("String"),
                                    InitExpression = new CodeSnippetExpression($"\"{match.Value}\""),
                                };
                                if (codeTypeDeclaration.Members.OfType<CodeMemberField>().All(p => p.Name != codeMemberField.Name))
                                {
                                    codeTypeDeclaration.Members.Add(codeMemberField);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                throw new Exception("没有type属性");
            }
        }
    }
}