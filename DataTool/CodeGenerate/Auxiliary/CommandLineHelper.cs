using System;
using System.Diagnostics;
using DataToolLog;

namespace DataTool.CodeGenerate.Auxiliary
{
    public static class CommandLineHelper
    {
        public static void CallCommandLine(string paramCommand)
        {
            if (string.IsNullOrWhiteSpace(paramCommand))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(paramCommand));
            
            var process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            
            process.Start();
            process.StandardInput.WriteLine(paramCommand + "&exit");
            process.StandardInput.AutoFlush = true;
            
            LogHelper.DefaultLog.WriteLine(process.StandardOutput.ReadToEnd());
            process.WaitForExit();
            process.Close();
        }
    }
}