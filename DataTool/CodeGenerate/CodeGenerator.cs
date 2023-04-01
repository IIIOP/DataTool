using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using DataTool.CodeGenerate.Auxiliary;
using DataTool.CodeGenerate.ConfigGenerate;
using DataTool.CodeGenerate.ConfigGenerate.ModelGenerate;
using DataToolInterface.Format.Config;
using DataToolInterface.Format.Config.Referenced;
using DataToolLog;

namespace DataTool.CodeGenerate
{
    public class CodeGenerator
    {
        private readonly DirectoryInfo _directoryInfo;
        private readonly XElement _rootElement;

        private readonly CodeCompileUnit _codeCompileUnit;
        private readonly CodeDomProvider _codeDomProvider;
        private readonly CodeGeneratorOptions _codeGeneratorOptions;
        private readonly CodeNamespace _codeNamespace;
        private readonly CodeTypeDeclaration _codeTypeDeclaration;
        private readonly CodeConstructor _codeConstructor;
        private Assembly _generatedAssembly;

        public static readonly CodeNamespace DefaultCodeNamespace;

        private readonly string _outputFileName;
        private readonly string _scriptDemo;

        public static Assembly GetGeneratedAssemblyByDirectory(DirectoryInfo paramDirectoryInfo)
        {
            if (paramDirectoryInfo == null) throw new ArgumentNullException(nameof(paramDirectoryInfo));
            if (!paramDirectoryInfo.Exists) throw new Exception($@"目录[{paramDirectoryInfo.FullName}]不存在");

            var codeGenerator = new CodeGenerator(paramDirectoryInfo);
            var assembly = codeGenerator._generatedAssembly;
            return assembly;
        }

        public static Assembly GetGeneratedAssemblyByZipFile(FileInfo paramFileInfo)
        {
            if (paramFileInfo == null) throw new ArgumentNullException(nameof(paramFileInfo));
            if (!paramFileInfo.Exists) throw new Exception($@"文件[{paramFileInfo.FullName}]不存在");

            var directoryInfo = new DirectoryInfo("./TempDataToolFile/" + paramFileInfo.Name);
            if (directoryInfo.Exists)
            {
                directoryInfo.Delete(true);
            }

            ZipFile.ExtractToDirectory(paramFileInfo.FullName, directoryInfo.FullName);

            var codeGenerator = new CodeGenerator(directoryInfo);
            var assembly = codeGenerator._generatedAssembly;

            return assembly;
        }
        
        static CodeGenerator()
        {
            var codeCompileUnit = new CodeCompileUnit();
            DefaultCodeNamespace = new CodeNamespace(@"DefaultConfig");
            codeCompileUnit.Namespaces.Add(DefaultCodeNamespace);
            var codeDomProvider = CodeDomProvider.CreateProvider("CSharp");
            var codeGeneratorOptions = new CodeGeneratorOptions
            {
                BracingStyle = "C",
                BlankLinesBetweenMembers = false
            };
            DefaultCodeNamespace.Imports.Add(new CodeNamespaceImport("System"));
            DefaultCodeNamespace.Imports.Add(new CodeNamespaceImport("System.Data"));
            DefaultCodeNamespace.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
            DefaultCodeNamespace.Imports.Add(new CodeNamespaceImport("DataToolInterface.Format.Config.Global"));
            DefaultCodeNamespace.Imports.Add(new CodeNamespaceImport("DataToolInterface.Format.Config.Input"));
            DefaultCodeNamespace.Imports.Add(new CodeNamespaceImport("DataToolInterface.Format.Config.Model"));
            DefaultCodeNamespace.Imports.Add(new CodeNamespaceImport("DataToolInterface.Format.Config.Output"));
            DefaultCodeNamespace.Imports.Add(new CodeNamespaceImport("DataToolInterface.Format.Config"));
            DefaultCodeNamespace.Imports.Add(new CodeNamespaceImport("DataToolInterface.Format.File.Binary"));
            DefaultCodeNamespace.Imports.Add(new CodeNamespaceImport("DataToolInterface.Format.File.CSharp"));
            DefaultCodeNamespace.Imports.Add(new CodeNamespaceImport("DataToolInterface.Format.File.Excel"));
            DefaultCodeNamespace.Imports.Add(new CodeNamespaceImport("DataToolInterface.Format.File.Ini"));
            DefaultCodeNamespace.Imports.Add(new CodeNamespaceImport("DataToolInterface.Format.File.Lua"));
            DefaultCodeNamespace.Imports.Add(new CodeNamespaceImport("DataToolInterface.Format.File.Xml"));
            DefaultCodeNamespace.Imports.Add(new CodeNamespaceImport("DataToolInterface.Format.File"));
            DefaultCodeNamespace.Imports.Add(new CodeNamespaceImport("DataToolInterface.Format.Script"));
            DefaultCodeNamespace.Imports.Add(new CodeNamespaceImport("DataToolInterface.Auxiliary"));
                
            var defaultFiles = Directory.GetFiles(Path.Combine(Environment.CurrentDirectory,CodeGenerateEnums.DefaultConfig.ToString())).Where(p=>p.EndsWith(@".xml"));
            foreach (var defaultFile in defaultFiles)
            {
                var rootElement = XDocument.Load(defaultFile).Root;
                var _ = new ModelGenerator(rootElement, DefaultCodeNamespace);
            }

            var csPath = Path.Combine(Environment.CurrentDirectory, CodeGenerateEnums.DefaultConfig.ToString(), $@"{CodeGenerateEnums.DefaultConfig.ToString()}.cs");
            var dllPath = Path.Combine(Environment.CurrentDirectory, CodeGenerateEnums.DefaultConfig.ToString(), $@"{CodeGenerateEnums.DefaultConfig.ToString()}.dll");
            using (var streamWriter = new StreamWriter(csPath))
            {
                codeDomProvider.GenerateCodeFromCompileUnit(codeCompileUnit, streamWriter, codeGeneratorOptions);
            }

            var reference = new StringBuilder(Path.Combine(Environment.CurrentDirectory,"DataToolInterface.dll").FormatToCommandLineArg());
            reference.Append(','+Path.Combine(Environment.CurrentDirectory,"DataToolLog.dll").FormatToCommandLineArg());
            CommandLineHelper.CallCommandLine($@"csc /t:library /out:{dllPath.FormatToCommandLineArg()} /reference:{reference} {csPath.FormatToCommandLineArg()}");

            if (File.Exists(dllPath))
            {
                var directoryInfo = new DirectoryInfo(Environment.CurrentDirectory).Parent?.Parent;
                if (directoryInfo!=null)
                {
                    var directoryInfos = directoryInfo.GetDirectories(CodeGenerateEnums.DefaultConfig.ToString());
                    if (directoryInfos.Length==1)
                    {
                        File.Copy(dllPath,$@"{Path.Combine(directoryInfos.First().FullName,CodeGenerateEnums.DefaultConfig.ToString())}.dll",true);
                    }
                }
            }
        }
        

        private CodeGenerator(DirectoryInfo paramDirectoryInfo)
        {
            if (paramDirectoryInfo == null) throw new ArgumentNullException(nameof(paramDirectoryInfo));
            if (!paramDirectoryInfo.Exists) throw new Exception($@"目录[{paramDirectoryInfo.FullName}]不存在");
            if (paramDirectoryInfo.EnumerateDirectories().All(p => p.Name != CodeGenerateEnums.Config.ToString()))
                throw new Exception($@"在目录{paramDirectoryInfo.FullName}下，未找到Config文件夹");
            
            _directoryInfo = paramDirectoryInfo;
            var configFile = _directoryInfo.GetDirectories()
                .Single(p => p.Name == CodeGenerateEnums.Config.ToString())
                .GetFiles("*.xml", SearchOption.TopDirectoryOnly).Single();
            _rootElement = XDocument.Load(configFile.OpenRead()).Root;
            
            _generatedAssembly = null;
            
            _codeCompileUnit = new CodeCompileUnit();

            var name = $"{_rootElement.Attribute(nameof(DataToolAttribute.project))?.Value}_{_rootElement.Attribute(nameof(DataToolAttribute.name))?.Value}_{_rootElement.Attribute(nameof(DataToolAttribute.author))?.Value}";
            _codeNamespace = new CodeNamespace(name);
            _codeCompileUnit.Namespaces.Add(_codeNamespace);

            _outputFileName = $@"{name}.cs";

            _codeDomProvider = CodeDomProvider.CreateProvider("CSharp");

            _codeGeneratorOptions = new CodeGeneratorOptions
            {
                BracingStyle = "C",
                BlankLinesBetweenMembers = false
            };

            _codeTypeDeclaration = new CodeTypeDeclaration
            {
                Name = _rootElement.Attribute(nameof(DataToolAttribute.author))?.Value ?? "ChenJilian",
                IsClass = true,
                IsPartial = true,
                TypeAttributes = TypeAttributes.Public
            };
            _codeNamespace.Types.Add(_codeTypeDeclaration);

            using (var stringWriter = new StringWriter())
            {
                _codeDomProvider.GenerateCodeFromCompileUnit(_codeCompileUnit, stringWriter, _codeGeneratorOptions);
                _scriptDemo = stringWriter.ToString();
            }

            _codeConstructor = new CodeConstructor
            {
                Attributes = MemberAttributes.Public
            };
            _codeTypeDeclaration.Members.Add(_codeConstructor);

            _codeTypeDeclaration.AppendCodeTypeAttribute<DataToolAttribute>(_rootElement);

            _codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
            _codeNamespace.Imports.Add(new CodeNamespaceImport("System.Data"));
            _codeNamespace.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
            _codeNamespace.Imports.Add(new CodeNamespaceImport("DataToolInterface.Format.Config.Global"));
            _codeNamespace.Imports.Add(new CodeNamespaceImport("DataToolInterface.Format.Config.Input"));
            _codeNamespace.Imports.Add(new CodeNamespaceImport("DataToolInterface.Format.Config.Model"));
            _codeNamespace.Imports.Add(new CodeNamespaceImport("DataToolInterface.Format.Config.Output"));
            _codeNamespace.Imports.Add(new CodeNamespaceImport("DataToolInterface.Format.Config"));
            _codeNamespace.Imports.Add(new CodeNamespaceImport("DataToolInterface.Format.File.Binary"));
            _codeNamespace.Imports.Add(new CodeNamespaceImport("DataToolInterface.Format.File.CSharp"));
            _codeNamespace.Imports.Add(new CodeNamespaceImport("DataToolInterface.Format.File.Excel"));
            _codeNamespace.Imports.Add(new CodeNamespaceImport("DataToolInterface.Format.File.Ini"));
            _codeNamespace.Imports.Add(new CodeNamespaceImport("DataToolInterface.Format.File.Lua"));
            _codeNamespace.Imports.Add(new CodeNamespaceImport("DataToolInterface.Format.File.Xml"));
            _codeNamespace.Imports.Add(new CodeNamespaceImport("DataToolInterface.Format.File"));
            _codeNamespace.Imports.Add(new CodeNamespaceImport("DataToolInterface.Format.Script"));
            _codeNamespace.Imports.Add(new CodeNamespaceImport("DataToolInterface.Auxiliary"));
            _codeNamespace.Imports.Add(new CodeNamespaceImport("DefaultConfig"));
            
            LogHelper.DefaultLog.WriteLine($@"开始读取配置文件与脚本 [{configFile.FullName}]");
            
            GenerateCodeFile();
            GenerateAssembly();
        }

        private void GenerateCodeFile()
        {
            var configGenerator = new ConfigGenerator(_rootElement, _codeNamespace, _codeTypeDeclaration, _codeConstructor);
            
            var targetDirectory = new DirectoryInfo(Path.Combine(_directoryInfo.FullName, CodeGenerateEnums.ScriptEditor.ToString()));
            var sourceDirectory = new DirectoryInfo(CodeGenerateEnums.ScriptEditor.ToString());
            if (!targetDirectory.Exists)
            {
                targetDirectory.Create();
                targetDirectory.Refresh();
                targetDirectory.CreateSubdirectory(CodeGenerateEnums.Library.ToString());
                targetDirectory.CopyFromDirectory(sourceDirectory,true);
                var subDirectory = targetDirectory.CreateSubdirectory(CodeGenerateEnums.Scripts.ToString());
                
                using (var streamWriter = new StreamWriter(Path.Combine(subDirectory.FullName, $@"{CodeGenerateEnums.ScriptDemo}.cs")))
                {
                    streamWriter.Write(_scriptDemo);
                }
            }
            else
            {
                var subDirectory = targetDirectory.GetDirectories().Single(p => p.Name == CodeGenerateEnums.Library.ToString());
                foreach (var file in subDirectory.GetFiles())
                {
                    file.Delete();
                }

                subDirectory = targetDirectory.GetDirectories().Single(p => p.Name == CodeGenerateEnums.DataTool.ToString());
                sourceDirectory = sourceDirectory.GetDirectories().Single(p => p.Name == CodeGenerateEnums.DataTool.ToString());
                subDirectory.CopyFromDirectory(sourceDirectory,true);
            }


            using (var streamWriter = new StreamWriter(Path.Combine(targetDirectory.FullName, CodeGenerateEnums.Library.ToString(), _outputFileName)))
            {
                _codeDomProvider.GenerateCodeFromCompileUnit(_codeCompileUnit, streamWriter, _codeGeneratorOptions);
                LogHelper.DefaultLog.WriteLine($@"成功生成Code");
            }

            var fileStream = new FileStream(Path.Combine(targetDirectory.FullName, $@"{CodeGenerateEnums.ScriptEditor}.csproj"), FileMode.Open);
            var csproj = XDocument.Load(fileStream);
            var csElements = csproj.Root?.Elements().Where(i => i.Name.ToString().EndsWith("ItemGroup"))
                .Single(p => p.Elements().First().Name.ToString().EndsWith("Compile"));
            XElement targetElement = null;
            foreach (var csElement in csElements.Elements())
            {
                if (csElement.Attribute("Include").Value.StartsWith("Library"))
                {
                    targetElement = csElement;
                }
            }

            if (targetElement == null)
            {
                var element = new XElement(csElements.Elements().First());
                element.SetAttributeValue("Include", $@"Library\{_outputFileName}");
                csElements.Add(element);
            }

            fileStream.Seek(0, SeekOrigin.Begin);
            fileStream.SetLength(0);
            csproj.Save(fileStream);
            fileStream.Close();
        }


        private void GenerateAssembly()
        {
            var filePath = $@"{Path.Combine(_directoryInfo.FullName, CodeGenerateEnums.ScriptEditor.ToString(),CodeGenerateEnums.Library.ToString(), _codeNamespace.Name)}.dll";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            
            var reference = new StringBuilder(Path.Combine(Environment.CurrentDirectory,"DataToolInterface.dll").FormatToCommandLineArg());
            reference.Append(','+Path.Combine(Environment.CurrentDirectory,"DataToolLog.dll").FormatToCommandLineArg());

            if (_rootElement.Element(ReferencedEnum.Referenced.ToString()) is XElement referenced)
            {
                foreach (var element in referenced.Elements(ReferencedElementEnum.Assembly.ToString()))
                {
                    reference.Append(',' + Path.Combine(_directoryInfo.FullName, element.Value).FormatToCommandLineArg());
                }
            }
            
            reference.Append(',' + Path.Combine(Environment.CurrentDirectory,CodeGenerateEnums.DefaultConfig.ToString(),$@"{CodeGenerateEnums.DefaultConfig.ToString()}.dll").FormatToCommandLineArg());

            var fileString = new StringBuilder();
            var scriptDirectory = new DirectoryInfo(Path.Combine(_directoryInfo.FullName,CodeGenerateEnums.ScriptEditor.ToString(),CodeGenerateEnums.Library.ToString()));
            var files = scriptDirectory.GetFiles("*.cs", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                fileString.Append($@"{file.FullName.FormatToCommandLineArg()} ");
            }

            scriptDirectory = new DirectoryInfo(Path.Combine(_directoryInfo.FullName,CodeGenerateEnums.ScriptEditor.ToString(), CodeGenerateEnums.Scripts.ToString()));
            files = scriptDirectory.GetFiles("*.cs", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                fileString.Append($@"{file.FullName.FormatToCommandLineArg()} ");
            }

            CommandLineHelper.CallCommandLine($@"csc /t:library /out:{filePath.FormatToCommandLineArg()} /reference:{reference} {fileString.ToString().TrimEnd()}");

            if (File.Exists(filePath))
            {
                _generatedAssembly = Assembly.LoadFile(filePath);
                LogHelper.DefaultLog.WriteLine($@"成功生成Assembly");
            }
            else
            {
                throw new Exception("代码生成失败");
            }
        }
    }
}