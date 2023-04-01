using System;
using System.Linq;
using System.Text.RegularExpressions;
using DataToolInterface.Format.Config.Global;
using DataToolInterface.Format.Config.Input;
using DataToolInterface.Format.File;

namespace DataTool.Auxiliary
{
    public static class ExtentMethods
    {
        public static string FilterLayerByPath(this string param, string paramPath)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));
            if (paramPath == null) throw new ArgumentNullException(nameof(paramPath));
            
            var index = 0;
            var result = param.FormatToPath();
            var layer = paramPath.FormatToPath().Split('\\').Length;
            if (layer<result.Split('\\').Length)
            {
                for (var i = 0; i < layer-1; i++)
                {
                    result = result.Substring(0, result.LastIndexOf('\\'));
                    index = result.LastIndexOf('\\');
                }
            }
            return param.Substring(index);
        }
        public static string FormatToPath(this string param)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));
            
            var result = param;
            result = result.Replace('/', '\\').TrimStart(@".\".ToCharArray()).TrimEnd('\\');
            return result+"\\";
        }
        public static string DecodeGlobal(this string param,string paramFullVariable,string paramFullActual)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));
            if (paramFullVariable == null) throw new ArgumentNullException(nameof(paramFullVariable));
            if (paramFullActual == null) throw new ArgumentNullException(nameof(paramFullActual));
            
            string result = null;
            var pattern = $@"{{global:{param}}}";
            if (paramFullVariable.Contains(pattern))
            {
                var prefix = paramFullVariable.Substring(0, paramFullVariable.IndexOf(pattern, StringComparison.Ordinal));
                var postfix = paramFullVariable.Substring(paramFullVariable.IndexOf(pattern, StringComparison.Ordinal) + pattern.Length);
                var regex = new Regex($@"(?<={prefix.ReplaceVariableToRegex()}).+(?={postfix.ReplaceVariableToRegex()})");
                result = regex.Match(paramFullActual).Value;
            }
            return result;
        }
        public static string DecodeLocal(this string param,string paramFullVariable,string paramFullActual)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));
            if (paramFullVariable == null) throw new ArgumentNullException(nameof(paramFullVariable));
            if (paramFullActual == null) throw new ArgumentNullException(nameof(paramFullActual));

            paramFullVariable = paramFullVariable.FormatToPath();
            paramFullActual = paramFullActual.FormatToPath();
            
            string result = null;
            var pattern = $@"{{local:{param}}}";
            if (paramFullVariable.Contains(pattern))
            {
                var prefix = paramFullVariable.Substring(0, paramFullVariable.IndexOf(pattern, StringComparison.Ordinal));
                var postfix = paramFullVariable.Substring(paramFullVariable.IndexOf(pattern, StringComparison.Ordinal) + pattern.Length);
                var regex = new Regex($@"(?<={prefix.ReplaceVariableToRegex()})[^\\]+(?={postfix.ReplaceVariableToRegex()})");
                result = regex.Match(paramFullActual).Value;
            }
            return result;
        }
        private static string EncodeGlobal(this string fieldName, object paramObject)
        {
            if (fieldName == null) throw new ArgumentNullException(nameof(fieldName));
            if (paramObject == null) throw new ArgumentNullException(nameof(paramObject));
            
            var property = paramObject.GetType().GetProperties().Single(p => p.Name == FileInfoEnum.fileInfo.ToString()||p.Name == InputEnum.pathInfo.ToString());
            var target = property.GetValue(paramObject);

            var field = target.GetType().GetFields().Single(p => p.Name == GlobalEnum.Global.ToString());
            target = field.GetValue(target);

            field = target.GetType().GetFields().Single(p => p.Name == fieldName);
            return field.GetValue(target).ToString();
        }
        private static string EncodeLocal(this string fieldName, object paramObject)
        {
            if (fieldName == null) throw new ArgumentNullException(nameof(fieldName));
            if (paramObject == null) throw new ArgumentNullException(nameof(paramObject));
            
            var property = paramObject.GetType().GetProperties().Single(p => p.Name == FileInfoEnum.fileInfo.ToString()||p.Name == InputEnum.pathInfo.ToString());
            var target = property.GetValue(paramObject);

            var field = target.GetType().GetFields().Single(p => p.Name == fieldName);
            return field.GetValue(target).ToString();
        }
        public static string EncodeVariable(this string param,object paramObject)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));
            if (paramObject == null) throw new ArgumentNullException(nameof(paramObject));

            var result = param;
            
            var localRegex = new Regex(@"(?<={local:)\w+(?=})");
            var globalRegex = new Regex(@"(?<={global:)\w+(?=})");
            
            var regex = new Regex(@"{local:\w+}");
            result = regex.Matches(result).Cast<Match>()
                .Aggregate(result, (current, match) => current.Replace(match.Value,localRegex.Match(match.Value).Value.EncodeLocal(paramObject)));
            
            regex = new Regex(@"{global:\w+}");
            result = regex.Matches(result).Cast<Match>()
                .Aggregate(result, (current, match) => current.Replace(match.Value,globalRegex.Match(match.Value).Value.EncodeGlobal(paramObject)));
            
            return result;
        }

        public static string ReplaceVariableToRegex(this string paramString)
        {
            const string replaceValue = @"[^\\]+";
            
            var result = paramString;
            var regex = new Regex(@"({local:\w+})|({global:\w+})");
            result = Regex.Escape(regex.Replace(result, replaceValue))
                .Replace(Regex.Escape(replaceValue), replaceValue);
            return result;
        }
        

        public static string GetPathBeforeVariable(this string paramString)
        {
            var result = paramString.FormatToPath();
            var regex = new Regex(@"({local:\w+})|({global:\w+})");
            if (regex.IsMatch(paramString))
            {
                result = result.Substring(0, regex.Match(paramString).Index);
                result = result.Substring(0, result.IndexOf('\\')!=-1?result.LastIndexOf('\\'):0);
            }
            return result.TrimEnd('\\');
        }

        public static object GetFieldValueByName(this object paramObject,string paramField)
        {
            if (paramObject == null) throw new ArgumentNullException(nameof(paramObject));
            if (paramField == null) throw new ArgumentNullException(nameof(paramField));

            var fieldNames = paramField.Trim().Split('.');
            var currentObject = paramObject;
            foreach (var fieldName in fieldNames)
            {
                var field = currentObject.GetType().GetFields().Single(p => p.Name == fieldName);
                currentObject = field.GetValue(currentObject);
            }
            return currentObject;
        }
    }
}