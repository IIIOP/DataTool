using System.CodeDom;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using DataTool.CodeGenerate.Auxiliary;
using DataTool.CodeGenerate.FileGenerate.XmlGenerate.ContentGenerate;
using DataTool.CodeGenerate.FileGenerate.XmlGenerate.DefaultGenerate;
using DataToolInterface.Format.File;
using DataToolInterface.Format.File.Xml;

namespace DataTool.CodeGenerate.FileGenerate.XmlGenerate
{
    [GenerateFileFormat(format = FileFormatEnum.Xml)]
    [GenerateFileSubFormat(format = FileFormatEnum.Xml,subFormat = nameof(XmlSubFormatEnum.Xml))]
    public class XmlGenerator:FileGenerator
    {
        public XmlGenerator(XElement paramXmlElement,CodeNamespace paramCodeNamespace):base(paramXmlElement, paramCodeNamespace)
        {
            GenerateXml();
        }

        private void GenerateXml()
        {
            CodeTypeDeclaration = new CodeTypeDeclaration
            {
                Name = CodeNamespace.GetUniqueNameByString(XmlElement.Name.LocalName),
                IsClass = true,
                TypeAttributes = TypeAttributes.Public
            };
            CodeNamespace.Types.Add(CodeTypeDeclaration);
            CodeTypeDeclaration.AppendCodeTypeAttribute<XmlFileAttribute>(XmlElement);
            var constructor = new CodeConstructor
            {
                Attributes = MemberAttributes.Public
            };
            CodeTypeDeclaration.Members.Add(constructor);

            if (XmlElement.HasElements)
            {
                var xElement = XmlElement.Elements().Single();
                var codeMemberField = new CodeMemberField()
                {
                    Name = xElement.Name.LocalName,
                    Attributes = MemberAttributes.Public,
                };
                CodeTypeDeclaration.Members.Add(codeMemberField);

                var contentGenerator = new ContentGenerator(xElement, CodeNamespace);
                codeMemberField.Type = new CodeTypeReference(contentGenerator.generatedTypeName);
                constructor.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"{codeMemberField.Name} = new {contentGenerator.generatedTypeName}()")));
                codeMemberField.AppendCodeTypeAttribute<XmlElementSingleAttribute>(xElement);
            }
            else
            {
                var defaultGenerator = new DefaultGenerator(XmlElement, CodeNamespace);
                var codeMemberField = new CodeMemberField()
                {
                    Name = XmlFieldNameEnum.root.ToString(),
                    Attributes = MemberAttributes.Public,
                    Type = new CodeTypeReference($@"{defaultGenerator.generatedTypeName}"),
                };
                CodeTypeDeclaration.Members.Add(codeMemberField);
                constructor.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));
                var codeAttributeDeclaration = new CodeAttributeDeclaration(nameof(XmlElementDefaultAttribute));
                codeMemberField.CustomAttributes.Add(codeAttributeDeclaration);
            }
        }
    }
}