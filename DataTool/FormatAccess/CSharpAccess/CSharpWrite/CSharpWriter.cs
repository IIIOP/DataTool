using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DataTool.FormatAccess.Auxiliary;
using DataToolInterface.Format.File;
using DataToolInterface.Format.File.CSharp;

namespace DataTool.FormatAccess.CSharpAccess.CSharpWrite
{
    [FormatAccess(format = FileFormatEnum.CSharp)]
    public class CSharpWriter:FormatWriter
    {
        public CSharpWriter(object paramObject,FileInfo paramFileInfo) : base(paramObject,paramFileInfo)
        {
            WriteCSharp();
        }

        private void WriteCSharp()
        {
            var textWriter = new StreamWriter(FileInfo.FullName, false, Encoding.UTF8);
            var indentedTextWriter = new IndentedTextWriter(textWriter);

            if (Object.GetType().GetField(CSharpFieldNameEnum.nameSpace.ToString()).GetValue(Object) is object nameSpace)
            {
                WriteCSharpNameSpace(nameSpace,indentedTextWriter);
            }
            
            indentedTextWriter.Flush();
        }

        private void WriteCSharpNameSpace(object paramNameSpace, IndentedTextWriter paramIndentedTextWriter)
        {
            if (paramNameSpace == null) throw new ArgumentNullException(nameof(paramNameSpace));
            if (paramIndentedTextWriter == null) throw new ArgumentNullException(nameof(paramIndentedTextWriter));

            var spaceName = paramNameSpace.GetType().GetField(CSharpFieldNameEnum.SpaceName.ToString())
                .GetValue(paramNameSpace) as string;
            paramIndentedTextWriter.WriteLine($@"namespace {spaceName}");
            paramIndentedTextWriter.WriteLine($@"{{");
            paramIndentedTextWriter.Indent++;

            if (paramNameSpace.GetType().GetField(CSharpFieldNameEnum.Usings.ToString())
                    .GetValue(paramNameSpace) is List<string> usings)
            {
                foreach (var iUsing in usings)
                {
                    paramIndentedTextWriter.WriteLine($@"using {iUsing};");
                }
            }

            if (paramNameSpace.GetType().GetField(CSharpFieldNameEnum.Classes.ToString())
                    .GetValue(paramNameSpace) is IList classes)
            {
                foreach (var @class in classes)
                {
                    WriteCSharpClass(@class, paramIndentedTextWriter);
                }
            }

            if (paramNameSpace.GetType().GetField(CSharpFieldNameEnum.Enums.ToString())
                    .GetValue(paramNameSpace) is IList enums)
            {
                foreach (var @enum in enums)
                {
                    WriteCSharpEnum(@enum, paramIndentedTextWriter);
                }
            }

            if (paramNameSpace.GetType().GetField(CSharpFieldNameEnum.Structs.ToString())
                    .GetValue(paramNameSpace) is IList structs)
            {
                foreach (var @struct in structs)
                {
                    WriteCSharpStruct(@struct, paramIndentedTextWriter);
                }
            }
            
            paramIndentedTextWriter.Indent--;
            paramIndentedTextWriter.WriteLine($@"}}");
        }

        private void WriteCSharpClass(object paramCSharpClass, IndentedTextWriter paramIndentedTextWriter)
        {
            if (paramCSharpClass.GetType().GetField(CSharpFieldNameEnum.Attributes.ToString())
                    .GetValue(paramCSharpClass) is IList attributes)
            {
                foreach (var attribute in attributes)
                {
                    WriteCSharpAttribute(attribute,paramIndentedTextWriter);
                }
            }

            var className = paramCSharpClass.GetType().GetField(CSharpFieldNameEnum.ClassName.ToString())
                .GetValue(paramCSharpClass) as string;
            var prefix = paramCSharpClass.GetType().GetField(CSharpFieldNameEnum.Prefix.ToString())
                .GetValue(paramCSharpClass) as string;
            var stringBuilder = new StringBuilder($@":");
            if (paramCSharpClass.GetType().GetField(CSharpFieldNameEnum.Postfixes.ToString())
                    .GetValue(paramCSharpClass) is List<string> postfixes)
            {
                foreach (var postfix in postfixes)
                {
                    stringBuilder.Append($@"{postfix},");
                }
            }
            paramIndentedTextWriter.WriteLine($@"{prefix} class {className} {(stringBuilder.Length>1?stringBuilder.ToString().TrimEnd(','):"")}");
            paramIndentedTextWriter.WriteLine($@"{{");
            paramIndentedTextWriter.Indent++;

            if (paramCSharpClass.GetType().GetField(CSharpFieldNameEnum.Fields.ToString())
                    .GetValue(paramCSharpClass) is IList fields)
            {
                foreach (var field in fields)
                {
                    WriteCSharpField(field, paramIndentedTextWriter);
                }
            }
            
            if (paramCSharpClass.GetType().GetField(CSharpFieldNameEnum.Properties.ToString())
                    .GetValue(paramCSharpClass) is IList properties)
            {
                foreach (var property in properties)
                {
                    WriteCSharpProperty(property, paramIndentedTextWriter);
                }
            }
            
            if (paramCSharpClass.GetType().GetField(CSharpFieldNameEnum.Ctor.ToString())
                    .GetValue(paramCSharpClass) is IList ctors)
            {
                foreach (var ctor in ctors)
                {
                    if (ctor.GetType().GetField(CSharpFieldNameEnum.Attributes.ToString())
                            .GetValue(ctor) is IList ctorAttributes)
                    {
                        foreach (var attribute in ctorAttributes)
                        {
                            WriteCSharpAttribute(attribute,paramIndentedTextWriter);
                        }
                    }

                    prefix = ctor.GetType().GetField(CSharpFieldNameEnum.Prefix.ToString())
                        .GetValue(ctor) as string;
                    var postfix = ctor.GetType().GetField(CSharpFieldNameEnum.Postfixes.ToString())
                        .GetValue(ctor) as string;
                    stringBuilder = new StringBuilder();
                    if (ctor.GetType().GetField(CSharpFieldNameEnum.Arguments.ToString())
                            .GetValue(ctor) is List<string> arguments)
                    {
                        foreach (var argument in arguments)
                        {
                            stringBuilder.Append($@"{argument},");
                        }
                    }
                    paramIndentedTextWriter.WriteLine($@"{prefix} {className}({stringBuilder.ToString().TrimEnd(',')}) {(!string.IsNullOrWhiteSpace(postfix)?$@": {postfix}":"")}");
                    paramIndentedTextWriter.WriteLine($@"{{");
                    paramIndentedTextWriter.Indent++;

                    if (ctor.GetType().GetField(CSharpFieldNameEnum.Body.ToString())
                            .GetValue(ctor) is IList CSharpChunks)
                    {
                        foreach (var cSharpChunk in CSharpChunks)
                        {
                            WriteCSharpChunk(cSharpChunk,paramIndentedTextWriter);
                        }
                    }
                
                    paramIndentedTextWriter.Indent--;
                    paramIndentedTextWriter.WriteLine($@"}}");
                }
            }

            
            if (paramCSharpClass.GetType().GetField(CSharpFieldNameEnum.Methods.ToString())
                    .GetValue(paramCSharpClass) is IList methods)
            {
                foreach (var method in methods)
                {
                    WriteCSharpMethod(method, paramIndentedTextWriter);
                }
            }
            
            
            paramIndentedTextWriter.Indent--;
            paramIndentedTextWriter.WriteLine($@"}}");
        }

        private void WriteCSharpEnum(object paramCSharpEnum, IndentedTextWriter paramIndentedTextWriter)
        {
            if (paramCSharpEnum.GetType().GetField(CSharpFieldNameEnum.Attributes.ToString())
                    .GetValue(paramCSharpEnum) is IList attributes)
            {
                foreach (var attribute in attributes)
                {
                    WriteCSharpAttribute(attribute,paramIndentedTextWriter);
                }
            }
            
            var enumName = paramCSharpEnum.GetType().GetField(CSharpFieldNameEnum.EnumName.ToString())
                .GetValue(paramCSharpEnum) as string;
            var prefix = paramCSharpEnum.GetType().GetField(CSharpFieldNameEnum.Prefix.ToString())
                .GetValue(paramCSharpEnum) as string;
            paramIndentedTextWriter.WriteLine($@"{prefix} enum {enumName}");
            paramIndentedTextWriter.WriteLine($@"{{");
            paramIndentedTextWriter.Indent++;

            if (paramCSharpEnum.GetType().GetField(CSharpFieldNameEnum.Items.ToString())
                    .GetValue(paramCSharpEnum) is List<string> items)
            {
                foreach (var item in items.Distinct())
                {
                    paramIndentedTextWriter.WriteLine($@"{item},");
                }
            }
            
            paramIndentedTextWriter.Indent--;
            paramIndentedTextWriter.WriteLine($@"}}");
        }


        private void WriteCSharpStruct(object paramCSharpStruct, IndentedTextWriter paramIndentedTextWriter)
        {
            if (paramCSharpStruct.GetType().GetField(CSharpFieldNameEnum.Attributes.ToString())
                    .GetValue(paramCSharpStruct) is IList attributes)
            {
                foreach (var attribute in attributes)
                {
                    WriteCSharpAttribute(attribute,paramIndentedTextWriter);
                }
            }

            var structName = paramCSharpStruct.GetType().GetField(CSharpFieldNameEnum.StructName.ToString())
                .GetValue(paramCSharpStruct) as string;
            var prefix = paramCSharpStruct.GetType().GetField(CSharpFieldNameEnum.Prefix.ToString())
                .GetValue(paramCSharpStruct) as string;
            var stringBuilder = new StringBuilder($@":");
            if (paramCSharpStruct.GetType().GetField(CSharpFieldNameEnum.Postfixes.ToString())
                    .GetValue(paramCSharpStruct) is List<string> postfixes)
            {
                foreach (var postfix in postfixes)
                {
                    stringBuilder.Append($@"{postfix},");
                }
            }
            paramIndentedTextWriter.WriteLine($@"{prefix} struct {structName} {(stringBuilder.Length>1?stringBuilder.ToString().TrimEnd(','):"")}");
            paramIndentedTextWriter.WriteLine($@"{{");
            paramIndentedTextWriter.Indent++;

            if (paramCSharpStruct.GetType().GetField(CSharpFieldNameEnum.Fields.ToString())
                    .GetValue(paramCSharpStruct) is IList fields)
            {
                foreach (var field in fields)
                {
                    WriteCSharpField(field, paramIndentedTextWriter);
                }
            }
            
            if (paramCSharpStruct.GetType().GetField(CSharpFieldNameEnum.Properties.ToString())
                    .GetValue(paramCSharpStruct) is IList properties)
            {
                foreach (var property in properties)
                {
                    WriteCSharpProperty(property, paramIndentedTextWriter);
                }
            }
            
            if (paramCSharpStruct.GetType().GetField(CSharpFieldNameEnum.Ctor.ToString())
                    .GetValue(paramCSharpStruct) is IList ctors)
            {
                foreach (var ctor in ctors)
                {
                    if (ctor.GetType().GetField(CSharpFieldNameEnum.Attributes.ToString())
                            .GetValue(ctor) is IList ctorAttributes)
                    {
                        foreach (var attribute in ctorAttributes)
                        {
                            WriteCSharpAttribute(attribute,paramIndentedTextWriter);
                        }
                    }

                    prefix = ctor.GetType().GetField(CSharpFieldNameEnum.Prefix.ToString())
                        .GetValue(ctor) as string;
                    var postfix = ctor.GetType().GetField(CSharpFieldNameEnum.Postfixes.ToString())
                        .GetValue(ctor) as string;
                    stringBuilder = new StringBuilder();
                    if (ctor.GetType().GetField(CSharpFieldNameEnum.Arguments.ToString())
                            .GetValue(ctor) is List<string> arguments)
                    {
                        foreach (var argument in arguments)
                        {
                            stringBuilder.Append($@"{argument},");
                        }
                    }
                    paramIndentedTextWriter.WriteLine($@"{prefix} {structName}({stringBuilder.ToString().TrimEnd(',')}) {(!string.IsNullOrWhiteSpace(postfix)?$@": {postfix}":"")}");
                    paramIndentedTextWriter.WriteLine($@"{{");
                    paramIndentedTextWriter.Indent++;

                    if (ctor.GetType().GetField(CSharpFieldNameEnum.Body.ToString())
                            .GetValue(ctor) is IList CSharpChunks)
                    {
                        foreach (var cSharpChunk in CSharpChunks)
                        {
                            WriteCSharpChunk(cSharpChunk,paramIndentedTextWriter);
                        }
                    }
                
                    paramIndentedTextWriter.Indent--;
                    paramIndentedTextWriter.WriteLine($@"}}");
                }
            }
            
            if (paramCSharpStruct.GetType().GetField(CSharpFieldNameEnum.Methods.ToString())
                    .GetValue(paramCSharpStruct) is IList methods)
            {
                foreach (var method in methods)
                {
                    WriteCSharpMethod(method, paramIndentedTextWriter);
                }
            }
            
            paramIndentedTextWriter.Indent--;
            paramIndentedTextWriter.WriteLine($@"}}");
        }
        private void WriteCSharpAttribute(object paramCSharpAttribute, IndentedTextWriter paramIndentedTextWriter)
        {
            var attributeName = paramCSharpAttribute.GetType().GetField(CSharpFieldNameEnum.AttributeName.ToString())
                .GetValue(paramCSharpAttribute) as string;

            var stringBuilder = new StringBuilder();
            if (paramCSharpAttribute.GetType().GetField(CSharpFieldNameEnum.Arguments.ToString())
                    .GetValue(paramCSharpAttribute) is List<string> arguments)
            {
                foreach (var argument in arguments)
                {
                    stringBuilder.Append($@"{argument},");
                }
            }
            paramIndentedTextWriter.WriteLine($@"[{attributeName}({stringBuilder.ToString().TrimEnd(',')})]");
        }
        
        private void WriteCSharpField(object paramCSharpField, IndentedTextWriter paramIndentedTextWriter)
        {
            if (paramCSharpField.GetType().GetField(CSharpFieldNameEnum.Attributes.ToString())
                    .GetValue(paramCSharpField) is IList attributes)
            {
                foreach (var attribute in attributes)
                {
                    WriteCSharpAttribute(attribute,paramIndentedTextWriter);
                }
            }

            var fieldType = paramCSharpField.GetType().GetField(CSharpFieldNameEnum.FieldType.ToString())
                .GetValue(paramCSharpField) as string;
            var fieldName = paramCSharpField.GetType().GetField(CSharpFieldNameEnum.FieldName.ToString())
                .GetValue(paramCSharpField) as string;
            var prefix = paramCSharpField.GetType().GetField(CSharpFieldNameEnum.Prefix.ToString())
                .GetValue(paramCSharpField) as string;
            var initExpression = paramCSharpField.GetType().GetField(CSharpFieldNameEnum.InitExpression.ToString())
                .GetValue(paramCSharpField) as string;
            
            paramIndentedTextWriter.WriteLine($@"{prefix} {fieldType} {fieldName} {(!string.IsNullOrWhiteSpace(initExpression)?$@"= {initExpression}":"")};");
        }

        private void WriteCSharpProperty(object paramCSharpProperty, IndentedTextWriter paramIndentedTextWriter)
        {
            if (paramCSharpProperty.GetType().GetField(CSharpFieldNameEnum.Attributes.ToString())
                    .GetValue(paramCSharpProperty) is IList attributes)
            {
                foreach (var attribute in attributes)
                {
                    WriteCSharpAttribute(attribute,paramIndentedTextWriter);
                }
            }

            var propertyType = paramCSharpProperty.GetType().GetField(CSharpFieldNameEnum.PropertyType.ToString())
                .GetValue(paramCSharpProperty) as string;
            var propertyName = paramCSharpProperty.GetType().GetField(CSharpFieldNameEnum.PropertyName.ToString())
                .GetValue(paramCSharpProperty) as string;
            var prefix = paramCSharpProperty.GetType().GetField(CSharpFieldNameEnum.Prefix.ToString())
                .GetValue(paramCSharpProperty) as string;
            var gets = paramCSharpProperty.GetType().GetField(CSharpFieldNameEnum.Get.ToString())
                .GetValue(paramCSharpProperty) as IList;
            var sets = paramCSharpProperty.GetType().GetField(CSharpFieldNameEnum.Set.ToString())
                .GetValue(paramCSharpProperty) as IList;
            var initExpression = paramCSharpProperty.GetType().GetField(CSharpFieldNameEnum.InitExpression.ToString())
                .GetValue(paramCSharpProperty) as string;
            
            paramIndentedTextWriter.WriteLine($@"{prefix} {propertyType} {propertyName}");
            paramIndentedTextWriter.WriteLine($@"{{");
            paramIndentedTextWriter.Indent++;
            if (gets.Count>0||sets.Count>0)
            {
                if (gets.Count>0)
                {
                    paramIndentedTextWriter.WriteLine($@"get");
                    paramIndentedTextWriter.WriteLine($@"{{");
                    paramIndentedTextWriter.Indent++;
                    foreach (var get in gets)
                    {
                        WriteCSharpChunk(get,paramIndentedTextWriter);
                    }
                    paramIndentedTextWriter.Indent--;
                    paramIndentedTextWriter.WriteLine($@"}}");
                }
                if (sets.Count>0)
                {
                    paramIndentedTextWriter.WriteLine($@"set");
                    paramIndentedTextWriter.WriteLine($@"{{");
                    paramIndentedTextWriter.Indent++;
                    foreach (var set in sets)
                    {
                        WriteCSharpChunk(set,paramIndentedTextWriter);
                    }
                    paramIndentedTextWriter.Indent--;
                    paramIndentedTextWriter.WriteLine($@"}}");
                }
            }
            else
            {
                paramIndentedTextWriter.WriteLine($@"get;");
                paramIndentedTextWriter.WriteLine($@"set;");
            }
            
            paramIndentedTextWriter.Indent--;

            if (!string.IsNullOrWhiteSpace(initExpression))
            {
                paramIndentedTextWriter.WriteLine($@"}} = {initExpression};");
            }
            else
            {
                paramIndentedTextWriter.WriteLine($@"}}");
            }
        }

        private void WriteCSharpMethod(object paramCSharpMethod, IndentedTextWriter paramIndentedTextWriter)
        {
            if (paramCSharpMethod.GetType().GetField(CSharpFieldNameEnum.Attributes.ToString())
                    .GetValue(paramCSharpMethod) is IList attributes)
            {
                foreach (var attribute in attributes)
                {
                    WriteCSharpAttribute(attribute,paramIndentedTextWriter);
                }
            }
            var methodName = paramCSharpMethod.GetType().GetField(CSharpFieldNameEnum.MethodName.ToString())
                .GetValue(paramCSharpMethod) as string;
            var prefix = paramCSharpMethod.GetType().GetField(CSharpFieldNameEnum.Prefix.ToString())
                .GetValue(paramCSharpMethod) as string;
            var returnType = paramCSharpMethod.GetType().GetField(CSharpFieldNameEnum.ReturnType.ToString())
                .GetValue(paramCSharpMethod) as string;
            var stringBuilder = new StringBuilder();
            if (paramCSharpMethod.GetType().GetField(CSharpFieldNameEnum.Arguments.ToString())
                    .GetValue(paramCSharpMethod) is List<string> arguments)
            {
                foreach (var argument in arguments)
                {
                    stringBuilder.Append($@"{argument},");
                }
            }
            paramIndentedTextWriter.WriteLine($@"{prefix} {returnType} {methodName}({stringBuilder.ToString().TrimEnd(',')})");
            paramIndentedTextWriter.WriteLine($@"{{");
            paramIndentedTextWriter.Indent++;

            if (paramCSharpMethod.GetType().GetField(CSharpFieldNameEnum.Body.ToString())
                    .GetValue(paramCSharpMethod) is IList chunks)
            {
                foreach (var chunk in chunks)
                {
                    WriteCSharpChunk(chunk, paramIndentedTextWriter);
                }
            }
            
            paramIndentedTextWriter.Indent--;
            paramIndentedTextWriter.WriteLine($@"}}");
        }
        private void WriteCSharpChunk(object paramCSharpChunk, IndentedTextWriter paramIndentedTextWriter)
        {
            var typeName = paramCSharpChunk.GetType().Name;
            switch (typeName)
            {
                case nameof(CSharpTypeNameEnum.CSharpStatement):
                    var body = paramCSharpChunk.GetType().GetField(CSharpFieldNameEnum.Body.ToString())
                        .GetValue(paramCSharpChunk) as string;
                    paramIndentedTextWriter.WriteLine($@"{body};");
                    break;
                case nameof(CSharpTypeNameEnum.CSharpIntentStatement):
                    body = paramCSharpChunk.GetType().GetField(CSharpFieldNameEnum.Body.ToString())
                        .GetValue(paramCSharpChunk) as string;
                    paramIndentedTextWriter.Indent++;
                    paramIndentedTextWriter.WriteLine($@"{body};");
                    paramIndentedTextWriter.Indent--;
                    break;
                case nameof(CSharpTypeNameEnum.CSharpBlock):
                    var head = paramCSharpChunk.GetType().GetField(CSharpFieldNameEnum.Head.ToString())
                        .GetValue(paramCSharpChunk) as string;
                    paramIndentedTextWriter.WriteLine($@"{head}");
                    paramIndentedTextWriter.WriteLine($@"{{");
                    paramIndentedTextWriter.Indent++;
                    if (paramCSharpChunk.GetType().GetField(CSharpFieldNameEnum.Body.ToString())
                            .GetValue(paramCSharpChunk) is IList items)
                    {
                        foreach (var item in items)
                        {
                            WriteCSharpChunk(item, paramIndentedTextWriter);
                        }
                    }
                    paramIndentedTextWriter.Indent--;
                    paramIndentedTextWriter.WriteLine($@"}}");
                    break;
            }
        }
    }
}