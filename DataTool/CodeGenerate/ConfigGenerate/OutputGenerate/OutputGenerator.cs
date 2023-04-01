using System;
using System.CodeDom;
using System.Reflection;
using System.Xml.Linq;
using DataTool.CodeGenerate.Auxiliary;
using DataTool.CodeGenerate.ConfigGenerate.Auxiliary;
using DataTool.CodeGenerate.ConfigGenerate.OutputGenerate.OutputPathGenerate;
using DataToolInterface.Data;
using DataToolInterface.Format.Config.Output;
using DataToolInterface.Format.File;

namespace DataTool.CodeGenerate.ConfigGenerate.OutputGenerate
{
    public class OutputGenerator:ConfigGeneratorBase
    {
        public OutputGenerator(XElement paramXmlElement,CodeNamespace paramCodeNamespace):base(paramXmlElement, paramCodeNamespace)
        {
            if (paramXmlElement.Name.LocalName!=OutputEnum.Output.ToString())
                throw new Exception("输入element必须为 Output!");
            
            GenerateOutput();
        }

        private void GenerateOutput()
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
            
            CodeTypeDeclaration.AppendCodeTypeAttribute<OutputAttribute>(XmlElement);
            
            foreach (var xElement in XmlElement.Elements())
            {
                var outputPathGenerator = new OutputPathGenerator(xElement, CodeNamespace);
                var codeMemberField = new CodeMemberField()
                {
                    Name = xElement.Name.LocalName,
                    Type = new CodeTypeReference(outputPathGenerator.generatedTypeName),
                    Attributes = MemberAttributes.Public,
                };
                CodeTypeDeclaration.Members.Add(codeMemberField);

                constructor.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));
                
            }
        }
    }
}