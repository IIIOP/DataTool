using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using DataTool.FormatAccess.ExcelAccess.ExcelRead.ContentRead.AdvancedMultipleRead;
using DataTool.FormatAccess.ExcelAccess.ExcelRead.ContentRead.Auxiliary;
using DataTool.FormatAccess.ExcelAccess.ExcelRead.ContentRead.BasicMultipleRead;
using DataTool.FormatAccess.ExcelAccess.ExcelRead.ContentRead.BasicSingleRead;
using DataToolInterface.Format.File.Excel;

namespace DataTool.FormatAccess.ExcelAccess.ExcelRead.ContentRead.AdvancedSingleRead
{
    public class AdvancedSingleReader
    {
        private readonly Type _resultType;
        private readonly DataTable _dataTable;
        private readonly Position _position;
        private readonly StringBuilder _stringBuilder;
        private readonly Dictionary<FieldInfo, string> _lastValueDictionary;
        private readonly SheetContentAdvancedSingleAttribute _sheetContentAdvancedSingleAttribute;

        private UInt16 _row;
        private UInt16 _column;
        private UInt16 _modelRowCount;
        private UInt16 _modelColumnCount;
        private ValidityKeyInfo _validityKeyInfo;
        private Regex _regex;

        public bool isValid { get; private set; }
        public object resultObject { get; private set; }

        public AdvancedSingleReader(Type paramType, DataTable paramDataTable,SheetContentAdvancedSingleAttribute paramSheetContentAdvancedSingleAttribute,
            Position paramPosition,StringBuilder paramStringBuilder, Dictionary<FieldInfo, string> paramLastValueDictionary = null)
        {
            _resultType = paramType;
            _dataTable = paramDataTable.Copy();
            _sheetContentAdvancedSingleAttribute = paramSheetContentAdvancedSingleAttribute;
            _position = paramPosition;
            _stringBuilder = paramStringBuilder;
            _lastValueDictionary = paramLastValueDictionary;

            ReadAdvancedSingle();
        }

        private void ParseConfig()
        {
            _row = UInt16.Parse(_sheetContentAdvancedSingleAttribute.row);
            _column = UInt16.Parse(_sheetContentAdvancedSingleAttribute.column);
            UInt16.TryParse(_sheetContentAdvancedSingleAttribute.modelRowCount, out _modelRowCount);
            UInt16.TryParse(_sheetContentAdvancedSingleAttribute.modelColumnCount, out _modelColumnCount);
            _dataTable.TrimStartRow(_row-1);
            _dataTable.TrimStartColumn(_column-1);
            
            _position.row += _row - 1;
            _position.column += _column - 1;
            
            if (_modelRowCount>0)
            {
                _dataTable.TrimEndRow(_modelRowCount);
            }

            if (_modelColumnCount>0)
            {
                _dataTable.TrimEndColumn(_modelColumnCount);
            }
            
            if (!string.IsNullOrWhiteSpace(_sheetContentAdvancedSingleAttribute.keyPattern))
            {
                _regex = new Regex(_sheetContentAdvancedSingleAttribute.keyPattern);
            }

            if (!string.IsNullOrWhiteSpace(_sheetContentAdvancedSingleAttribute.key))
            {
                var keyFieldInfo = resultObject.GetType().GetFields()
                    .Single(item => item.Name == _sheetContentAdvancedSingleAttribute.key);
                _validityKeyInfo = new ValidityKeyInfo(keyFieldInfo, _dataTable, _regex);
            }
        }

        private void ReadAdvancedSingle()
        {
            resultObject = Activator.CreateInstance(_resultType);

            ParseConfig();
            
            var isKeyValid = false;
            if (_validityKeyInfo!=null)
            {
                isKeyValid = _validityKeyInfo.CheckValid();
            }
            else
            {
                isKeyValid = true;
            }
            
            if (isKeyValid)
            {
                isValid = true;
                foreach (var fieldInfo in resultObject.GetType().GetFields())
                {
                    string lastValue = null;
                    _lastValueDictionary?.TryGetValue(fieldInfo, out lastValue);
                    if (fieldInfo.GetCustomAttribute<SheetContentBasicSingleAttribute>() is SheetContentBasicSingleAttribute inputExcelSheetContentBasicSingleAttribute)
                    {
                        var contentBasicSingleReader = new BasicSingleReader(fieldInfo.FieldType,_dataTable,inputExcelSheetContentBasicSingleAttribute,_position.Clone(),_stringBuilder,lastValue);
                        if (contentBasicSingleReader.isValid)
                        {
                            fieldInfo.SetValue(resultObject,contentBasicSingleReader.ResultObject);
                        }
                    }
                    else if (fieldInfo.GetCustomAttribute<SheetContentBasicMultipleAttribute>() is SheetContentBasicMultipleAttribute inputExcelSheetContentBasicMultipleAttribute)
                    {
                        var contentBasicMultipleReader = new BasicMultipleReader(fieldInfo.FieldType,_dataTable,inputExcelSheetContentBasicMultipleAttribute,_position.Clone(),_stringBuilder,lastValue);
                        if (contentBasicMultipleReader.isValid)
                        {
                            fieldInfo.SetValue(resultObject,contentBasicMultipleReader.ResultObject);
                        }
                    }
                    else if (fieldInfo.GetCustomAttribute<SheetContentAdvancedSingleAttribute>() is SheetContentAdvancedSingleAttribute inputExcelSheetContentAdvancedSingleAttribute)
                    {
                        var contentAdvancedSingleReader = new AdvancedSingleReader(fieldInfo.FieldType,_dataTable,inputExcelSheetContentAdvancedSingleAttribute,_position.Clone(),_stringBuilder);
                        if (contentAdvancedSingleReader.isValid)
                        {
                            fieldInfo.SetValue(resultObject,contentAdvancedSingleReader.resultObject);
                        }
                        else
                        {
                            fieldInfo.SetValue(resultObject,null);
                        }
                    }
                    else if (fieldInfo.GetCustomAttribute<SheetContentAdvancedMultipleAttribute>() is SheetContentAdvancedMultipleAttribute inputExcelSheetContentAdvancedMultipleAttribute)
                    {
                        var contentAdvancedMultipleReader = new AdvancedMultipleReader(fieldInfo.FieldType,_dataTable,inputExcelSheetContentAdvancedMultipleAttribute,_position.Clone(),_stringBuilder);
                        fieldInfo.SetValue(resultObject,contentAdvancedMultipleReader.resultObject);
                    }
                }
            }
        }
    }
}