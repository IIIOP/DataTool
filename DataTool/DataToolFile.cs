using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using DataTool.CaseTest;
using DataTool.ConsistencyCheck;
using DataTool.InputRead;
using DataTool.InputWrite;
using DataTool.OutputGenerate;
using DataTool.OutputRead;
using DataTool.OutputWrite;
using DataToolInterface.Format.Config;
using DataToolInterface.Format.Config.Global;
using DataToolInterface.Format.Config.Input;
using DataToolInterface.Format.Config.Output;
using DataToolInterface.Methods;
using CodeGenerator = DataTool.CodeGenerate.CodeGenerator;

namespace DataTool
{
    public class DataToolFile
    {
        private readonly Assembly _assembly;
        
        private readonly object _object;
        
        private readonly DataToolAttribute _dataToolAttribute;
        public string inputPath { get; set; }
        public string outputPath { get; set; }
        // ReSharper disable once MemberCanBePrivate.Global
        public string project => _dataToolAttribute.project;
        // ReSharper disable once MemberCanBePrivate.Global
        public string name => _dataToolAttribute.name;
        // ReSharper disable once UnusedMember.Global
        public string version => _dataToolAttribute.version;
        // ReSharper disable once MemberCanBePrivate.Global
        public string author => _dataToolAttribute.author;
        
        public static DataToolFile GetDataToolFileByDirectory(DirectoryInfo paramDirectoryInfo)
        {
            return new DataToolFile(paramDirectoryInfo);
        }
        public static DataToolFile GetDataToolFileByZipFile(FileInfo paramFileInfo)
        {
            return new DataToolFile(paramFileInfo);
        }

        public static DataToolFile GetDataToolFileByAssembly(Assembly paramAssembly)
        {
            return new DataToolFile(paramAssembly);
        }
        
        private DataToolFile(DirectoryInfo paramDirectoryInfo)
        {
            if (paramDirectoryInfo == null) throw new ArgumentNullException(nameof(paramDirectoryInfo));
            
            if (paramDirectoryInfo.Exists)
            {
                _assembly = CodeGenerator.GetGeneratedAssemblyByDirectory(paramDirectoryInfo);
            
                _object = Activator.CreateInstance(_assembly.DefinedTypes.Single(item => item.GetCustomAttribute<DataToolAttribute>()!=null));
            
                _dataToolAttribute = _object.GetType().GetCustomAttribute<DataToolAttribute>();

                inputPath = _object.GetType().GetFields().Single(p => p.Name == InputEnum.Input.ToString()).FieldType.GetCustomAttribute<InputAttribute>().path;
            
                outputPath = _object.GetType().GetFields().Single(p => p.Name == OutputEnum.Output.ToString()).FieldType.GetCustomAttribute<OutputAttribute>().path;
            }
            else
            {
                throw new Exception($@"<{GetType().Name}> Directory [{paramDirectoryInfo.FullName}] not exist!!");
            }
        }
        
        private DataToolFile(FileInfo paramFileInfo)
        {
            if (paramFileInfo == null) throw new ArgumentNullException(nameof(paramFileInfo));

            if (paramFileInfo.Exists)
            {
                _assembly = CodeGenerator.GetGeneratedAssemblyByZipFile(paramFileInfo);
            
                _object = Activator.CreateInstance(_assembly.DefinedTypes.Single(item => item.GetCustomAttribute<DataToolAttribute>()!=null));
            
                _dataToolAttribute = _object.GetType().GetCustomAttribute<DataToolAttribute>();

                inputPath = _object.GetType().GetFields().Single(p => p.Name == InputEnum.Input.ToString()).FieldType.GetCustomAttribute<InputAttribute>().path;
            
                outputPath = _object.GetType().GetFields().Single(p => p.Name == OutputEnum.Output.ToString()).FieldType.GetCustomAttribute<OutputAttribute>().path;
            }
            else
            {
                throw new Exception($@"<{GetType().Name}> File [{paramFileInfo.FullName}] not exist!!");
            }
        }
        
        private DataToolFile(Assembly paramAssembly)
        {
            if (paramAssembly == null) throw new ArgumentNullException(nameof(paramAssembly));
            
            _assembly = paramAssembly;
            
            _object = Activator.CreateInstance(_assembly.DefinedTypes.Single(item => item.GetCustomAttribute<DataToolAttribute>()!=null));
            
            _dataToolAttribute = _object.GetType().GetCustomAttribute<DataToolAttribute>();

            inputPath = _object.GetType().GetFields().Single(p => p.Name == InputEnum.Input.ToString()).FieldType.GetCustomAttribute<InputAttribute>().path;
            
            outputPath = _object.GetType().GetFields().Single(p => p.Name == OutputEnum.Output.ToString()).FieldType.GetCustomAttribute<OutputAttribute>().path;
        }
        public Dictionary<string,(string,string)> GetRequiredVariable()
        {
            var dictionary = new Dictionary<string, (string,string)>()
            {
                ["输入文件路径"]=(nameof(inputPath),inputPath),
                ["输出文件路径"]=(nameof(outputPath),outputPath),
            };
            var field = _object.GetType().GetFields().Single(p => p.Name == GlobalEnum.Global.ToString());
            foreach (var fieldInfo in field.FieldType.GetFields())
            {
                if (fieldInfo.GetCustomAttribute<GlobalAttribute>() is GlobalAttribute globalAttribute)
                {
                    if (globalAttribute.isRequire!=null&&bool.TryParse(globalAttribute.isRequire,out var isRequire))
                    {
                        if (isRequire)
                        {
                            dictionary.Add(globalAttribute.describe??fieldInfo.Name,(fieldInfo.Name,fieldInfo.GetValue(field.GetValue(_object)).ToString()));
                        }
                    }
                }
                else
                {
                    Debug.Assert(false);
                }
            }

            return dictionary;
        }

        public void SetRequiredVariable(Dictionary<string, string> paramDictionary)
        {
            if (paramDictionary == null) throw new ArgumentNullException(nameof(paramDictionary));
            
            var global = _object.GetType().GetFields().Single(p => p.Name == GlobalEnum.Global.ToString())
                .GetValue(null);
            foreach (var item in paramDictionary)
            {
                if (item.Key==nameof(inputPath))
                {
                    inputPath = item.Value;
                }
                else if (item.Key==nameof(outputPath))
                {
                    outputPath = item.Value;
                }
                else
                {
                    global.GetType().GetFields().Single(p=>p.Name==item.Key).SetValue(global,item.Value);
                }
            }
        }
        
        

        public void ReadInput()
        {
            var field = _object.GetType().GetFields().Single(p => p.Name == InputEnum.Input.ToString());
            var inputReader = new InputReader(field.FieldType,new DirectoryInfo(Path.Combine(Environment.CurrentDirectory,inputPath??"")));
            field.SetValue(_object,inputReader.resultObject);
        }

        public void WriteInput()
        {
            var field = _object.GetType().GetFields().Single(p => p.Name == InputEnum.Input.ToString());
            var directoryInfo = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory,inputPath??""));
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
                directoryInfo.Refresh();
            }
            var inputWriter = new InputWriter(field.GetValue(_object),directoryInfo);
        }
        

        public void ReadOutput()
        {
            var field = _object.GetType().GetFields().Single(p => p.Name == OutputEnum.Output.ToString());
            var outputWriter = new OutputReader(field.FieldType,new DirectoryInfo(Path.Combine(Environment.CurrentDirectory,outputPath??"")));
            field.SetValue(_object,outputWriter.resultObject);
        }

        public void WriteOutput()
        {
            var field = _object.GetType().GetFields().Single(p => p.Name == OutputEnum.Output.ToString());
            var directoryInfo = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory,outputPath??""));
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
                directoryInfo.Refresh();
            }
            var outputWriter = new OutputWriter(field.GetValue(_object),directoryInfo);
        }

        public void TestCase()
        {
            var caseTester = new CaseTester(_object);
            //caseTester.ExportResultToExcel(new DirectoryInfo(!string.IsNullOrWhiteSpace(_assembly.Location)?_assembly.Location:Environment.CurrentDirectory));
        }
        
        public void CheckConsistency()
        {
            var consistencyChecker = new ConsistencyChecker(_object);
        }
        
        public void GenerateOutput()
        {
            var outputGenerator = new OutputGenerator(_object);
        }

        public void Serialize()
        {
            var directoryInfo = !string.IsNullOrWhiteSpace(_assembly.Location)?new FileInfo(_assembly.Location).Directory:new DirectoryInfo(Environment.CurrentDirectory);
            _object.Serialize(new FileInfo(Path.Combine(directoryInfo.FullName,$@"{project}_{name}_{author}.xml")));
        }
    }
}