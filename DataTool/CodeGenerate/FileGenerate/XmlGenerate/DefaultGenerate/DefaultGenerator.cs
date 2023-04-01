using System.CodeDom;
using System.Reflection;
using System.Xml.Linq;
using DataTool.CodeGenerate.Auxiliary;
using DataToolInterface.Format.File.Xml;

namespace DataTool.CodeGenerate.FileGenerate.XmlGenerate.DefaultGenerate
{
    public class DefaultGenerator:FileGenerator
    {
        public DefaultGenerator(XElement paramXmlElement, CodeNamespace paramCodeNamespace) : base(paramXmlElement, paramCodeNamespace)
        {
            GenerateDefault();
        }

        private void GenerateDefault()
        {
            if (CodeNamespace.HasCodeTypeDeclarationOfName(XmlTypeNameEnum.XmlDefaultType.ToString()))
            {
                CodeTypeDeclaration = CodeNamespace.GetCodeTypeDeclarationByName(XmlTypeNameEnum.XmlDefaultType.ToString());
            }
            else 
            {
                CodeTypeDeclaration = new CodeTypeDeclaration
                {
                    Name = XmlTypeNameEnum.XmlDefaultType.ToString(),
                    IsClass = true,
                    TypeAttributes = TypeAttributes.Public
                };
                CodeNamespace.Types.Add(CodeTypeDeclaration);

                var constructor = new CodeConstructor
                {
                    Attributes = MemberAttributes.Public
                };
                CodeTypeDeclaration.Members.Add(constructor);
                
                var codeMemberField = new CodeMemberField()
                {
                    Name = XmlFieldNameEnum.name.ToString(),
                    Attributes = MemberAttributes.Public,
                    Type = new CodeTypeReference($@"String"),
                };
                CodeTypeDeclaration.Members.Add(codeMemberField);
                
                codeMemberField = new CodeMemberField()
                {
                    Name = XmlFieldNameEnum.attribute.ToString(),
                    Attributes = MemberAttributes.Public,
                    Type = new CodeTypeReference($@"XmlAttributeDictionary"),
                };
                CodeTypeDeclaration.Members.Add(codeMemberField);
            
                constructor.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($@"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));

                codeMemberField = new CodeMemberField()
                {
                    Name = XmlFieldNameEnum.content.ToString(),
                    Attributes = MemberAttributes.Public,
                    Type = new CodeTypeReference($@"String"),
                };
                CodeTypeDeclaration.Members.Add(codeMemberField);
                
                codeMemberField = new CodeMemberField()
                {
                    Name = XmlFieldNameEnum.elements.ToString(),
                    Attributes = MemberAttributes.Public,
                    Type = new CodeTypeReference($@"List<{CodeTypeDeclaration.Name}>"),
                };
                CodeTypeDeclaration.Members.Add(codeMemberField);
            
                constructor.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($@"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));
            }
        }
    }
}