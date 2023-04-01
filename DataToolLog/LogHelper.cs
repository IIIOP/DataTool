using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DataToolLog
{
    public class LogHelper
    {
        private static readonly List<string> LogList;
        public static readonly LogHelper DefaultLog;
        
        private readonly TextWriter _textWriter;
        public Action<string> ExtraLog;
        static LogHelper()
        {
            LogList = new List<string>();
            DefaultLog = new LogHelper();
        }
        
        public LogHelper(string paramLogName = "default.log")
        {
            if (!LogList.Contains(paramLogName))
            {
                LogList.Add(paramLogName);
                _textWriter = new StreamWriter(Path.Combine(Environment.CurrentDirectory,paramLogName), false, Encoding.UTF8);
                _textWriter = new IndentedTextWriter(_textWriter);
            }
            else
            {
                throw new Exception("Log Name Repeat!!!");
            }
        }
        public void WriteLine(string paramLog)
        {
            Console.WriteLine(paramLog);
            _textWriter.WriteLine(paramLog);
            _textWriter.Flush();
            ExtraLog?.Invoke(paramLog+"\r\n");
        }
    }
}