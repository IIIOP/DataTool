using System;
using System.IO;
using System.Reflection;
using DataToolLog;

namespace DataTool
{
    internal static class Program
    {
        public static void Main(string[] paramArgs)
        {
            try
            {
                DataToolFile dataToolFile;
                if (paramArgs[0].EndsWith(".dll"))
                {
                    dataToolFile = DataToolFile.GetDataToolFileByAssembly(Assembly.LoadFile(paramArgs[0]));
                }
                else
                {
                    dataToolFile = DataToolFile.GetDataToolFileByDirectory(new DirectoryInfo(paramArgs[0]));
                }

                if (paramArgs[1]=="generate")
                {
                    dataToolFile.ReadInput();
                    dataToolFile.WriteInput();
                    dataToolFile.CheckConsistency();
                    dataToolFile.GenerateOutput();
                    dataToolFile.WriteOutput();
                }
                else if (paramArgs[1]=="test")
                {
                    dataToolFile.ReadInput();
                    dataToolFile.ReadOutput();
                    dataToolFile.TestCase();
                }
                
                dataToolFile.Serialize();
            }
            catch (Exception e)
            {
                LogHelper.DefaultLog.WriteLine(e.ToString());
            }
            
            LogHelper.DefaultLog.WriteLine("Done!");
            //Console.ReadLine();
        }
    }
}
