using System;
using System.CodeDom;
using System.Xml.Linq;
using DataTool.CodeGenerate.Auxiliary;
using DataTool.CodeGenerate.ConfigGenerate.GlobalGenerate;
using DataTool.CodeGenerate.ConfigGenerate.InputGenerate;
using DataTool.CodeGenerate.ConfigGenerate.ModelGenerate;
using DataTool.CodeGenerate.ConfigGenerate.OutputGenerate;
using DataToolInterface.Format.Config.Global;
using DataToolInterface.Format.Config.Input;
using DataToolInterface.Format.Config.Model;
using DataToolInterface.Format.Config.Output;

namespace DataTool.CodeGenerate.ConfigGenerate
{
    public class ConfigGenerator
    {
        private readonly XElement _xmlElement;
        private readonly CodeNamespace _codeNamespace;
        private readonly CodeTypeDeclaration _baseCodeTypeDeclaration;
        private readonly CodeConstructor _baseCodeConstructor;

        public ConfigGenerator(XElement paramXmlElement,CodeNamespace paramCodeNamespace,CodeTypeDeclaration paramBaseCodeTypeDeclaration,CodeConstructor paramBaseCodeConstructor)
        {
            if (paramXmlElement == null) throw new ArgumentNullException(nameof(paramXmlElement));
            if (paramCodeNamespace == null) throw new ArgumentNullException(nameof(paramCodeNamespace));
            if (paramBaseCodeTypeDeclaration == null) throw new ArgumentNullException(nameof(paramBaseCodeTypeDeclaration));
            if (paramBaseCodeConstructor == null) throw new ArgumentNullException(nameof(paramBaseCodeConstructor));
            
            _xmlElement = paramXmlElement;
            _codeNamespace = paramCodeNamespace;
            _baseCodeTypeDeclaration = paramBaseCodeTypeDeclaration;
            _baseCodeConstructor = paramBaseCodeConstructor;

            GenerateConfig();
        }

        private void GenerateConfig()
        {
            GenerateConfig_Global();
            GenerateConfig_Model();
            GenerateConfig_Input();
            GenerateConfig_Output();
        }
        
        private void GenerateConfig_Global()
        {
            var globalElement = _xmlElement.Element(GlobalEnum.Global.ToString());
            if (globalElement!=null)
            {
                var globalGenerator = new GlobalGenerator(globalElement, _codeNamespace);
                var codeMemberField = new CodeMemberField
                {
                    Name = globalElement.Name.LocalName,
                    Attributes =  MemberAttributes.Public | MemberAttributes.Static,
                    Type = new CodeTypeReference(globalGenerator.generatedTypeName),
                };
                _baseCodeTypeDeclaration.Members.Add(codeMemberField);
                codeMemberField.InitExpression = new CodeSnippetExpression($"new {codeMemberField.Type.BaseType}()");
            }
        }
        
        private void GenerateConfig_Model()
        {
            var modelElement = _xmlElement.Element(ModelEnum.Model.ToString());
            if (modelElement!=null)
            {
                var modelGenerator = new ModelGenerator(modelElement, _codeNamespace);
            }
        }
        private void GenerateConfig_Input()
        {
            var inputElement = _xmlElement.Element(InputEnum.Input.ToString());
            if (inputElement!=null)
            {
                var inputGenerator = new InputGenerator(inputElement, _codeNamespace);
                var codeMemberField = new CodeMemberField
                {
                    Name = inputElement.Name.LocalName,
                    Attributes =  MemberAttributes.Public,
                    Type = new CodeTypeReference(inputGenerator.generatedTypeName),
                };
                _baseCodeTypeDeclaration.Members.Add(codeMemberField);
                _baseCodeConstructor.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));
            }
        }
        private void GenerateConfig_Output()
        {
            var outputElement = _xmlElement.Element(OutputEnum.Output.ToString());
            if (outputElement!=null)
            {
                var outputGenerator = new OutputGenerator(outputElement, _codeNamespace);
                var codeMemberField = new CodeMemberField
                {
                    Name = outputElement.Name.LocalName,
                    Attributes =  MemberAttributes.Public,
                    Type = new CodeTypeReference(outputGenerator.generatedTypeName),
                };
                _baseCodeTypeDeclaration.Members.Add(codeMemberField);
                _baseCodeConstructor.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));
            }
        }
    }
}