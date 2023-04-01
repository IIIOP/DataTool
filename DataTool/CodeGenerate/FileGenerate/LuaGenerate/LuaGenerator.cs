using System;
using System.CodeDom;
using System.Reflection;
using System.Xml.Linq;
using DataTool.CodeGenerate.Auxiliary;
using DataToolInterface.Format.File;
using DataToolInterface.Format.File.Lua;

namespace DataTool.CodeGenerate.FileGenerate.LuaGenerate
{
    [GenerateFileFormat(format = FileFormatEnum.Lua)]
    public class LuaGenerator:FileGenerator
    {
        public LuaGenerator(XElement paramXmlElement,CodeNamespace paramCodeNamespace):base(paramXmlElement, paramCodeNamespace)
        {
            GenerateLua();
        }

        private void GenerateLua()
        {
            CodeTypeDeclaration = new CodeTypeDeclaration
            {
                Name = LuaTypeNameEnum.LuaFile.ToString(),
                IsClass = true,
                TypeAttributes = TypeAttributes.Public
            };
            
            var constructor = new CodeConstructor
            {
                Attributes = MemberAttributes.Public
            };
            CodeTypeDeclaration.Members.Add(constructor);
            
            if (!CodeNamespace.HasCodeTypeDeclarationOfName(CodeTypeDeclaration.Name))
            {
                CodeNamespace.Types.Add(CodeTypeDeclaration);
                CodeTypeDeclaration.AppendCodeTypeAttribute<LuaFileAttribute>(XmlElement);
                var codeAttributeDeclaration = new CodeAttributeDeclaration($@"System.Xml.Serialization.XmlInclude");
                codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument(new CodeSnippetExpression($@"typeof({LuaTypeNameEnum.LuaStatement})")));
                CodeTypeDeclaration.CustomAttributes.Add(codeAttributeDeclaration);
                codeAttributeDeclaration = new CodeAttributeDeclaration($@"System.Xml.Serialization.XmlInclude");
                codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument(new CodeSnippetExpression($@"typeof({LuaTypeNameEnum.LuaBlock})")));
                CodeTypeDeclaration.CustomAttributes.Add(codeAttributeDeclaration);
                codeAttributeDeclaration = new CodeAttributeDeclaration($@"System.Xml.Serialization.XmlInclude");
                codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument(new CodeSnippetExpression($@"typeof({LuaTypeNameEnum.LuaIntentBlock})")));
                CodeTypeDeclaration.CustomAttributes.Add(codeAttributeDeclaration);
                
                var codeMemberField = new CodeMemberField()
                {
                    Name = LuaFieldNameEnum.body.ToString(),
                    Attributes = MemberAttributes.Public,
                    Type = new CodeTypeReference($@"List<{LuaTypeNameEnum.LuaChunk}>")
                };
                CodeTypeDeclaration.Members.Add(codeMemberField);
                constructor.Statements.Add(new CodeExpressionStatement(
                    new CodeSnippetExpression($@"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));
            
                GenerateLuaAssistance();
            }
        }

        private void GenerateLuaAssistance()
        {
            var codeTypeDeclaration = new CodeTypeDeclaration
            {
                Name = LuaTypeNameEnum.LuaChunk.ToString(),
                IsClass = true,
                TypeAttributes = TypeAttributes.Public|TypeAttributes.Abstract
            };
            CodeNamespace.Types.Add(codeTypeDeclaration);
            
            codeTypeDeclaration = new CodeTypeDeclaration
            {
                Name = LuaTypeNameEnum.LuaStatement.ToString(),
                IsClass = true,
                TypeAttributes = TypeAttributes.Public,
                BaseTypes = { new CodeTypeReference(LuaTypeNameEnum.LuaChunk.ToString()) }
            };
            CodeNamespace.Types.Add(codeTypeDeclaration);
            
            var codeMemberField = new CodeMemberField()
            {
                Name = LuaFieldNameEnum.body.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"String")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            codeTypeDeclaration = new CodeTypeDeclaration
            {
                Name = LuaTypeNameEnum.LuaBlock.ToString(),
                IsClass = true,
                TypeAttributes = TypeAttributes.Public,
                BaseTypes = { new CodeTypeReference(LuaTypeNameEnum.LuaChunk.ToString()) }
            };
            CodeNamespace.Types.Add(codeTypeDeclaration);
            
            var constructor = new CodeConstructor
            {
                Attributes = MemberAttributes.Public
            };
            codeTypeDeclaration.Members.Add(constructor);
            
            codeMemberField = new CodeMemberField()
            {
                Name = LuaFieldNameEnum.head.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"String")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            codeMemberField = new CodeMemberField()
            {
                Name = LuaFieldNameEnum.tail.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"String")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            codeMemberField = new CodeMemberField()
            {
                Name = LuaFieldNameEnum.body.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"List<{LuaTypeNameEnum.LuaChunk}>")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            constructor.Statements.Add(new CodeExpressionStatement(
                new CodeSnippetExpression($@"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));
            
            codeTypeDeclaration = new CodeTypeDeclaration
            {
                Name = LuaTypeNameEnum.LuaIntentBlock.ToString(),
                IsClass = true,
                TypeAttributes = TypeAttributes.Public,
                BaseTypes = { new CodeTypeReference(LuaTypeNameEnum.LuaChunk.ToString()) }
            };
            CodeNamespace.Types.Add(codeTypeDeclaration);
            
            constructor = new CodeConstructor
            {
                Attributes = MemberAttributes.Public
            };
            codeTypeDeclaration.Members.Add(constructor);
            
            codeMemberField = new CodeMemberField()
            {
                Name = LuaFieldNameEnum.body.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"List<{LuaTypeNameEnum.LuaChunk}>")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            constructor.Statements.Add(new CodeExpressionStatement(
                new CodeSnippetExpression($@"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));
        }
    }
}