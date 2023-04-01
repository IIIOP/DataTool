using System;
using System.CodeDom;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using DataTool.CodeGenerate.Auxiliary;
using DataTool.CodeGenerate.ConfigGenerate.Auxiliary;
using DataTool.CodeGenerate.ConfigGenerate.InputGenerate.InputFileGenerate;
using DataToolInterface.Data;
using DataToolInterface.Format.Config;
using DataToolInterface.Format.Config.Global;
using DataToolInterface.Format.Config.Input;

namespace DataTool.CodeGenerate.ConfigGenerate.InputGenerate.InputPathGenerate
{
    public class InputPathGenerator:ConfigGeneratorBase
    {
        public InputPathGenerator(XElement paramXmlElement, CodeNamespace paramCodeNamespace) : base(paramXmlElement, paramCodeNamespace)
        {
            GenerateInputPath();
        }

        private void GenerateInputPath()
        {
            CodeTypeDeclaration = new CodeTypeDeclaration
            {
                Name = $@"{XmlElement.Parent?.Name.LocalName}_{XmlElement.Name.LocalName}",
                IsClass = true,
                TypeAttributes = TypeAttributes.Public
            };
            CodeNamespace.Types.Add(CodeTypeDeclaration);

            CodeTypeDeclaration.AppendCodeTypeAttribute<InputPathAttribute>(XmlElement);
            
            var constructor = new CodeConstructor
            {
                Attributes = MemberAttributes.Public
            };
            CodeTypeDeclaration.Members.Add(constructor);

            var codeTypeDeclaration = new CodeTypeDeclaration()
            {
                Name = CodeNamespace.GetUniqueNameByString($@"{CodeTypeDeclaration.Name}_{InputEnum.pathInfo}"),
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
            if (XmlElement.Attribute(nameof(InputPathAttribute.path)) is XAttribute xAttribute)
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
            
            CodeTypeDeclaration.AddPropertyDeclaration(codeTypeDeclaration.Name,InputEnum.pathInfo.ToString());
            var codeConstructor = CodeTypeDeclaration.Members.Cast<CodeTypeMember>().OfType<CodeConstructor>().Single();
            codeConstructor.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"{InputEnum.pathInfo.ToString()} = new {codeTypeDeclaration.Name}()")));
            
            foreach (var xElement in XmlElement.Elements())
            {
                var inputFileGenerator = new InputFileGenerator(xElement, CodeNamespace);

                codeMemberField = new CodeMemberField()
                {
                    Name = xElement.Name.LocalName,
                    Attributes = MemberAttributes.Public,
                };
                CodeTypeDeclaration.Members.Add(codeMemberField);
                codeMemberField.AppendCodeTypeAttribute<InputFileAttribute>(xElement);
                if (xElement.Attribute(nameof(InputFileAttribute.type)) is XAttribute attribute)
                {
                    if (attribute.Value==AdvancedDataTypeEnum.Single.ToString())
                    {
                        codeMemberField.Type = new CodeTypeReference(inputFileGenerator.generatedTypeName);
                    }
                    else if (attribute.Value==AdvancedDataTypeEnum.Multiple.ToString())
                    {
                        codeMemberField.Type = new CodeTypeReference($@"List<{inputFileGenerator.generatedTypeName}>");
                        constructor.Statements.Add(new CodeExpressionStatement(
                            new CodeSnippetExpression($"{codeMemberField.Name} = new List<{inputFileGenerator.generatedTypeName}>()")));
                    }
                    else if (attribute.Value.IsArray())
                    {
                        codeMemberField.Type = new CodeTypeReference($@"List<{inputFileGenerator.generatedTypeName}>");
                        constructor.Statements.Add(new CodeExpressionStatement(
                            new CodeSnippetExpression($"{codeMemberField.Name} = new List<{inputFileGenerator.generatedTypeName}>()")));
                    }
                    else
                    {
                        codeMemberField.Type = new CodeTypeReference(inputFileGenerator.generatedTypeName);
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