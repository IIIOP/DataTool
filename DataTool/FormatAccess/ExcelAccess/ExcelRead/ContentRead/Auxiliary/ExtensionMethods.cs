using System.Data;

namespace DataTool.FormatAccess.ExcelAccess.ExcelRead.ContentRead.Auxiliary
{
    public static class ExtensionMethods
    {
        public static void TrimStartRow(this DataTable dataTable, int count)
        {
            if (dataTable.Rows.Count>=count)
            {
                for (var i = 0; i < count; i++)
                {
                    dataTable.Rows.RemoveAt(0);
                }
            }
        }
        
        public static void TrimStartColumn(this DataTable dataTable, int count)
        {
            if (dataTable.Columns.Count>=count)
            {
                for (var i = 0; i < count; i++)
                {
                    dataTable.Columns.RemoveAt(0);
                }
            }
        }
        
        public static void TrimEndRow(this DataTable dataTable, int count)
        {
            var extraCount = dataTable.Rows.Count - count;
            if (extraCount>0)
            {
                for (var i = 0; i < extraCount; i++)
                {
                    dataTable.Rows.RemoveAt(count);
                }
            }
        }
        
        public static void TrimEndColumn(this DataTable dataTable, int count)
        {
            var extraCount = dataTable.Columns.Count - count;
            if (extraCount>0)
            {
                for (var i = 0; i < extraCount; i++)
                {
                    dataTable.Columns.RemoveAt(count);
                }
            }
        }
        
        public static DataTable TakeRow(this DataTable dataTable, int count)
        {
            var newDataTable = dataTable.Clone();
            for (var i = 0; i < count; i++)
            {
                var row = newDataTable.NewRow();
                newDataTable.Rows.Add(row);
                for (var j = 0; j < dataTable.Columns.Count; j++)
                {
                    row[j] = dataTable.Rows[i][j];
                }
            }
            return newDataTable;
        }
        
        public static DataTable TakeColumn(this DataTable dataTable, int count)
        {
            var newDataTable = dataTable.Copy();
            newDataTable.TrimEndColumn(count);
            return newDataTable;
        }
        
        public static DataTable TakeTable(this DataTable dataTable, int row,int column)
        {
            var table = dataTable.Copy();
            table.TrimEndRow(row);
            table.TrimEndColumn(column);
            return table;
        }
        
        public static DataTable TakeTable(this DataTable dataTable, int row1,int column1,int row2,int column2)
        {
            var table =  dataTable.Copy();
            table.TrimEndRow(row2);
            table.TrimEndColumn(column2);
            table.TrimStartRow(row1);
            table.TrimStartColumn(column1);
            return table;
        }
    }
}