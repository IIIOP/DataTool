using System;
using System.Data;
using DataToolInterface.Data;
using DataToolInterface.Format.File.Excel;

namespace DataTool.FormatAccess.ExcelAccess.ExcelWrite.ExcelSheetContentWrite.ContentBasicSingleWrite
{
    public class ContentBasicSingleWriter
    {
        private int _row;
        private int _column;
        private readonly object _object;
        private readonly DataTable _dataTable;
        private readonly SheetContentBasicSingleAttribute _sheetContentBasicSingleAttribute;

        public int edgeRow => _row+1;
        public int edgeColumn => _column+1;

        public ContentBasicSingleWriter(int paramRow,int paramColumn,object paramObject,DataTable paramDatable,SheetContentBasicSingleAttribute paramSheetContentBasicSingleAttribute)
        {
            _row = paramRow;
            _column = paramColumn;
            _object = paramObject;
            _dataTable = paramDatable;
            _sheetContentBasicSingleAttribute = paramSheetContentBasicSingleAttribute;

            DoWork();
        }

        private void DoWork()
        {
            var row = int.Parse(_sheetContentBasicSingleAttribute.row);
            var column = int.Parse(_sheetContentBasicSingleAttribute.column);
            _row = _row + row - 1;
            _column = _column + column - 1;

            while (_dataTable.Columns.Count<_column+1)
            {
                _dataTable.Columns.Add(new DataColumn(null, typeof(string)));
            }

            while (_dataTable.Rows.Count<_row+1)
            {
                _dataTable.Rows.Add(_dataTable.NewRow());
            }

            _dataTable.Rows[_row][_column] = _object?.ToString();
            
            var type = (BasicDataTypeEnum)Enum.Parse(typeof(BasicDataTypeEnum), _sheetContentBasicSingleAttribute.type);
            switch (type)
            {
                case BasicDataTypeEnum.INT8:
                case BasicDataTypeEnum.INT16:
                case BasicDataTypeEnum.INT32:
                case BasicDataTypeEnum.INT64:
                case BasicDataTypeEnum.UINT8:
                case BasicDataTypeEnum.UINT16:
                case BasicDataTypeEnum.UINT32:
                case BasicDataTypeEnum.UINT64:
                case BasicDataTypeEnum.FLOAT:
                case BasicDataTypeEnum.DOUBLE:
                case BasicDataTypeEnum.DECIMAL:
                    if (_object.ToString()=="0")
                    {
                        if (!string.IsNullOrEmpty(_sheetContentBasicSingleAttribute.allowNull))
                        {
                            var allowNull = bool.Parse(_sheetContentBasicSingleAttribute.allowNull);
                            if (allowNull)
                            {
                                _dataTable.Rows[_row][_column] = null;
                            }
                        }
                    }
                    break;
                case BasicDataTypeEnum.String:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}