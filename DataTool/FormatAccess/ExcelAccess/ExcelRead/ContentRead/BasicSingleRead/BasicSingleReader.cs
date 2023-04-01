using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DataTool.FormatAccess.ExcelAccess.ExcelRead.ContentRead.Auxiliary;
using DataToolInterface.Data;
using DataToolInterface.Format.File.Excel;

namespace DataTool.FormatAccess.ExcelAccess.ExcelRead.ContentRead.BasicSingleRead
{
    public class BasicSingleReader
    {
        public bool isValid { get; private set; }
        public string currentValue { get; private set; }
        
        private Type _resultType;
        private readonly DataTable _dataTable;
        private readonly Position _position;
        private readonly StringBuilder _stringBuilder;
        private readonly string _lastValue;
        private readonly SheetContentBasicSingleAttribute _sheetContentBasicSingleAttribute;

        private int _row;
        private int _column;
        private string _type;
        private BasicDataFormatEnum _format;
        private ConfigModeValueEnum _configModeValueEnum;
        private Regex _patternRegex;
        private Regex _filterRegex;
        private (string min, string max) _numRange;
        private List<string> _stringRange;
        private bool _allowNull;
        private List<string> _nullValue;
        private string _describe;

        public object ResultObject;

        public BasicSingleReader(Type paramType,DataTable paramDataTable,SheetContentBasicSingleAttribute paramSheetContentBasicSingleAttribute,Position paramPosition,StringBuilder paramStringBuilder,string paramLastValue = null)
        {
            _resultType = paramType;
            _dataTable = paramDataTable;
            _sheetContentBasicSingleAttribute = paramSheetContentBasicSingleAttribute;
            _position = paramPosition;
            _stringBuilder = paramStringBuilder;
            _lastValue = paramLastValue;

            ParseConfig();
            
            DoWork();
        }

        private void ParseConfig()
        {
            _row = int.Parse(_sheetContentBasicSingleAttribute.row);
            _column = int.Parse(_sheetContentBasicSingleAttribute.column);
            _type = _sheetContentBasicSingleAttribute.type;
            if (!string.IsNullOrEmpty(_sheetContentBasicSingleAttribute.format))
            {
                Enum.TryParse(_sheetContentBasicSingleAttribute.format, out _format);
            }
            if (!string.IsNullOrEmpty(_sheetContentBasicSingleAttribute.mode))
            {
                Enum.TryParse(_sheetContentBasicSingleAttribute.mode, out _configModeValueEnum);
            }

            if (!string.IsNullOrEmpty(_sheetContentBasicSingleAttribute.pattern))
            {
                _patternRegex = new Regex(_sheetContentBasicSingleAttribute.pattern);
            }

            if (!string.IsNullOrEmpty(_sheetContentBasicSingleAttribute.filter))
            {
                _filterRegex = new Regex(_sheetContentBasicSingleAttribute.filter);
            }

            if (!string.IsNullOrEmpty(_sheetContentBasicSingleAttribute.range))
            {
                if (_sheetContentBasicSingleAttribute.range.Contains(':'))
                {
                    var list = _sheetContentBasicSingleAttribute.range.Split(':');
                    _numRange.min = list[0];
                    _numRange.max = list[1];
                }
                else if (_sheetContentBasicSingleAttribute.range.Contains(','))
                {
                    _stringRange = _sheetContentBasicSingleAttribute.range.Split(',').ToList();
                }
            }

            if (!string.IsNullOrEmpty(_sheetContentBasicSingleAttribute.allowNull))
            {
                _allowNull = bool.Parse(_sheetContentBasicSingleAttribute.allowNull);
            }

            if (!string.IsNullOrEmpty(_sheetContentBasicSingleAttribute.nullValue))
            {
                _nullValue = _sheetContentBasicSingleAttribute.nullValue.Split(',').ToList();
            }

            _describe = _sheetContentBasicSingleAttribute.describe;
        }

        private void DoWork()
        {
            if (_dataTable.Rows.Count>=_row&&_dataTable.Columns.Count>=_column)
            {
                currentValue = _dataTable.Rows[_row - 1][_column - 1].ToString().Trim();
                _position.row += _row - 1;
                _position.column += _column - 1;
                if (string.IsNullOrEmpty(currentValue)&&_configModeValueEnum == ConfigModeValueEnum.EmptyFillPre)
                {
                    currentValue = _lastValue;
                }

                if (!string.IsNullOrEmpty(currentValue)|| _allowNull)
                {
                    var isNull = false;
                    if (!string.IsNullOrEmpty(currentValue))
                    {
                        if (_nullValue!=null&&_nullValue.Contains(currentValue))
                        {
                            isNull = true;
                        }
                    }
                    else
                    {
                        isNull = true;
                    }

                    if (!isNull)
                    {
                        SetValue();
                    }
                }
                else
                {
                    _stringBuilder.AppendLine($@"[Error] {_position.row}行{_position.column}列 <{_sheetContentBasicSingleAttribute.describe}>内容为空");
                }
            }
        }

        private void SetValue()
        {
            if (_patternRegex!=null)
            {
                if (!_patternRegex.IsMatch(currentValue))
                {
                    _stringBuilder.AppendLine($"[Error] {_position.row}行{_position.column}列 格式错误");
                }
            }

            if (_filterRegex!= null)
            {
                currentValue = _filterRegex.Match(currentValue).Value;
            }

            NumberStyles numberStyles;
            if (_format==BasicDataFormatEnum.x16)
            {
                numberStyles = NumberStyles.HexNumber;
            }
            else
            {
                numberStyles = NumberStyles.Any;
            }

            var isParsePass = false;
            var isOutOfRange = false;
            if (Enum.TryParse(_type,out BasicDataTypeEnum typeEnum))
            {
                object value = null;
                switch (typeEnum)
                {
                    case BasicDataTypeEnum.INT8:
                        if (sbyte.TryParse(currentValue,numberStyles,null,out var tempSbyte))
                        {
                            isParsePass = true;
                            if (_numRange.min!=null)
                            {
                                var min = sbyte.Parse(_numRange.min,numberStyles);
                                var max = sbyte.Parse(_numRange.max,numberStyles);
                                if (tempSbyte>max||tempSbyte<min)
                                {
                                    isOutOfRange = true;
                                }
                            }
                            value = tempSbyte;
                        }
                        break;
                    case BasicDataTypeEnum.INT16:
                        if (Int16.TryParse(currentValue,numberStyles,null,out var tempInt16))
                        {
                            isParsePass = true;
                            if (_numRange.min!=null)
                            {
                                var min = Int16.Parse(_numRange.min,numberStyles);
                                var max = Int16.Parse(_numRange.max,numberStyles);
                                if (tempInt16>max||tempInt16<min)
                                {
                                    isOutOfRange = true;
                                }
                            }
                            value = tempInt16;
                        }
                        break;
                    case BasicDataTypeEnum.INT32:
                        if (Int32.TryParse(currentValue,numberStyles,null,out var tempInt32))
                        {
                            isParsePass = true;
                            tempInt32 = Int32.Parse(currentValue);
                            if (_numRange.min!=null)
                            {
                                var min = Int32.Parse(_numRange.min,numberStyles);
                                var max = Int32.Parse(_numRange.max,numberStyles);
                                if (tempInt32>max||tempInt32<min)
                                {
                                    isOutOfRange = true;
                                }
                            }
                            value = tempInt32;
                        }
                        break;
                    case BasicDataTypeEnum.INT64:
                        if (Int64.TryParse(currentValue,numberStyles,null,out var tempInt64))
                        {
                            isParsePass = true;
                            tempInt64 = Int64.Parse(currentValue);
                            if (_numRange.min!=null)
                            {
                                var min = Int64.Parse(_numRange.min,numberStyles);
                                var max = Int64.Parse(_numRange.max,numberStyles);
                                if (tempInt64>max||tempInt64<min)
                                {
                                    isOutOfRange = true;
                                }
                            }
                            value = tempInt64;
                        }
                        break;
                    case BasicDataTypeEnum.UINT8:
                        if (byte.TryParse(currentValue,numberStyles,null,out var tempByte))
                        {
                            isParsePass = true;
                            tempByte = byte.Parse(currentValue);
                            if (_numRange.min!=null)
                            {
                                var min = byte.Parse(_numRange.min,numberStyles);
                                var max = byte.Parse(_numRange.max,numberStyles);
                                if (tempByte>max||tempByte<min)
                                {
                                    isOutOfRange = true;
                                }
                            }
                            value = tempByte;
                        }
                        break;
                    case BasicDataTypeEnum.UINT16:
                        if (UInt16.TryParse(currentValue,numberStyles,null,out var tempUInt16))
                        {
                            isParsePass = true;
                            tempUInt16 = UInt16.Parse(currentValue);
                            if (_numRange.min!=null)
                            {
                                var min = UInt16.Parse(_numRange.min,numberStyles);
                                var max = UInt16.Parse(_numRange.max,numberStyles);
                                if (tempUInt16>max||tempUInt16<min)
                                {
                                    isOutOfRange = true;
                                }
                            }
                            value = tempUInt16;
                        }
                        break;
                    case BasicDataTypeEnum.UINT32:
                        if (UInt32.TryParse(currentValue,numberStyles,null,out var tempUInt32))
                        {
                            isParsePass = true;
                            tempUInt32 = UInt32.Parse(currentValue);
                            if (_numRange.min!=null)
                            {
                                var min = UInt32.Parse(_numRange.min,numberStyles);
                                var max = UInt32.Parse(_numRange.max,numberStyles);
                                if (tempUInt32>max||tempUInt32<min)
                                {
                                    isOutOfRange = true;
                                }
                            }
                            value = tempUInt32;
                        }
                        break;
                    case BasicDataTypeEnum.UINT64:
                        if (UInt64.TryParse(currentValue,numberStyles,null,out var tempUInt64))
                        {
                            isParsePass = true;
                            tempUInt64 = UInt64.Parse(currentValue);
                            if (_numRange.min!=null)
                            {
                                var min = UInt64.Parse(_numRange.min,numberStyles);
                                var max = UInt64.Parse(_numRange.max,numberStyles);
                                if (tempUInt64>max||tempUInt64<min)
                                {
                                    isOutOfRange = true;
                                }
                            }
                            value = tempUInt64;
                        }
                        break;
                    case BasicDataTypeEnum.FLOAT:
                        if (float.TryParse(currentValue,out var tempFloat))
                        {
                            isParsePass = true;
                            tempFloat = float.Parse(currentValue);
                            if (_numRange.min!=null)
                            {
                                var min = float.Parse(_numRange.min);
                                var max = float.Parse(_numRange.max);
                                if (tempFloat>max||tempFloat<min)
                                {
                                    isOutOfRange = true;
                                }
                            }
                            value = tempFloat;
                        }
                        break;
                    case BasicDataTypeEnum.DOUBLE:
                        if (double.TryParse(currentValue,out var tempDouble))
                        {
                            isParsePass = true;
                            tempDouble = double.Parse(currentValue);
                            if (_numRange.min!=null)
                            {
                                var min = double.Parse(_numRange.min);
                                var max = double.Parse(_numRange.max);
                                if (tempDouble>max||tempDouble<min)
                                {
                                    isOutOfRange = true;
                                }
                            }
                            value = tempDouble;
                        }
                        break;
                    case BasicDataTypeEnum.DECIMAL:
                        if (Decimal.TryParse(currentValue,out var tempDecimal))
                        {
                            isParsePass = true;
                            tempDecimal = Decimal.Parse(currentValue);
                            if (_numRange.min!=null)
                            {
                                var min = Decimal.Parse(_numRange.min);
                                var max = Decimal.Parse(_numRange.max);
                                if (tempDecimal>max||tempDecimal<min)
                                {
                                    isOutOfRange = true;
                                }
                            }
                            value = tempDecimal;
                        }
                        break;
                    case BasicDataTypeEnum.String:
                        isParsePass = true;
                        value = currentValue;
                        if (_stringRange !=null)
                        {
                            if (!_stringRange.Contains(currentValue))
                            {
                                isOutOfRange = true;
                            }
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                if (!isParsePass)
                {
                    _stringBuilder.AppendLine($"[Error] {_position.row}行{_position.column}列 内容数据类型错误-[{currentValue}]");
                }
                if (isOutOfRange)
                {
                    _stringBuilder.AppendLine($"[Error] {_position.row}行{_position.column}列 {_sheetContentBasicSingleAttribute.describe} out of range!");
                }
                
                isValid = true;
                ResultObject = value;
                
            }
            else
            {
                _stringBuilder.AppendLine($"[Error] {_position.row}行{_position.column}列 类型不支持");
            }
        }
    }
}