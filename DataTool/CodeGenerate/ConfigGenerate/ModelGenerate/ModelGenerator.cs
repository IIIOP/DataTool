using System;
using System.CodeDom;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using DataTool.CodeGenerate.Auxiliary;
using DataTool.CodeGenerate.ConfigGenerate.Auxiliary;
using DataTool.CodeGenerate.FileGenerate;
using DataToolInterface.Format.Config.Model;
using DataToolInterface.Format.File;

namespace DataTool.CodeGenerate.ConfigGenerate.ModelGenerate
{
    public class ModelGenerator:ConfigGeneratorBase
    {
        public ModelGenerator(XElement paramXmlElement,CodeNamespace paramCodeNamespace):base(paramXmlElement, paramCodeNamespace)
        {
            if (paramXmlElement.Name.LocalName!=ModelEnum.Model.ToString())
                throw new Exception("输入element必须为 Model!");
            
            GenerateModel();
        }

        private void GenerateModel()
        {
            var enumElement = XmlElement.Element(ModelElementEnum.Enum.ToString());
            if (enumElement!=null)
            {
                foreach (var element in enumElement.Elements())
                {
                    var codeTypeDeclaration = new CodeTypeDeclaration()
                    {
                        Name = element.Name.LocalName,
                        IsEnum = true,
                        TypeAttributes = TypeAttributes.Public
                    };
                    CodeNamespace.Types.Add(codeTypeDeclaration);

                    foreach (var subElement in element.Elements())
                    {
                        string enumName;
                        var attributeName = subElement.Attribute(nameof(ModelEnumElementItemAttribute.value))?.Value;
                        if (attributeName == null)
                            enumName = subElement.Name.LocalName;
                        else
                            enumName = subElement.Name + " = " + attributeName;

                        var codeMemberField = new CodeMemberField(typeof(Enum), enumName);
                        codeMemberField.AppendCodeTypeAttribute<ModelEnumElementItemAttribute>(subElement);
                        codeTypeDeclaration.Members.Add(codeMemberField);
                    }
                }
            }
            
            var structElement = XmlElement.Element(ModelElementEnum.Struct.ToString());
            if (structElement!=null)
            {
                foreach (var element in structElement.Elements())
                {
                    if (!CodeNamespace.HasCodeTypeDeclarationOfName(element.Name.LocalName))
                    {
                        var _ = FileGenerator.GenerateSubFile(element, CodeNamespace);
                    }
                    else
                    {
                        throw new Exception("struct type repeat");
                    }
                }
            }
        }
    }
}