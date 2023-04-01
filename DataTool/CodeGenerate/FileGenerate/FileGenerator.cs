using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using DataTool.CodeGenerate.Auxiliary;
using DataToolInterface.Data;
using DataToolInterface.Format.Config.Input;
using DataToolInterface.Format.Config.Model;
using DataToolInterface.Format.Config.Output;
using DataToolInterface.Format.File;

namespace DataTool.CodeGenerate.FileGenerate
{
    public abstract class FileGenerator
    {
        protected readonly XElement XmlElement;
        protected readonly CodeNamespace CodeNamespace;
        public CodeTypeDeclaration CodeTypeDeclaration;
        public string generatedTypeName => CodeTypeDeclaration?.Name;

        protected FileGenerator(XElement paramXmlElement,CodeNamespace paramCodeNamespace)
        {
            if (paramXmlElement == null) throw new ArgumentNullException(nameof(paramXmlElement));
            if (paramCodeNamespace == null) throw new ArgumentNullException(nameof(paramCodeNamespace));

            XmlElement = paramXmlElement;
            CodeNamespace = paramCodeNamespace;
            
        }

        public static FileGenerator GenerateFile(XElement paramXmlElement,CodeNamespace paramCodeNamespace)
        {
            if (paramXmlElement == null) throw new ArgumentNullException(nameof(paramXmlElement));
            if (paramCodeNamespace == null) throw new ArgumentNullException(nameof(paramCodeNamespace));

            FileGenerator retFileGenerator;
            if (paramXmlElement.Attribute(nameof(FileFormatAttribute.format)) is XAttribute attribute)
            {
                if (Enum.TryParse(attribute.Value,out FileFormatEnum fileFormatEnum))
                {
                    var type = typeof(FileGenerator).Assembly.GetTypes()
                        .Where(p => p.IsSubclassOf(typeof(FileGenerator)) && p.GetCustomAttribute<GenerateFileFormatAttribute>() != null)
                        .Single(p=>p.GetCustomAttribute<GenerateFileFormatAttribute>().format==fileFormatEnum);

                    if (Activator.CreateInstance(type, paramXmlElement, paramCodeNamespace) is FileGenerator fileGenerator)
                    {
                        retFileGenerator = fileGenerator;
                    }
                    else
                    {
                        throw new Exception("CreateInstance 失败");
                    }
                }
                else
                {
                    throw new Exception("配置文件 format 属性 暂不支持");
                }
            }
            else
            {
                throw new Exception("配置文件缺少 format 属性");
            }

            return retFileGenerator;
        }
        
        public static FileGenerator GenerateSubFile(XElement paramXmlElement,CodeNamespace paramCodeNamespace)
        {
            if (paramXmlElement == null) throw new ArgumentNullException(nameof(paramXmlElement));
            if (paramCodeNamespace == null) throw new ArgumentNullException(nameof(paramCodeNamespace));

            FileGenerator retFileGenerator;
            var format = paramXmlElement.Attribute(nameof(ModelStructElementAttribute.format));
            var subFormat = paramXmlElement.Attribute(nameof(ModelStructElementAttribute.subFormat));
            if (format!=null&&subFormat!=null)
            {
                if (Enum.TryParse(format.Value,out FileFormatEnum fileFormatEnum))
                {
                    var type = typeof(FileGenerator).Assembly.GetTypes()
                        .Where(p => p.IsSubclassOf(typeof(FileGenerator)) && p.GetCustomAttribute<GenerateFileSubFormatAttribute>() != null)
                        .SingleOrDefault(p => p.GetCustomAttribute<GenerateFileSubFormatAttribute>().format == fileFormatEnum&&p.GetCustomAttribute<GenerateFileSubFormatAttribute>().subFormat==subFormat.Value);
                    if (type!=null)
                    {
                        if (Activator.CreateInstance(type, paramXmlElement, paramCodeNamespace) is FileGenerator fileGenerator)
                        {
                            retFileGenerator = fileGenerator;
                        }
                        else
                        {
                            throw new Exception("实例化失败");
                        }
                    }
                    else
                    {
                        throw new Exception("Struct subformat no support");
                    }
                }
                else
                {
                    throw new Exception("Struct format no support");
                }
            }
            else
            {
                throw new Exception("Struct no format or subformat");
            }

            return retFileGenerator;
        }
    }
}