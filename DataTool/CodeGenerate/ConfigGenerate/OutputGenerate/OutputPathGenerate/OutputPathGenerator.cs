using System;
using System.CodeDom;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using DataTool.CodeGenerate.Auxiliary;
using DataTool.CodeGenerate.ConfigGenerate.Auxiliary;
using DataTool.CodeGenerate.ConfigGenerate.OutputGenerate.OutputFileGenerate;
using DataToolInterface.Data;
using DataToolInterface.Format.Config;
using DataToolInterface.Format.Config.Global;
using DataToolInterface.Format.Config.Input;
using DataToolInterface.Format.Config.Output;

namespace DataTool.CodeGenerate.ConfigGenerate.OutputGenerate.OutputPathGenerate
{
    public class OutputPathGenerator:ConfigGeneratorBase
    {
        public OutputPathGenerator(XElement paramXmlElement, CodeNamespace paramCodeNamespace) : base(paramXmlElement, paramCodeNamespace)
        {
            GenerateOutputPath();
        }
        
        private void GenerateOutputPath()
        {
            CodeTypeDeclaration = new CodeTypeDeclaration
            {
                Name = $@"{XmlElement.Parent?.Name.LocalName}_{XmlElement.Name.LocalName}",
                IsClass = true,
                TypeAttributes = TypeAttributes.Public
            };
            CodeNamespace.Types.Add(CodeTypeDeclaration);
            
            CodeTypeDeclaration.AppendCodeTypeAttribute<OutputPathAttribute>(XmlElement);

            var constructor = new CodeConstructor
            {
                Attributes = MemberAttributes.Public
            };
            CodeTypeDeclaration.Members.Add(constructor);

            var codeTypeDeclaration = new CodeTypeDeclaration()
            {
                Name = CodeNamespace.GetUniqueNameByString($@"{CodeTypeDeclaration.Name}_{OutputEnum.pathInfo}"),
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
            if (XmlElement.Attribute(nameof(OutputPathAttribute.path)) is XAttribute xAttribute)
            {
                foreach (Match match in regex.Matches(xAttribute.Value))
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
            
            CodeTypeDeclaration.AddPropertyDeclaration(codeTypeDeclaration.Name,OutputEnum.pathInfo.ToString());
            var codeConstructor = CodeTypeDeclaration.Members.Cast<CodeTypeMember>().OfType<CodeConstructor>().Single();
            codeConstructor.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"{OutputEnum.pathInfo.ToString()} = new {codeTypeDeclaration.Name}()")));
            
            foreach (var xElement in XmlElement.Elements())
            {
                var outputFileGenerator = new OutputFileGenerator(xElement, CodeNamespace);
                
                codeMemberField = new CodeMemberField()
                {
                    Name = xElement.Name.LocalName,
                    Attributes = MemberAttributes.Public,
                };
                CodeTypeDeclaration.Members.Add(codeMemberField);
                codeMemberField.AppendCodeTypeAttribute<OutputFileAttribute>(xElement);
                if (xElement.Attribute(nameof(OutputFileAttribute.type)) is XAttribute lAttribute)
                {
                    var typeValue = lAttribute.Value;
                    if (typeValue == AdvancedDataTypeEnum.Multiple.ToString())
                    {
                        codeMemberField.Type = new CodeTypeReference($@"List<{outputFileGenerator.generatedTypeName}>");
                        constructor.Statements.Add(new CodeExpressionStatement(
                            new CodeSnippetExpression($"{codeMemberField.Name} = new List<{outputFileGenerator.generatedTypeName}>()")));

                    }
                    else if (typeValue == AdvancedDataTypeEnum.Single.ToString())
                    {
                        codeMemberField.Type = new CodeTypeReference(outputFileGenerator.generatedTypeName);
                    }
                    else if (typeValue.IsArray())
                    {
                        codeMemberField.Type = new CodeTypeReference($@"List<{outputFileGenerator.generatedTypeName}>");
                        constructor.Statements.Add(new CodeExpressionStatement(
                            new CodeSnippetExpression($"{codeMemberField.Name} = new List<{outputFileGenerator.generatedTypeName}>()")));
                    }
                    else
                    {
                        codeMemberField.Type = new CodeTypeReference(outputFileGenerator.generatedTypeName);
                    }
                }
                else
                {
                    throw new Exception("没有type属性");
                }
            }
        }
    }
}