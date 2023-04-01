using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Runtime.InteropServices;

namespace DataTool.FormatAccess.ExcelAccess.Auxiliary
{
    public static class ExcelHelper
    {
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(int paramHd, out int paramId);
        
        public static DataSet GetDataSet(FileInfo paramFileInfo)
        {
            if (paramFileInfo == null) throw new ArgumentNullException(nameof(paramFileInfo));

            if (paramFileInfo.Exists)
            {
                var temp = Path.Combine(Environment.CurrentDirectory,$@"temp_{paramFileInfo.Name}");
                File.Copy(paramFileInfo.FullName,temp,true);
                var fileInfo = new FileInfo(temp)
                {
                    IsReadOnly = false
                };
                var connectString = CreateDataSource(fileInfo);
                var oleDbLock = new OleDbConnection(connectString);
                var devsTable = ExcelToDataSet(oleDbLock);
                File.Delete(temp);
                return devsTable;
            }
            throw new Exception($@"Excel not exist!");
        }

        private static string CreateDataSource(FileInfo paramFile)
        {
            if (paramFile == null) throw new ArgumentNullException(nameof(paramFile));

            if (paramFile.Exists)
            {
                var filePath = paramFile.FullName;
                var connectString = "";
                switch (Path.GetExtension(filePath).ToUpper())
                {
                    case ".XLS":
                        connectString = $@"Provider = Microsoft.Jet.OLEDB.4.0;Data Source ={filePath};Extended Properties='Excel 8.0;HDR=NO;IMEX=1'";
                        break;
                    case ".XLSX":
                    case ".XLSM":
                        connectString = $@"Provider = Microsoft.Ace.OleDb.12.0;Data Source={filePath};Extended Properties='Excel 12.0;HDR=NO'";
                        break;
                }
                return connectString;
            }
            throw new Exception($@"File not exist!");
        }

        private static DataSet ExcelToDataSet(OleDbConnection paramConnection)
        {
            if (paramConnection == null) throw new ArgumentNullException(nameof(paramConnection));
            
            var dataSet = new DataSet();
            var fileInfo = new FileInfo(paramConnection.DataSource);
            dataSet.DataSetName = fileInfo.Name;
            paramConnection.Open();
            var sheets = paramConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            foreach (DataRow row in sheets.Rows)
            {
                var sheetName = row["TABLE_NAME"].ToString();
                if (sheetName.EndsWith("$")||(sheetName.StartsWith("'")&&sheetName.EndsWith("$'")))
                {
                    sheetName = sheetName.TrimStart('\'').TrimEnd('\'');
                    var strExcel = $"select * from [{sheetName}A:IU]";
                    var myCommand = new OleDbDataAdapter(strExcel, paramConnection);

                    myCommand.Fill(dataSet, sheetName.TrimEnd('$'));
                }
            }
            paramConnection.Close();
            return dataSet;
        }
        
        public static string GetColumnChar(this int paramRowCount)
        {
            paramRowCount--;
            var offset = paramRowCount % 26;
            var reserve = paramRowCount / 26;

            var result = (char) ('A' + offset);
            var subResult = "";
            if (reserve>0)
            {
                subResult = GetColumnChar(reserve);
            }

            return subResult + result;
        }
    }
}