using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using DataTool.FormatAccess.ExcelAccess.ExcelRead.ContentRead.BasicMultipleRead;
using DataTool.FormatAccess.ExcelAccess.ExcelRead.ContentRead.BasicSingleRead;
using DataToolInterface.Format.File.Excel;
using DataToolLog;

namespace DataTool.FormatAccess.ExcelAccess.ExcelRead.ContentRead.Auxiliary
{
    public class ValidityKeyInfo
    {
        private readonly FieldInfo _fieldInfo;
        private readonly DataTable _dataTable;
        private readonly Regex _regex;

        private readonly Attribute _attribute;
        
        public ValidityKeyInfo(FieldInfo paramFieldInfo,DataTable paramDataTable,Regex paramRegex)
        {
            _fieldInfo = paramFieldInfo;
            _dataTable = paramDataTable;
            _regex = paramRegex;

            if (_fieldInfo.GetCustomAttribute<SheetContentBasicSingleAttribute>() is SheetContentBasicSingleAttribute basicSingleAttribute)
            {
                _attribute = basicSingleAttribute;
            }
            else if (_fieldInfo.GetCustomAttribute<SheetContentBasicMultipleAttribute>() is SheetContentBasicMultipleAttribute basicMultipleAttribute)
            {
                _attribute = basicMultipleAttribute;
            }
            else
            {
                LogHelper.DefaultLog.WriteLine($@"[#####] 抱歉，目前key仅支持基本数据类型 [{_fieldInfo.Name}]");
            }
        }

        
        public bool CheckValid()
        {
            var result = false;
            if (_attribute is SheetContentBasicSingleAttribute basicSingleAttribute)
            {
                var contentBasicSingleReader = new BasicSingleReader(_fieldInfo.FieldType,_dataTable,basicSingleAttribute,new Position(),new StringBuilder());
                result = contentBasicSingleReader.isValid;
                if (result&&_regex!=null)
                {
                    result = _regex.IsMatch(contentBasicSingleReader.currentValue);
                }
            }
            else if (_attribute is SheetContentBasicMultipleAttribute basicMultipleAttribute)
            {
                var contentBasicMultipleReader = new BasicMultipleReader(_fieldInfo.FieldType,_dataTable,basicMultipleAttribute,new Position(),new StringBuilder());
                result = contentBasicMultipleReader.isValid;
                if (result&&_regex!=null)
                {
                    result = _regex.IsMatch(contentBasicMultipleReader.currentValue);
                }
            }

            return result;
        }
    }

    public class SpliterKeyInfo
    {
        private readonly List<SpliterKeyItemInfo> _spliterKeyItemInfos;
        public SpliterKeyInfo(Dictionary<FieldInfo,Regex> paramDictionary,string paramModelRow,string paramModelColumn,DataTable paramDataTable)
        {
            _spliterKeyItemInfos = new List<SpliterKeyItemInfo>();
            foreach (var item in paramDictionary)
            {
                _spliterKeyItemInfos.Add(new SpliterKeyItemInfo(item.Key,paramModelRow,paramModelColumn,paramDataTable,item.Value));
            }
        }

        public (List<int> row, List<int> column) GetSpliterUnionPosition()
        {
            var row = new List<int>();
            var column = new List<int>();

            foreach (var spliterKeyItemInfo in _spliterKeyItemInfos)
            {
                var subResult = spliterKeyItemInfo.GetSpliterPosition();
                row = row.Union(subResult.row).ToList();
                column = column.Union(subResult.column).ToList();
            }

            row.Sort();
            column.Sort();
            
            return (row,column);
        }
        
        public (List<int> row, List<int> column) GetSpliterIntersectPosition()
        {
            var row = _spliterKeyItemInfos.First().GetSpliterPosition().row;
            var column = _spliterKeyItemInfos.First().GetSpliterPosition().column;

            foreach (var spliterKeyItemInfo in _spliterKeyItemInfos)
            {
                var subResult = spliterKeyItemInfo.GetSpliterPosition();
                row = row.Intersect(subResult.row).ToList();
                column = column.Intersect(subResult.column).ToList();
            }

            row.Sort();
            column.Sort();
            
            return (row,column);
        }
    }
    
    public class SpliterKeyItemInfo
    {
        private readonly FieldInfo _fieldInfo;
        private readonly int _modelRow;
        private readonly int _modelColumn;
        private readonly RowColumnModeEnum _autoModeEnum;
        private readonly DataTable _dataTable;
        private readonly Regex _regex;

        private readonly Attribute _attribute;
        private readonly int _row;
        private readonly int _column;

        private (List<int> row, List<int> column) generatedPosition;
        public SpliterKeyItemInfo(FieldInfo paramFieldInfo,string paramModelRow,string paramModelColumn,DataTable paramDataTable,Regex paramRegex)
        {
            _fieldInfo = paramFieldInfo;
            int.TryParse(paramModelRow, out _modelRow);
            int.TryParse(paramModelColumn, out _modelColumn);
            _autoModeEnum = StaticMethods.GetAutoMode(paramModelRow, paramModelColumn);
            _dataTable = paramDataTable;
            _regex = paramRegex;

            if (_fieldInfo.GetCustomAttribute<SheetContentBasicSingleAttribute>() is SheetContentBasicSingleAttribute inputExcelSheetContentBasicSingleAttribute)
            {
                _attribute = inputExcelSheetContentBasicSingleAttribute;
                _row = int.Parse(inputExcelSheetContentBasicSingleAttribute.row);
                _column = int.Parse(inputExcelSheetContentBasicSingleAttribute.column);
            }
            else if (_fieldInfo.GetCustomAttribute<SheetContentBasicMultipleAttribute>() is SheetContentBasicMultipleAttribute inputExcelSheetContentBasicMultipleAttribute)
            {
                _attribute = inputExcelSheetContentBasicMultipleAttribute;
                _row = int.Parse(inputExcelSheetContentBasicMultipleAttribute.row);
                _column = int.Parse(inputExcelSheetContentBasicMultipleAttribute.column);
            }
            else
            {
                LogHelper.DefaultLog.WriteLine($@"[Error] key只能是基本数据类型哦 [{_fieldInfo.Name}");
            }

            GenerateSpliterPosition();
        }

        private void GenerateSpliterPosition()
        {
            switch (_autoModeEnum)
            {
                case RowColumnModeEnum.Non:
                    generatedPosition = GetSpliterPositionForNon();
                    break;
                case RowColumnModeEnum.Row:
                    generatedPosition = GetSpliterPositionForRow();
                    break;
                case RowColumnModeEnum.Column:
                    generatedPosition = GetSpliterPositionForColumn();
                    break;
                case RowColumnModeEnum.RowColumn:
                    generatedPosition = GetSpliterPositionForRowColumn();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public (List<int> row, List<int> column) GetSpliterPosition()
        {
            return generatedPosition;
        }

        private (List<int> row, List<int> column) GetSpliterPositionForNon()
        {
            var row = new List<int>();
            var index = 0;
            do
            {
                row.Add(index);
                index += _modelRow;
            } while (index<_dataTable.Rows.Count);

            var column = new List<int>();
            index = 0;
            do
            {
                column.Add(index);
                index += _modelColumn;
            } while (index<_dataTable.Columns.Count);

            return (row, column);
        }

        private (List<int> row, List<int> column) GetSpliterPositionForRow()
        {
            var row = GetRowSpliterPosition();

            var column = new List<int>();
            var index = 0;
            do
            {
                column.Add(index);
                index += _modelColumn;
            } while (index<_dataTable.Columns.Count);

            return (row, column);
        }

        private (List<int> row, List<int> column) GetSpliterPositionForColumn()
        {
            var row = new List<int>();
            var index = 0;
            do
            {
                row.Add(index);
                index += _modelRow;
            } while (index<_dataTable.Rows.Count);

            List<int> column = GetColumnSpliterPosition();

            return (row, column);
        }

        private (List<int> row, List<int> column) GetSpliterPositionForRowColumn()
        {
            var row = GetRowSpliterPosition();
            var column = GetColumnSpliterPosition();

            return (row, column);
        }

        private List<int> GetRowSpliterPosition()
        {
            var list = new List<int>();
            for (var i = 0; i < _dataTable.Rows.Count; i++)
            {
                if (CheckValid(_row+i,_column))
                {
                    list.Add(i);
                }
            }
            return list;
        }

        private List<int> GetColumnSpliterPosition()
        {
            var list = new List<int>();
            for (var i = 0; i < _dataTable.Columns.Count; i++)
            {
                if (CheckValid(_row,_column+i))
                {
                    list.Add(i);
                }
            }
            return list;
        }

        private bool CheckValid(int paramRow,int paramColumn)
        {
            var result = false;

            if (_attribute is SheetContentBasicSingleAttribute sheetContentBasicSingleAttribute)
            {
                sheetContentBasicSingleAttribute.row = paramRow.ToString();
                sheetContentBasicSingleAttribute.column = paramColumn.ToString();
                var contentBasicSingleReader = 
                    new BasicSingleReader(_fieldInfo.FieldType,_dataTable,sheetContentBasicSingleAttribute,new Position(),new StringBuilder());
                result = contentBasicSingleReader.isValid;
                if (result&&_regex!=null)
                {
                    result = _regex.IsMatch(contentBasicSingleReader.currentValue);
                }
            }
            else if (_attribute is SheetContentBasicMultipleAttribute sheetContentBasicMultipleAttribute)
            {
                sheetContentBasicMultipleAttribute.row = paramRow.ToString();
                sheetContentBasicMultipleAttribute.column = paramColumn.ToString();
                var contentBasicMultipleReader = 
                    new BasicMultipleReader(_fieldInfo.FieldType,_dataTable,sheetContentBasicMultipleAttribute,new Position(),new StringBuilder());
                result = contentBasicMultipleReader.isValid;
                if (result&&_regex!=null)
                {
                    result = _regex.IsMatch(contentBasicMultipleReader.currentValue);
                }
            }

            return result;
        }
    }
}