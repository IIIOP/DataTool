using System;
using System.CodeDom;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using DataTool.CodeGenerate.Auxiliary;
using DataTool.CodeGenerate.FileGenerate.ExcelGenerate.ContentGenerate;
using DataTool.CodeGenerate.FileGenerate.ExcelGenerate.DefaultGenerate;
using DataToolInterface.Data;
using DataToolInterface.Format.Config;
using DataToolInterface.Format.Config.Global;
using DataToolInterface.Format.File;
using DataToolInterface.Format.File.Excel;

namespace DataTool.CodeGenerate.FileGenerate.ExcelGenerate.SheetGenerate
{
    [GenerateFileSubFormat(format = FileFormatEnum.Excel,subFormat = nameof(ExcelSubFormatEnum.Sheet))]
    public class SheetGenerator:FileGenerator
    {
        public SheetGenerator(XElement paramXmlElement,CodeNamespace paramCodeNamespace) : base(paramXmlElement, paramCodeNamespace)
        {
            GenerateSheet();
        }
        private void GenerateSheet()
        {
            if (XmlElement.HasElements)
            {
                var contentGenerator = new ContentGenerator(XmlElement, CodeNamespace);
                CodeTypeDeclaration = contentGenerator.CodeTypeDeclaration;
                
                var codeTypeDeclaration = new CodeTypeDeclaration()
                {
                    Name = CodeNamespace.GetUniqueNameByString($@"{CodeTypeDeclaration.Name}_{ExcelFieldNameEnum.sheetInfo}"),
                    IsClass = true,
                    TypeAttributes = TypeAttributes.Public
                };
                CodeNamespace.Types.Add(codeTypeDeclaration);

                var regex = new Regex(@"(?<={local:)\w+(?=})");
                if (XmlElement.Attribute(nameof(ExcelSheetAttribute.name)) is XAttribute cattribute)
                {
                    foreach (Match match in regex.Matches(cattribute.Value))
                    {
                        var codeMemberField = new CodeMemberField()
                        {
                            Name = match.Value,
                            Attributes = MemberAttributes.Public,
                            Type = new CodeTypeReference("String"),
                            InitExpression = new CodeSnippetExpression($"\"{match.Value}\""),
                        };
                        if (codeTypeDeclaration.Members.OfType<CodeMemberField>().All(p => p.Name != codeMemberField.Name))
                        {
                            codeTypeDeclaration.Members.Add(codeMemberField);
                        }
                    }
                }
                CodeTypeDeclaration.AddPropertyDeclaration(codeTypeDeclaration.Name,ExcelFieldNameEnum.sheetInfo.ToString());
                var codeConstructor = CodeTypeDeclaration.Members.Cast<CodeTypeMember>().OfType<CodeConstructor>().Single();
                codeConstructor.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"{ExcelFieldNameEnum.sheetInfo.ToString()} = new {codeTypeDeclaration.Name}()")));
            }
            else
            {
                var defaultGenerator = new DefaultGenerator(XmlElement, CodeNamespace);
                CodeTypeDeclaration = defaultGenerator.CodeTypeDeclaration;
            }
        }
    }
}