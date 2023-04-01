using System.CodeDom;
using System.Xml.Linq;
using DataTool.CodeGenerate.Auxiliary;
using DataTool.CodeGenerate.FileGenerate.BinaryGenerate.ContentGenerate;
using DataToolInterface.Format.Config.Model;
using DataToolInterface.Format.File;
using DataToolInterface.Format.File.Binary;

namespace DataTool.CodeGenerate.FileGenerate.BinaryGenerate
{
    [GenerateFileFormat(format = FileFormatEnum.Binary)]
    [GenerateFileSubFormat(format = FileFormatEnum.Binary,subFormat = nameof(BinarySubFormatEnum.Binary))]
    public sealed class BinaryGenerator:FileGenerator
    {
        public BinaryGenerator(XElement paramXmlElement, CodeNamespace paramCodeNamespace) : base(paramXmlElement, paramCodeNamespace)
        {
            GenerateBinary();
        }

        private  void GenerateBinary()
        {
            var contentGenerator = new ContentGenerator(XmlElement, CodeNamespace);
            CodeTypeDeclaration = contentGenerator.CodeTypeDeclaration;
            
            if (XmlElement.Attribute(nameof(ModelStructElementAttribute.subFormat))==null)
            {
                CodeTypeDeclaration.AppendCodeTypeAttribute<BinaryFileAttribute>(XmlElement);
            }
        }
    }
}