using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using DataTool.FormatAccess.Auxiliary;
using DataTool.FormatAccess.XmlAccess.XmlWrite.AttributeWrite;
using DataTool.FormatAccess.XmlAccess.XmlWrite.ContentWrite;
using DataTool.FormatAccess.XmlAccess.XmlWrite.DefaultWrite;
using DataToolInterface.Format.File;
using DataToolInterface.Format.File.Xml;

namespace DataTool.FormatAccess.XmlAccess.XmlWrite
{
    [FormatAccess(format = FileFormatEnum.Xml)]
    public class XmlWriter:FormatWriter
    {
        public XmlWriter(object paramObject,FileInfo paramFileInfo):base(paramObject,paramFileInfo)
        {
            WriteXml();
        }

        private void WriteXml()
        {
            var xmlDocument = new XmlDocument();
            XmlNode xmlNode = xmlDocument.CreateXmlDeclaration("1.0", "utf-8", "");
            xmlDocument.AppendChild(xmlNode);

            var rootField = Object.GetType().GetFields().Single();
            var root = xmlDocument.CreateElement(rootField.Name);
            xmlDocument.AppendChild(root);

            var target = rootField.GetValue(Object);
            
            foreach (var fieldInfo in Object.GetType().GetFields())
            {
                if (fieldInfo.GetCustomAttribute<XmlElementSingleAttribute>() is XmlElementSingleAttribute _)
                {
                    var _ = new ContentWriter(fieldInfo.GetValue(Object), root);
                }
                else if (fieldInfo.GetCustomAttribute<XmlElementDefaultAttribute>() is XmlElementDefaultAttribute _)
                {
                    var _ = new DefaultWriter(fieldInfo.GetValue(Object), root);
                }
                else
                {
                    throw new Exception("code has bug");
                }
            }

            xmlDocument.Save(FileInfo.FullName);
        }
    }
}