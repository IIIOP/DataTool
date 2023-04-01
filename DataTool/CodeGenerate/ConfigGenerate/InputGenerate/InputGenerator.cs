using System;
using System.CodeDom;
using System.Reflection;
using System.Xml.Linq;
using DataTool.CodeGenerate.Auxiliary;
using DataTool.CodeGenerate.ConfigGenerate.Auxiliary;
using DataTool.CodeGenerate.ConfigGenerate.InputGenerate.InputPathGenerate;
using DataTool.CodeGenerate.FileGenerate;
using DataToolInterface.Data;
using DataToolInterface.Format.Config.Input;

namespace DataTool.CodeGenerate.ConfigGenerate.InputGenerate
{
    public class InputGenerator:ConfigGeneratorBase
    {
        public InputGenerator(XElement paramXmlElement,CodeNamespace paramCodeNamespace):base(paramXmlElement, paramCodeNamespace)
        {
            if (paramXmlElement.Name.LocalName!=InputEnum.Input.ToString())
                throw new Exception("输入element必须为 Input!");

            GenerateInput();
        }

        private void GenerateInput()
        {
            CodeTypeDeclaration = new CodeTypeDeclaration
            {
                Name = XmlElement.Name.LocalName,
                IsClass = true,
                TypeAttributes = TypeAttributes.Public
            };
            CodeNamespace.Types.Add(CodeTypeDeclaration);
            
            var constructor = new CodeConstructor
            {
                Attributes = MemberAttributes.Public
            };
            CodeTypeDeclaration.Members.Add(constructor);
            
            CodeTypeDeclaration.AppendCodeTypeAttribute<InputAttribute>(XmlElement);
            
            foreach (var xElement in XmlElement.Elements())
            {
                var inputPathGenerator = new InputPathGenerator(xElement, CodeNamespace);
                var codeMemberField = new CodeMemberField()
                {
                    Name = xElement.Name.LocalName,
                    Type = new CodeTypeReference(inputPathGenerator.generatedTypeName),
                    Attributes = MemberAttributes.Public,
                };
                CodeTypeDeclaration.Members.Add(codeMemberField);
                
                constructor.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));
            }
        }
    }
}