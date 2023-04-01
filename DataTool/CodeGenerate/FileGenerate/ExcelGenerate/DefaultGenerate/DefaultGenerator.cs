using System.CodeDom;
using System.Reflection;
using System.Xml.Linq;

namespace DataTool.CodeGenerate.FileGenerate.ExcelGenerate.DefaultGenerate
{
    public class DefaultGenerator:FileGenerator
    {
        public DefaultGenerator(XElement paramXmlElement, CodeNamespace paramCodeNamespace) : base(paramXmlElement, paramCodeNamespace)
        {
            GenerateDefault();
        }

        private void GenerateDefault()
        {
            CodeTypeDeclaration = new CodeTypeDeclaration
            {
                Name = @"DataTable",
                IsClass = true,
                TypeAttributes = TypeAttributes.Public
            };
        }
    }
}