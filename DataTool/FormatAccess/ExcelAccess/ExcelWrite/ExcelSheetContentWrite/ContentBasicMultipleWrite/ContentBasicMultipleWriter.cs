using System.Collections;
using System.Data;
using System.Text;
using DataToolInterface.Format.File.Excel;

namespace DataTool.FormatAccess.ExcelAccess.ExcelWrite.ExcelSheetContentWrite.ContentBasicMultipleWrite
{
    public class ContentBasicMultipleWriter
    {
        private int _row;
        private int _column;
        private readonly object _object;
        private readonly DataTable _dataTable;
        private readonly SheetContentBasicMultipleAttribute _sheetContentBasicMultipleAttribute;
        
        public int edgeRow => _row+1;
        public int edgeColumn => _column+1;
        public ContentBasicMultipleWriter(int paramRow,int paramColumn,object paramObject,DataTable paramDatable,SheetContentBasicMultipleAttribute paramSheetContentBasicMultipleAttribute)
        {
            _row = paramRow;
            _column = paramColumn;
            _object = paramObject;
            _dataTable = paramDatable;
            _sheetContentBasicMultipleAttribute = paramSheetContentBasicMultipleAttribute;

            DoWork();
        }

        private void DoWork()
        {
            var row = int.Parse(_sheetContentBasicMultipleAttribute.row);
            var column = int.Parse(_sheetContentBasicMultipleAttribute.column);
            _row = _row + row - 1;
            _column = _column + column - 1;
            
            var stringBuilder = new StringBuilder();

            char spliter = ',';
            if (!string.IsNullOrEmpty(_sheetContentBasicMultipleAttribute.spliter))
            {
                spliter = _sheetContentBasicMultipleAttribute.spliter[0];
            }
            if (_object is IList list)
            {
                foreach (var t in list)
                {
                    stringBuilder.Append(t.ToString() + spliter);
                }
            }
            
            while (_dataTable.Columns.Count<_column+1)
            {
                _dataTable.Columns.Add(new DataColumn(null, typeof(string)));
            }

            while (_dataTable.Rows.Count<_row+1)
            {
                _dataTable.Rows.Add(_dataTable.NewRow());
            }
            
            _dataTable.Rows[_row][_column] = stringBuilder.ToString().TrimEnd(spliter);
        }
    }
}