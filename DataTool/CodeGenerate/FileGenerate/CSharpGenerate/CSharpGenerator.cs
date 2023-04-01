using System;
using System.CodeDom;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using DataTool.CodeGenerate.Auxiliary;
using DataToolInterface.Format.File;
using DataToolInterface.Format.File.Binary;
using DataToolInterface.Format.File.CSharp;

namespace DataTool.CodeGenerate.FileGenerate.CSharpGenerate
{
    [GenerateFileFormat(format = FileFormatEnum.CSharp)]
    public class CSharpGenerator:FileGenerator
    {

        public CSharpGenerator(XElement paramXmlElement,CodeNamespace paramCodeNamespace):base(paramXmlElement, paramCodeNamespace)
        {
            GenerateCSharp();
        }

        private void GenerateCSharp()
        {
            CodeTypeDeclaration = new CodeTypeDeclaration
            {
                Name = CSharpTypeNameEnum.CSharpFile.ToString(),
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
                CodeTypeDeclaration.AppendCodeTypeAttribute<CSharpFileAttribute>(XmlElement);
                
                var codeMemberField = new CodeMemberField()
                {
                    Name = CSharpFieldNameEnum.nameSpace.ToString(),
                    Attributes = MemberAttributes.Public,
                    Type = new CodeTypeReference($@"CSharpNameSpace")
                };
                CodeTypeDeclaration.Members.Add(codeMemberField);
                constructor.Statements.Add(new CodeExpressionStatement(
                    new CodeSnippetExpression($@"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));

                GenerateCSharpNameSpace();
                GenerateCSharpAttribute();
                GenerateCSharpClass();
                GenerateCSharpEnum();
                GenerateCSharpStruct();
                GenerateCSharpField();
                GenerateCSharpProperty();
                GenerateCSharpCtor();
                GenerateCSharpMethod();
                GenerateCSharpChunk();
                GenerateCSharpStatement();
                GenerateCSharpIntentStatement();
                GenerateCSharpBlock();
            }
        }

        private void GenerateCSharpNameSpace()
        {
            var codeTypeDeclaration = new CodeTypeDeclaration
            {
                Name = CSharpTypeNameEnum.CSharpNameSpace.ToString(),
                IsClass = true,
                TypeAttributes = TypeAttributes.Public
            };
            CodeNamespace.Types.Add(codeTypeDeclaration);
            
            var constructor = new CodeConstructor
            {
                Attributes = MemberAttributes.Public
            };
            codeTypeDeclaration.Members.Add(constructor);
            
            var codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.SpaceName.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"String")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.Usings.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"List<String>")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            constructor.Statements.Add(new CodeExpressionStatement(
                new CodeSnippetExpression($@"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));
            
            codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.Classes.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"List<{CSharpTypeNameEnum.CSharpClass}>")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            constructor.Statements.Add(new CodeExpressionStatement(
                new CodeSnippetExpression($@"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));
            
            codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.Enums.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"List<{CSharpTypeNameEnum.CSharpEnum}>")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            constructor.Statements.Add(new CodeExpressionStatement(
                new CodeSnippetExpression($@"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));
            
            codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.Structs.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"List<{CSharpTypeNameEnum.CSharpStruct}>")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            constructor.Statements.Add(new CodeExpressionStatement(
                new CodeSnippetExpression($@"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));
        }
        
        private void GenerateCSharpAttribute()
        {
            var codeTypeDeclaration = new CodeTypeDeclaration
            {
                Name = CSharpTypeNameEnum.CSharpAttribute.ToString(),
                IsClass = true,
                TypeAttributes = TypeAttributes.Public
            };
            CodeNamespace.Types.Add(codeTypeDeclaration);
            
            var constructor = new CodeConstructor
            {
                Attributes = MemberAttributes.Public
            };
            codeTypeDeclaration.Members.Add(constructor);
            
            var codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.AttributeName.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"String")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.Arguments.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"List<String>")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            constructor.Statements.Add(new CodeExpressionStatement(
                new CodeSnippetExpression($@"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));
        }
        
        private void GenerateCSharpClass()
        {
            var codeTypeDeclaration = new CodeTypeDeclaration
            {
                Name = CSharpTypeNameEnum.CSharpClass.ToString(),
                IsClass = true,
                TypeAttributes = TypeAttributes.Public
            };
            CodeNamespace.Types.Add(codeTypeDeclaration);
            
            var constructor = new CodeConstructor
            {
                Attributes = MemberAttributes.Public
            };
            codeTypeDeclaration.Members.Add(constructor);
            
            var codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.Attributes.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"List<{CSharpTypeNameEnum.CSharpAttribute}>")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            constructor.Statements.Add(new CodeExpressionStatement(
                new CodeSnippetExpression($@"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));
            
            codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.ClassName.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"String")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.Prefix.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"String"),
                InitExpression = new CodeSnippetExpression($@"$@""public""")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.Postfixes.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"List<String>")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            constructor.Statements.Add(new CodeExpressionStatement(
                new CodeSnippetExpression($@"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));
            
            codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.Ctor.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"List<{CSharpTypeNameEnum.CSharpCtor}>")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            constructor.Statements.Add(new CodeExpressionStatement(
                new CodeSnippetExpression($@"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));
            
            codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.Fields.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"List<{CSharpTypeNameEnum.CSharpField}>")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            constructor.Statements.Add(new CodeExpressionStatement(
                new CodeSnippetExpression($@"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));
            
            codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.Properties.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"List<{CSharpTypeNameEnum.CSharpProperty}>")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            constructor.Statements.Add(new CodeExpressionStatement(
                new CodeSnippetExpression($@"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));
            
            codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.Methods.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"List<{CSharpTypeNameEnum.CSharpMethod}>")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            constructor.Statements.Add(new CodeExpressionStatement(
                new CodeSnippetExpression($@"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));
        }
        
        private void GenerateCSharpEnum()
        {
            var codeTypeDeclaration = new CodeTypeDeclaration
            {
                Name = CSharpTypeNameEnum.CSharpEnum.ToString(),
                IsClass = true,
                TypeAttributes = TypeAttributes.Public
            };
            CodeNamespace.Types.Add(codeTypeDeclaration);
            
            var constructor = new CodeConstructor
            {
                Attributes = MemberAttributes.Public
            };
            codeTypeDeclaration.Members.Add(constructor);
            
            var codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.Attributes.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"List<{CSharpTypeNameEnum.CSharpAttribute}>")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            constructor.Statements.Add(new CodeExpressionStatement(
                new CodeSnippetExpression($@"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));
            
            codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.EnumName.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"String")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.Prefix.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"String"),
                InitExpression = new CodeSnippetExpression($@"$@""public""")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.Items.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"List<String>")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            constructor.Statements.Add(new CodeExpressionStatement(
                new CodeSnippetExpression($@"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));
        }
        
        private void GenerateCSharpStruct()
        {
            var codeTypeDeclaration = new CodeTypeDeclaration
            {
                Name = CSharpTypeNameEnum.CSharpStruct.ToString(),
                IsClass = true,
                TypeAttributes = TypeAttributes.Public
            };
            CodeNamespace.Types.Add(codeTypeDeclaration);
            
            var constructor = new CodeConstructor
            {
                Attributes = MemberAttributes.Public
            };
            codeTypeDeclaration.Members.Add(constructor);
            
            var codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.Attributes.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"List<{CSharpTypeNameEnum.CSharpAttribute}>")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            constructor.Statements.Add(new CodeExpressionStatement(
                new CodeSnippetExpression($@"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));
            
            codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.StructName.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"String")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.Prefix.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"String"),
                InitExpression = new CodeSnippetExpression($@"$@""public""")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.Postfixes.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"List<String>")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            constructor.Statements.Add(new CodeExpressionStatement(
                new CodeSnippetExpression($@"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));
            
            codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.Ctor.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"List<{CSharpTypeNameEnum.CSharpCtor}>")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            constructor.Statements.Add(new CodeExpressionStatement(
                new CodeSnippetExpression($@"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));
            
            codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.Fields.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"List<{CSharpTypeNameEnum.CSharpField}>")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            constructor.Statements.Add(new CodeExpressionStatement(
                new CodeSnippetExpression($@"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));
            
            codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.Properties.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"List<{CSharpTypeNameEnum.CSharpProperty}>")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            constructor.Statements.Add(new CodeExpressionStatement(
                new CodeSnippetExpression($@"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));
            
            codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.Methods.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"List<{CSharpTypeNameEnum.CSharpMethod}>")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            constructor.Statements.Add(new CodeExpressionStatement(
                new CodeSnippetExpression($@"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));
        }
        
        private void GenerateCSharpField()
        {
            var codeTypeDeclaration = new CodeTypeDeclaration
            {
                Name = CSharpTypeNameEnum.CSharpField.ToString(),
                IsClass = true,
                TypeAttributes = TypeAttributes.Public
            };
            CodeNamespace.Types.Add(codeTypeDeclaration);
            
            var constructor = new CodeConstructor
            {
                Attributes = MemberAttributes.Public
            };
            codeTypeDeclaration.Members.Add(constructor);
            
            var codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.Attributes.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"List<{CSharpTypeNameEnum.CSharpAttribute}>")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            constructor.Statements.Add(new CodeExpressionStatement(
                new CodeSnippetExpression($@"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));
            
            codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.FieldType.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"String")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.FieldName.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"String")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.Prefix.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"String"),
                InitExpression = new CodeSnippetExpression($@"$@""public""")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.InitExpression.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"String")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
        }
        
        private void GenerateCSharpProperty()
        {
            var codeTypeDeclaration = new CodeTypeDeclaration
            {
                Name = CSharpTypeNameEnum.CSharpProperty.ToString(),
                IsClass = true,
                TypeAttributes = TypeAttributes.Public
            };
            CodeNamespace.Types.Add(codeTypeDeclaration);
            
            var codeAttributeDeclaration = new CodeAttributeDeclaration($@"System.Xml.Serialization.XmlInclude");
            codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument(new CodeSnippetExpression($@"typeof({CSharpTypeNameEnum.CSharpStatement})")));
            codeTypeDeclaration.CustomAttributes.Add(codeAttributeDeclaration);
            
            codeAttributeDeclaration = new CodeAttributeDeclaration($@"System.Xml.Serialization.XmlInclude");
            codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument(new CodeSnippetExpression($@"typeof({CSharpTypeNameEnum.CSharpIntentStatement})")));
            codeTypeDeclaration.CustomAttributes.Add(codeAttributeDeclaration);
            
            codeAttributeDeclaration = new CodeAttributeDeclaration($@"System.Xml.Serialization.XmlInclude");
            codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument(new CodeSnippetExpression($@"typeof({CSharpTypeNameEnum.CSharpBlock})")));
            codeTypeDeclaration.CustomAttributes.Add(codeAttributeDeclaration);
            
            var constructor = new CodeConstructor
            {
                Attributes = MemberAttributes.Public
            };
            codeTypeDeclaration.Members.Add(constructor);
            
            var codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.Attributes.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"List<{CSharpTypeNameEnum.CSharpAttribute}>")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            constructor.Statements.Add(new CodeExpressionStatement(
                new CodeSnippetExpression($@"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));
            
            codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.PropertyType.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"String")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.PropertyName.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"String")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.Prefix.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"String"),
                InitExpression = new CodeSnippetExpression($@"$@""public""")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.Get.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"List<CSharpChunk>")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            constructor.Statements.Add(new CodeExpressionStatement(
                new CodeSnippetExpression($@"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));
            
            codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.Set.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"List<CSharpChunk>")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            constructor.Statements.Add(new CodeExpressionStatement(
                new CodeSnippetExpression($@"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));
            
            codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.InitExpression.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"String")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
        }
        private void GenerateCSharpCtor()
        {
            var codeTypeDeclaration = new CodeTypeDeclaration
            {
                Name = CSharpTypeNameEnum.CSharpCtor.ToString(),
                IsClass = true,
                TypeAttributes = TypeAttributes.Public
            };
            CodeNamespace.Types.Add(codeTypeDeclaration);
            
            var codeAttributeDeclaration = new CodeAttributeDeclaration($@"System.Xml.Serialization.XmlInclude");
            codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument(new CodeSnippetExpression($@"typeof({CSharpTypeNameEnum.CSharpStatement})")));
            codeTypeDeclaration.CustomAttributes.Add(codeAttributeDeclaration);
            
            codeAttributeDeclaration = new CodeAttributeDeclaration($@"System.Xml.Serialization.XmlInclude");
            codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument(new CodeSnippetExpression($@"typeof({CSharpTypeNameEnum.CSharpIntentStatement})")));
            codeTypeDeclaration.CustomAttributes.Add(codeAttributeDeclaration);
            
            codeAttributeDeclaration = new CodeAttributeDeclaration($@"System.Xml.Serialization.XmlInclude");
            codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument(new CodeSnippetExpression($@"typeof({CSharpTypeNameEnum.CSharpBlock})")));
            codeTypeDeclaration.CustomAttributes.Add(codeAttributeDeclaration);
            
            var constructor = new CodeConstructor
            {
                Attributes = MemberAttributes.Public
            };
            codeTypeDeclaration.Members.Add(constructor);
            
            var codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.Attributes.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"List<{CSharpTypeNameEnum.CSharpAttribute}>")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            constructor.Statements.Add(new CodeExpressionStatement(
                new CodeSnippetExpression($@"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));

            codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.Prefix.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"String"),
                InitExpression = new CodeSnippetExpression($@"$@""public""")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.Postfixes.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"String")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);

            codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.Arguments.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"List<String>")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            constructor.Statements.Add(new CodeExpressionStatement(
                new CodeSnippetExpression($@"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));
            
            codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.Body.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"List<{CSharpTypeNameEnum.CSharpChunk}>")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            constructor.Statements.Add(new CodeExpressionStatement(
                new CodeSnippetExpression($@"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));
        }
        private void GenerateCSharpMethod()
        {
            var codeTypeDeclaration = new CodeTypeDeclaration
            {
                Name = CSharpTypeNameEnum.CSharpMethod.ToString(),
                IsClass = true,
                TypeAttributes = TypeAttributes.Public
            };
            CodeNamespace.Types.Add(codeTypeDeclaration);
            
            var codeAttributeDeclaration = new CodeAttributeDeclaration($@"System.Xml.Serialization.XmlInclude");
            codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument(new CodeSnippetExpression($@"typeof({CSharpTypeNameEnum.CSharpStatement})")));
            codeTypeDeclaration.CustomAttributes.Add(codeAttributeDeclaration);
            
            codeAttributeDeclaration = new CodeAttributeDeclaration($@"System.Xml.Serialization.XmlInclude");
            codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument(new CodeSnippetExpression($@"typeof({CSharpTypeNameEnum.CSharpIntentStatement})")));
            codeTypeDeclaration.CustomAttributes.Add(codeAttributeDeclaration);
            
            codeAttributeDeclaration = new CodeAttributeDeclaration($@"System.Xml.Serialization.XmlInclude");
            codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument(new CodeSnippetExpression($@"typeof({CSharpTypeNameEnum.CSharpBlock})")));
            codeTypeDeclaration.CustomAttributes.Add(codeAttributeDeclaration);
            
            var constructor = new CodeConstructor
            {
                Attributes = MemberAttributes.Public
            };
            codeTypeDeclaration.Members.Add(constructor);
            
            var codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.Attributes.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"List<{CSharpTypeNameEnum.CSharpAttribute}>")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            constructor.Statements.Add(new CodeExpressionStatement(
                new CodeSnippetExpression($@"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));
            
            codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.MethodName.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"String")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.Prefix.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"String"),
                InitExpression = new CodeSnippetExpression($@"$@""public""")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.ReturnType.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"String"),
                InitExpression = new CodeSnippetExpression($@"$@""void""")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.Arguments.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"List<String>")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            constructor.Statements.Add(new CodeExpressionStatement(
                new CodeSnippetExpression($@"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));
            
            codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.Body.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"List<{CSharpTypeNameEnum.CSharpChunk}>")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            constructor.Statements.Add(new CodeExpressionStatement(
                new CodeSnippetExpression($@"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));
        }
        
        private void GenerateCSharpChunk()
        {
            var codeTypeDeclaration = new CodeTypeDeclaration
            {
                Name = CSharpTypeNameEnum.CSharpChunk.ToString(),
                IsClass = true,
                TypeAttributes = TypeAttributes.Public|TypeAttributes.Abstract
            };
            CodeNamespace.Types.Add(codeTypeDeclaration);
        }
        
        private void GenerateCSharpStatement()
        {
            var codeTypeDeclaration = new CodeTypeDeclaration
            {
                Name = CSharpTypeNameEnum.CSharpStatement.ToString(),
                IsClass = true,
                TypeAttributes = TypeAttributes.Public,
                BaseTypes = { new CodeTypeReference(CSharpTypeNameEnum.CSharpChunk.ToString()) }
            };
            CodeNamespace.Types.Add(codeTypeDeclaration);

            var codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.Body.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"String")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
        }
        private void GenerateCSharpIntentStatement()
        {
            var codeTypeDeclaration = new CodeTypeDeclaration
            {
                Name = CSharpTypeNameEnum.CSharpIntentStatement.ToString(),
                IsClass = true,
                TypeAttributes = TypeAttributes.Public,
                BaseTypes = { new CodeTypeReference(CSharpTypeNameEnum.CSharpChunk.ToString()) }
            };
            CodeNamespace.Types.Add(codeTypeDeclaration);

            var codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.Body.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"String")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
        }
        private void GenerateCSharpBlock()
        {
            var codeTypeDeclaration = new CodeTypeDeclaration
            {
                Name = CSharpTypeNameEnum.CSharpBlock.ToString(),
                IsClass = true,
                TypeAttributes = TypeAttributes.Public,
                BaseTypes = { new CodeTypeReference(CSharpTypeNameEnum.CSharpChunk.ToString()) }
            };
            CodeNamespace.Types.Add(codeTypeDeclaration);
            
            var constructor = new CodeConstructor
            {
                Attributes = MemberAttributes.Public
            };
            codeTypeDeclaration.Members.Add(constructor);
            
            var codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.Head.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"String")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            codeMemberField = new CodeMemberField()
            {
                Name = CSharpFieldNameEnum.Body.ToString(),
                Attributes = MemberAttributes.Public,
                Type = new CodeTypeReference($@"List<{CSharpTypeNameEnum.CSharpChunk}>")
            };
            codeTypeDeclaration.Members.Add(codeMemberField);
            
            constructor.Statements.Add(new CodeExpressionStatement(
                new CodeSnippetExpression($@"{codeMemberField.Name} = new {codeMemberField.Type.BaseType}()")));
        }
    }
}