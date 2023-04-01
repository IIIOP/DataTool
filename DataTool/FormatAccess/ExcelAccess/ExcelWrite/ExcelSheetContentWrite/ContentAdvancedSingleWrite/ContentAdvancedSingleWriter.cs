using System.Data;
using System.Reflection;
using DataTool.FormatAccess.ExcelAccess.ExcelWrite.ExcelSheetContentWrite.ContentAdvancedMultipleWrite;
using DataTool.FormatAccess.ExcelAccess.ExcelWrite.ExcelSheetContentWrite.ContentBasicMultipleWrite;
using DataTool.FormatAccess.ExcelAccess.ExcelWrite.ExcelSheetContentWrite.ContentBasicSingleWrite;
using DataToolInterface.Format.File.Excel;

namespace DataTool.FormatAccess.ExcelAccess.ExcelWrite.ExcelSheetContentWrite.ContentAdvancedSingleWrite
{
    public class ContentAdvancedSingleWriter
    {
        private int _row;
        private int _column;
        private readonly object _object;
        private readonly DataTable _dataTable;
        private readonly SheetContentAdvancedSingleAttribute _sheetContentAdvancedSingleAttribute;
        
        public int edgeRow { get; private set; }
        public int edgeColumn { get; private set; }

        public ContentAdvancedSingleWriter(int paramRow,int paramColumn,object paramObject, DataTable paramDataTable, SheetContentAdvancedSingleAttribute paramSheetContentAdvancedSingleAttribute)
        {
            _row = paramRow;
            _column = paramColumn;
            _object = paramObject;
            _dataTable = paramDataTable;
            _sheetContentAdvancedSingleAttribute = paramSheetContentAdvancedSingleAttribute;

            DoWork();
        }

        private void DoWork()
        {
            var row = int.Parse(_sheetContentAdvancedSingleAttribute.row);
            var column = int.Parse(_sheetContentAdvancedSingleAttribute.column);
            _row = _row + row - 1;
            _column = _column + column - 1;

            foreach (var fieldInfo in _object.GetType().GetFields())
            {
                var obj = fieldInfo.GetValue(_object);

                if (fieldInfo.GetCustomAttribute<SheetContentAdvancedMultipleAttribute>() is SheetContentAdvancedMultipleAttribute sheetContentAdvancedMultipleAttribute)
                {
                    var contentAdvancedMultipleWriter = new ContentAdvancedMultipleWriter(_row,_column,obj,_dataTable,sheetContentAdvancedMultipleAttribute);
                    if (contentAdvancedMultipleWriter.edgeRow>edgeRow)
                    {
                        edgeRow = contentAdvancedMultipleWriter.edgeRow;
                    }
                    if (contentAdvancedMultipleWriter.edgeColumn>edgeColumn)
                    {
                        edgeColumn = contentAdvancedMultipleWriter.edgeColumn;
                    }
                }
                else if (fieldInfo.GetCustomAttribute<SheetContentAdvancedSingleAttribute>() is SheetContentAdvancedSingleAttribute sheetContentAdvancedSingleAttribute)
                {
                    var contentAdvancedSingleWriter = new ContentAdvancedSingleWriter(_row, _column, obj, _dataTable, sheetContentAdvancedSingleAttribute);
                    if (contentAdvancedSingleWriter.edgeRow>edgeRow)
                    {
                        edgeRow = contentAdvancedSingleWriter.edgeRow;
                    }
                    if (contentAdvancedSingleWriter.edgeColumn>edgeColumn)
                    {
                        edgeColumn = contentAdvancedSingleWriter.edgeColumn;
                    }
                }
                else if (fieldInfo.GetCustomAttribute<SheetContentBasicMultipleAttribute>() is SheetContentBasicMultipleAttribute sheetContentBasicMultipleAttribute)
                {
                    var contentBasicMultipleWriter = new ContentBasicMultipleWriter(_row, _column, obj, _dataTable, sheetContentBasicMultipleAttribute);
                    if (contentBasicMultipleWriter.edgeRow>edgeRow)
                    {
                        edgeRow = contentBasicMultipleWriter.edgeRow;
                    }
                    if (contentBasicMultipleWriter.edgeColumn>edgeColumn)
                    {
                        edgeColumn = contentBasicMultipleWriter.edgeColumn;
                    }
                }
                else if (fieldInfo.GetCustomAttribute<SheetContentBasicSingleAttribute>() is SheetContentBasicSingleAttribute sheetContentBasicSingleAttribute)
                {
                    var contentBasicSingleWriter = new ContentBasicSingleWriter(_row, _column, obj, _dataTable, sheetContentBasicSingleAttribute);
                    if (contentBasicSingleWriter.edgeRow>edgeRow)
                    {
                        edgeRow = contentBasicSingleWriter.edgeRow;
                    }
                    if (contentBasicSingleWriter.edgeColumn>edgeColumn)
                    {
                        edgeColumn = contentBasicSingleWriter.edgeColumn;
                    }
                }
            }

            if (_sheetContentAdvancedSingleAttribute.modelRowCount!=ConfigRowColumnValueEnum.Auto.ToString())
            {
                edgeRow = _row + int.Parse(_sheetContentAdvancedSingleAttribute.modelRowCount);
            }

            if (_sheetContentAdvancedSingleAttribute.modelColumnCount!=ConfigRowColumnValueEnum.Auto.ToString())
            {
                edgeColumn = _column + int.Parse(_sheetContentAdvancedSingleAttribute.modelColumnCount);
            }
        }
    }
}