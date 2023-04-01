using System;
using System.CodeDom;
using System.Reflection;
using System.Xml.Linq;
using DataTool.CodeGenerate.Auxiliary;
using DataToolInterface.Data;
using DataToolInterface.Format.Config.Input;
using DataToolInterface.Format.File;
using DataToolInterface.Format.File.Xml;

namespace DataTool.CodeGenerate.FileGenerate.XmlGenerate.AttributeGenerate
{
    public class AttributeGenerator:FileGenerator
    {
        public AttributeGenerator(XElement paramXmlElement, CodeNamespace paramCodeNamespace) : base(paramXmlElement, paramCodeNamespace)
        {
            GenerateAttribute();
        }
        
        private void GenerateAttribute()
        {
            CodeTypeDeclaration = new CodeTypeDeclaration
            {
                Name = CodeNamespace.GetUniqueNameByString(XmlFieldNameEnum.attribute.ToString()),
                IsClass = true,
                TypeAttributes = TypeAttributes.Public
            };
            CodeNamespace.Types.Add(CodeTypeDeclaration);
            
            var constructor = new CodeConstructor
            {
                Attributes = MemberAttributes.Public
            };
            CodeTypeDeclaration.Members.Add(constructor);
            
            foreach (var xAttribute in XmlElement.Attributes())
            {
                string typeValue;
                if (typeof(InputFileAttribute).GetProperty(xAttribute.Name.LocalName)!=null||
                    typeof(FileFormatAttribute).GetProperty(xAttribute.Name.LocalName)!=null)
                {
                    var typeValues = xAttribute.Value.Split('|');
                    if (typeValues.Length>1)
                    {
                        typeValue = typeValues[1];
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    typeValue = xAttribute.Value;
                }

                if (Enum.TryParse(typeValue.BaseType(),out BasicDataTypeEnum _))
                {
                    var codeMemberField = new CodeMemberField()
                    {
                        Name = xAttribute.Name.LocalName,
                        Attributes = MemberAttributes.Public,
                    };
                    CodeTypeDeclaration.Members.Add(codeMemberField);

                    if (typeValue.IsArray())
                    {
                        codeMemberField.Type = new CodeTypeReference($@"List<{typeValue.BaseType().AsCodeType()}>");
                        constructor.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($@"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));
                    }
                    else
                    {
                        codeMemberField.Type = new CodeTypeReference($@"{typeValue.AsCodeType()}");
                    }
                        
                    var codeAttributeDeclaration = new CodeAttributeDeclaration(nameof(XmlAttributeAttribute));
                    codeMemberField.CustomAttributes.Add(codeAttributeDeclaration);
                    codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument(nameof(XmlAttributeAttribute.type), new CodeSnippetExpression("@\"" + typeValue + "\"")));
                }
                else
                {
                    throw new Exception($@"<{GetType().Name}> attribute data type can only be basic!!");
                }
            }
        }
    }
}