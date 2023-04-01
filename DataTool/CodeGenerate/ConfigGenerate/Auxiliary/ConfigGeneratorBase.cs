using System;
using System.CodeDom;
using System.Xml.Linq;

namespace DataTool.CodeGenerate.ConfigGenerate.Auxiliary
{
    public class ConfigGeneratorBase
    {
        protected readonly XElement XmlElement;
        protected readonly CodeNamespace CodeNamespace;
        
        protected CodeTypeDeclaration CodeTypeDeclaration;
        public string generatedTypeName => CodeTypeDeclaration?.Name;

        protected ConfigGeneratorBase(XElement paramXmlElement,CodeNamespace paramCodeNamespace)
        {
            if (paramXmlElement == null) throw new ArgumentNullException(nameof(paramXmlElement));
            if (paramCodeNamespace == null) throw new ArgumentNullException(nameof(paramCodeNamespace));
            
            XmlElement = paramXmlElement;
            CodeNamespace = paramCodeNamespace;
        }
    }
}