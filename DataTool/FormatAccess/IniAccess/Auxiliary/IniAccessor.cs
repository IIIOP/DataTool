using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DataTool.FormatAccess.IniAccess.Auxiliary
{
    public class IniAccessor
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32", EntryPoint = "GetPrivateProfileString")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        [DllImport("kernel32", EntryPoint = "GetPrivateProfileString")]
        private static extern uint GetPrivateProfileStringA(string section, string key, string def, Byte[] retVal, int size, string filePath);
        
        public string _iniPath;
        public IniAccessor(string paramIniPath)
        {
            _iniPath = paramIniPath;
        }

        /// <summary> 
        /// 写入INI文件 
        /// </summary> 
        /// <param name="Section">项目名称(如 [TypeName] )</param> 
        /// <param name="Key">键</param> 
        /// <param name="Value">值</param> 
        public void WriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, _iniPath);
        }
        
        /// <summary> 
        /// 读出INI文件 
        /// </summary> 
        /// <param name="paramSection">项目名称(如 [TypeName] )</param> 
        /// <param name="paramKey">键</param> 
        public string ReadValue(string paramSection, string paramKey)
        {
            var temp = new StringBuilder(500);
            var i = GetPrivateProfileString(paramSection, paramKey, "", temp, 500, _iniPath);

            var result = temp.ToString().Trim();
            if(result.Contains(';'))
            {
                result = result.Substring(0, result.IndexOf(';'));
            }
            if (result.Contains("//"))
            {
                result = result.Substring(0, result.IndexOf("//"));
            }
            if (result.Contains('#'))
            {
                result = result.Substring(0, result.IndexOf('#'));
            }
            if (result.Contains(' '))
            {
                result = result.Substring(0, result.IndexOf(' '));
            }
            return result;
        }

        public List<string> GetAllSections()
        {
            var result = new List<string>();
            var buf = new byte[65536];
            var len = GetPrivateProfileStringA(null, null, null, buf, buf.Length, _iniPath);
            var j = 0;
            for (var i = 0; i < len; i++)
            {
                if (buf[i] == 0)
                {
                    result.Add(Encoding.Default.GetString(buf, j, i - j));
                    j = i + 1;
                }
            }
                
            return result;
        }

        public List<string> GetSectionAllKeys(string paramSectionName)
        {
            var result = new List<string>();
            var buf = new byte[65536];
            var len = GetPrivateProfileStringA(paramSectionName, null, null, buf, buf.Length, _iniPath);
            var j = 0;
            for (var i = 0; i < len; i++)
            {
                if (buf[i] == 0)
                {
                    result.Add(Encoding.Default.GetString(buf, j, i - j));
                    j = i + 1;
                }
            } 
            return result;
        }
    }
}