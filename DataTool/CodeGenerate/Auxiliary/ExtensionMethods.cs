using System;
using System.CodeDom;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using DataToolInterface.Auxiliary;
using DataToolInterface.Data;

namespace DataTool.CodeGenerate.Auxiliary
{
    public static class ExtensionMethods
    {
        public static bool IsArray(this string param)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));
            return param.EndsWith("[]");
        }
        public static bool IsBasicType(this string param)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));
            return Enum.TryParse(param.BaseType(),out BasicDataTypeEnum _);
        }
        public static string BaseType(this string param)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));
            return param.TrimEnd("[]".ToCharArray());
        }

        public static string AsCodeType(this string param)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));
            
            var codeType = param;
            if (Enum.TryParse(codeType,out BasicDataTypeEnum basicDataTypeEnum))
            {
                codeType = BasicDataType.BasicDataTypeNameConvertDictionary[basicDataTypeEnum];
            }

            return codeType;
        }

        public static string FormatToCommandLineArg(this string param)
        {
            return $@"""{param}""";
        }
        
        public static void AppendCodeTypeAttribute<T>(this CodeTypeMember paramCodeTypeMember, XElement paramXElement) where T:Attribute
        {
            if (paramCodeTypeMember == null) throw new ArgumentNullException(nameof(paramCodeTypeMember));
            if (paramXElement == null) throw new ArgumentNullException(nameof(paramXElement));
            
            var codeAttributeDeclaration = new CodeAttributeDeclaration(typeof(T).Name);
            paramCodeTypeMember.CustomAttributes.Add(codeAttributeDeclaration);

            foreach (var fieldInfo in typeof(T).GetProperties())
            {
                var require = fieldInfo.GetCustomAttribute<RequireAttribute>();
                var attValue = paramXElement.Attribute(fieldInfo.Name);
                if (attValue != null)
                {
                    codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument(fieldInfo.Name, new CodeSnippetExpression("@\"" + attValue.Value + "\"")));
                }
                else if (require!=null)
                {
                    throw new Exception($@"缺少 {fieldInfo.Name}属性");
                }
            }
        }

        public static void AddPropertyDeclaration(this CodeTypeDeclaration paramCodeTypeDeclaration,string propertyType,string propertyName)
        {
            paramCodeTypeDeclaration.Members.Add(new CodeSnippetTypeMember($"\t\tpublic {propertyType} {propertyName} {{ get; set; }}\n"));
        }
        
        public static void CopyFromDirectory(this DirectoryInfo paramLocalDirectoryInfo,DirectoryInfo paramRemoteDirectoryInfo,bool paramRecursive = false)
        {
            if (paramLocalDirectoryInfo == null) throw new ArgumentNullException(nameof(paramLocalDirectoryInfo));
            if (paramRemoteDirectoryInfo == null) throw new ArgumentNullException(nameof(paramRemoteDirectoryInfo));
            if (paramLocalDirectoryInfo.Exists&&paramRemoteDirectoryInfo.Exists)
            {
                var files = paramRemoteDirectoryInfo.GetFiles();
                foreach (var file in files)
                {
                    File.Copy(file.FullName,Path.Combine(paramLocalDirectoryInfo.FullName,file.Name),true);
                }

                if (paramRecursive)
                {
                    var directories = paramRemoteDirectoryInfo.GetDirectories();
                    foreach (var directory in directories)
                    {
                        var subdirectory = paramLocalDirectoryInfo.GetDirectories().Any(p=>p.Name==directory.Name) ? paramLocalDirectoryInfo.GetDirectories().Single(p => p.Name == directory.Name) : paramLocalDirectoryInfo.CreateSubdirectory(directory.Name);
                        subdirectory.CopyFromDirectory(directory,true);
                    }
                }
            }
            else
            {
                throw new Exception($@"{paramLocalDirectoryInfo} not exist!");
            }
        }
        
        public static string GetUniqueNameByString(this CodeNamespace paramCodeNamespace,string paramString)
        {
            if (paramCodeNamespace == null) throw new ArgumentNullException(nameof(paramCodeNamespace));
            if (paramString == null) throw new ArgumentNullException(nameof(paramString));

            string result;
            var codeTypeDeclarations = paramCodeNamespace.Types.Cast<CodeTypeDeclaration>().ToArray();
            if (codeTypeDeclarations.All(p=>p.Name!=paramString))
            {
                result = paramString;
            }
            else
            {
                var enumerable = codeTypeDeclarations.Where(p => Regex.IsMatch(p.Name, $@"(?<={paramString}_)\d+"));
                result = $@"{paramString}_{enumerable.Count()}";
            }

            return result;
        }
        
        public static bool HasCodeTypeDeclarationOfName(this CodeNamespace paramCodeNamespace, string paramName)
        {
            if (paramCodeNamespace == null) throw new ArgumentNullException(nameof(paramCodeNamespace));
            if (paramName == null) throw new ArgumentNullException(nameof(paramName));
            
            return paramCodeNamespace.Types.Cast<CodeTypeDeclaration>().Any(p => p.Name == paramName);
        }

        public static CodeTypeDeclaration GetCodeTypeDeclarationByName(this CodeNamespace paramCodeNamespace, string paramString)
        {
            if (paramCodeNamespace == null) throw new ArgumentNullException(nameof(paramCodeNamespace));
            if (paramString == null) throw new ArgumentNullException(nameof(paramString));

            return paramCodeNamespace.Types.Cast<CodeTypeDeclaration>().Single(p => p.Name == paramString);
        }
    }
}