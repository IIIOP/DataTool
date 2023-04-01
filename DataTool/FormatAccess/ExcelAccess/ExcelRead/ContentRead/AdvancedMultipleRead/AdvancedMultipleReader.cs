using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using DataTool.FormatAccess.ExcelAccess.ExcelRead.ContentRead.AdvancedSingleRead;
using DataTool.FormatAccess.ExcelAccess.ExcelRead.ContentRead.Auxiliary;
using DataToolInterface.Data;
using DataToolInterface.Format.File.Excel;

namespace DataTool.FormatAccess.ExcelAccess.ExcelRead.ContentRead.AdvancedMultipleRead
{
    public class AdvancedMultipleReader
    {
        private readonly Type _resultType;
        private readonly DataTable _dataTable;
        private readonly SheetContentAdvancedMultipleAttribute _sheetContentAdvancedMultipleAttribute;
        private readonly Position _position;
        private readonly StringBuilder _stringBuilder;

        private SpliterKeyInfo _spliterKeyInfo;

        private Dictionary<Position,DataTable> _dataTables;
        public object resultObject { get; private set; }

        public AdvancedMultipleReader(Type paramType,DataTable paramDataTable,SheetContentAdvancedMultipleAttribute paramSheetContentAdvancedMultipleAttribute,Position paramPosition,StringBuilder paramStringBuilder)
        {
            if (paramType == null) throw new ArgumentNullException(nameof(paramType));
            if (paramDataTable == null) throw new ArgumentNullException(nameof(paramDataTable));
            if (paramSheetContentAdvancedMultipleAttribute == null)
                throw new ArgumentNullException(nameof(paramSheetContentAdvancedMultipleAttribute));
            if (paramPosition == null) throw new ArgumentNullException(nameof(paramPosition));
            if (paramStringBuilder == null) throw new ArgumentNullException(nameof(paramStringBuilder));
            
            _resultType = paramType;
            _dataTable = paramDataTable.Copy();
            _sheetContentAdvancedMultipleAttribute = paramSheetContentAdvancedMultipleAttribute;
            _position = paramPosition;
            _stringBuilder = paramStringBuilder;

            ReadAdvancedMultiple();
        }
        
        private void InitWithConfig()
        {
            var row = UInt16.Parse(_sheetContentAdvancedMultipleAttribute.row);
            var column = UInt16.Parse(_sheetContentAdvancedMultipleAttribute.column);
            UInt16.TryParse(_sheetContentAdvancedMultipleAttribute.totalRowCount, out var totalRowCount);
            UInt16.TryParse(_sheetContentAdvancedMultipleAttribute.totalColumnCount, out var totalColumnCount);
            
            _dataTable.TrimStartRow(row-1);
            _dataTable.TrimStartColumn(column-1);
            _position.row += row - 1;
            _position.column += column - 1;
            if (totalRowCount>0)
            {
                _dataTable.TrimEndRow(totalRowCount);
            }
            if (totalColumnCount>0)
            {
                _dataTable.TrimEndColumn(totalColumnCount);
            }

            if (!string.IsNullOrWhiteSpace(_sheetContentAdvancedMultipleAttribute.key))
            {
                var keys = _sheetContentAdvancedMultipleAttribute.key.Split(',').ToList();
                var patterns = new List<Regex>();
                if (!string.IsNullOrWhiteSpace(_sheetContentAdvancedMultipleAttribute.keyPattern))
                {
                    var list = _sheetContentAdvancedMultipleAttribute.keyPattern.Split(',');
                    foreach (var item in list)
                    {
                        if (item==String.Empty)
                        {
                            patterns.Add(null);
                        }
                        else
                        {
                            patterns.Add(new Regex(item));
                        }
                    }
                }

                if (!patterns.Any()||keys.Count==patterns.Count)
                {
                    var keyDictionary = new Dictionary<FieldInfo, Regex>();
                    for (var i = 0; i < keys.Count; i++)
                    {
                        var field = resultObject.GetType().GetGenericArguments()[0].GetFields()
                            .Single(item => item.Name == keys[i]);
                        if (patterns.Any())
                        {
                            keyDictionary.Add(field,patterns[i]);
                        }
                        else
                        {
                            keyDictionary.Add(field,null);
                        }
                    }

                    _spliterKeyInfo = new SpliterKeyInfo(keyDictionary, _sheetContentAdvancedMultipleAttribute.modelRowCount,
                        _sheetContentAdvancedMultipleAttribute.modelColumnCount, _dataTable);

                    _dataTables = new Dictionary<Position, DataTable>();
                }
                else
                {
                    throw new Exception($@"[######] key和keypattern的数量不一致 [{_sheetContentAdvancedMultipleAttribute.describe}]");
                }
            }
            else
            {
                throw new Exception("no key defined!!");
            }
        }
       
        private void DoSplit()
        {
            var split = _spliterKeyInfo.GetSpliterUnionPosition();

            if (split.row.Count==0||split.column.Count==0)
            {
                return;
            }
            
            if (split.row.First()!=0)
            {
                _dataTable.TrimStartRow(split.row.First());
            }
            
            if (split.column.First()!=0)
            {
                _dataTable.TrimStartColumn(split.column.First());
            }

            var dataTables = new Dictionary<int, DataTable>();
            for (var i = 1; i < split.row.Count; i++)
            {
                var count = split.row[i] - split.row[i - 1];
                dataTables.Add(split.row[i-1],_dataTable.TakeRow(count));
                _dataTable.TrimStartRow(count);
            }
            dataTables.Add(split.row.Last(),_dataTable);

            foreach (var dataTable in dataTables)
            {
                for (var i = 1; i < split.column.Count; i++)
                {
                    var count = split.column[i] - split.column[i - 1];
                    _dataTables.Add(new Position(dataTable.Key,split.column[i-1]),dataTable.Value.TakeColumn(count));
                    dataTable.Value.TrimStartColumn(count);
                }
                _dataTables.Add(new Position(dataTable.Key,split.column.Last()),dataTable.Value);
            }
        }
        
        private void ReadAdvancedMultiple()
        {
            resultObject = Activator.CreateInstance(_resultType);
                
            InitWithConfig();

            DoSplit();
            
            var basicLastValueDictionary = new Dictionary<FieldInfo, string>();
            foreach (var dataTable in _dataTables)
            {
                var position = new Position(_position.row+dataTable.Key.row,_position.column+dataTable.Key.column);
                
                var advancedSingleAttribute = new SheetContentAdvancedSingleAttribute()
                    {
                        row = 1.ToString(),
                        column = 1.ToString(),
                        type = AdvancedDataTypeEnum.Single.ToString(),
                        modelRowCount = _sheetContentAdvancedMultipleAttribute.modelRowCount,
                        modelColumnCount = _sheetContentAdvancedMultipleAttribute.modelColumnCount,
                        key = _sheetContentAdvancedMultipleAttribute.key,
                        keyPattern = _sheetContentAdvancedMultipleAttribute.keyPattern
                    };
                
                var baseType = resultObject.GetType().GetGenericArguments()[0];
                var contentAdvancedSingleReader = new AdvancedSingleReader(baseType,dataTable.Value,advancedSingleAttribute,position,_stringBuilder,basicLastValueDictionary);

                if (contentAdvancedSingleReader.isValid)
                {
                    if (resultObject is IList list)
                    {
                        list.Add(contentAdvancedSingleReader.resultObject);
                    }
                }
            }
        }
    }
}