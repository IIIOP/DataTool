using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace DataToolInterface.Methods
{
    public static class SerializeHelper
    {
        public static void Serialize(this object paramObject,FileInfo paramFileInfo)
        {
            if (paramObject == null) throw new ArgumentNullException(nameof(paramObject));
            if (paramFileInfo == null) throw new ArgumentNullException(nameof(paramFileInfo));

            if (!paramFileInfo.Directory.Exists)
            {
                paramFileInfo.Directory.Create();
            }

            using (XmlWriter stream = new XmlTextWriter(paramFileInfo.FullName,Encoding.UTF8))
            {
                var xml = new XmlSerializer(paramObject.GetType());
                xml.Serialize(stream,paramObject);
            }
        }
    }
}