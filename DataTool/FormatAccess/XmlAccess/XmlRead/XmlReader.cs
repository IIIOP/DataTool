using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using DataTool.FormatAccess.Auxiliary;
using DataTool.FormatAccess.XmlAccess.XmlRead.ContentRead;
using DataTool.FormatAccess.XmlAccess.XmlRead.DefaultRead;
using DataToolInterface.Format.File;
using DataToolInterface.Format.File.Xml;
using DataToolLog;

namespace DataTool.FormatAccess.XmlAccess.XmlRead
{
    [FormatAccess(format = FileFormatEnum.Xml)]
    public class XmlReader:FormatReader
    {
        public XmlReader(Type paramType,FileInfo paramFileInfo):base(paramType,paramFileInfo)
        {
            ReadXml();
        }

        private void ReadXml()
        {
            LogHelper.DefaultLog.WriteLine($@">>> Read Xml [{FileInfo.FullName}]");

            if (XDocument.Load(FileInfo.FullName).Root is XElement rootElement)
            {
                var field = Type.GetFields().Single();
                if (field.GetCustomAttribute<XmlElementSingleAttribute>() is XmlElementSingleAttribute singleAttribute)
                {
                    var contentReader = new ContentReader(field.FieldType, rootElement);
                    field.SetValue(resultObject,contentReader.resultObject);
                }
                else if(field.GetCustomAttribute<XmlElementDefaultAttribute>() is XmlElementDefaultAttribute defaultAttribute)
                {
                    var defaultReader = new DefaultReader(field.FieldType, rootElement);
                    field.SetValue(resultObject,defaultReader.resultObject);
                }
            }
            else
            {
                throw new Exception($@"<{GetType().Name}> Xml root read fail!!");
            }
        }
    }
}