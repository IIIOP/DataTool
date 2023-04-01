using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DataTool.CodeGenerate.Auxiliary;
using DataTool.FormatAccess.ExcelAccess.ExcelRead.ContentRead.Auxiliary;
using DataToolInterface.Data;
using DataToolInterface.Format.File.Excel;

namespace DataTool.FormatAccess.ExcelAccess.ExcelRead.ContentRead.BasicMultipleRead
{
    public class BasicMultipleReader
    {
        public bool isValid { get; set; }
        public string currentValue { get; set; }

        private Type _resultType;
        private DataTable _dataTable;
        private readonly Position _position;
        private readonly StringBuilder _stringBuilder;
        private readonly string _lastValue;
        private readonly SheetContentBasicMultipleAttribute _sheetContentBasicMultipleAttribute;
        
        private UInt16 _row;
        private UInt16 _column;
        private string _type;
        private BasicDataFormatEnum _format;
        private ConfigModeValueEnum _configModeValueEnum;
        private Regex _patternRegex;
        private Regex _filterRegex;
        private string _spliter;
        private (string min, string max) _numRange;
        private List<string> _stringRange;
        private bool _allowNull;
        private List<string> _nullValue;
        private string _describe;
        
        public object ResultObject;
        
        public BasicMultipleReader(Type paramType,DataTable paramDataTable,SheetContentBasicMultipleAttribute paramSheetContentBasicMultipleAttribute,Position paramPosition,StringBuilder paramStringBuilder,string paramLastValue = null)
        {
            _resultType = paramType;
            _dataTable = paramDataTable;
            _position = paramPosition;
            _stringBuilder = paramStringBuilder;
            _lastValue = paramLastValue;
            _sheetContentBasicMultipleAttribute = paramSheetContentBasicMultipleAttribute;

            ResultObject = Activator.CreateInstance(_resultType);
                
            ParseConfig();
            
            DoWork();
        }

        private void ParseConfig()
        {
            _row = UInt16.Parse(_sheetContentBasicMultipleAttribute.row);
            _column = UInt16.Parse(_sheetContentBasicMultipleAttribute.column);
            _type = _sheetContentBasicMultipleAttribute.type;
            if (!string.IsNullOrEmpty(_sheetContentBasicMultipleAttribute.format))
            {
                Enum.TryParse(_sheetContentBasicMultipleAttribute.format, out _format);
            }
            if (!string.IsNullOrEmpty(_sheetContentBasicMultipleAttribute.mode))
            {
                Enum.TryParse(_sheetContentBasicMultipleAttribute.mode, out _configModeValueEnum);
            }

            if (!string.IsNullOrEmpty(_sheetContentBasicMultipleAttribute.pattern))
            {
                _patternRegex = new Regex(_sheetContentBasicMultipleAttribute.pattern);
            }

            if (!string.IsNullOrEmpty(_sheetContentBasicMultipleAttribute.filter))
            {
                _filterRegex = new Regex(_sheetContentBasicMultipleAttribute.filter);
            }
            
            _spliter = _sheetContentBasicMultipleAttribute.spliter;

            if (!string.IsNullOrEmpty(_sheetContentBasicMultipleAttribute.range))
            {
                if (_sheetContentBasicMultipleAttribute.range.Contains(':'))
                {
                    var list = _sheetContentBasicMultipleAttribute.range.Split(':');
                    _numRange.min = list[0];
                    _numRange.max = list[1];
                }
                else if (_sheetContentBasicMultipleAttribute.range.Contains(','))
                {
                    _stringRange = _sheetContentBasicMultipleAttribute.range.Split(',').ToList();
                }
            }

            if (!string.IsNullOrEmpty(_sheetContentBasicMultipleAttribute.allowNull))
            {
                _allowNull = bool.Parse(_sheetContentBasicMultipleAttribute.allowNull);
            }

            if (!string.IsNullOrEmpty(_sheetContentBasicMultipleAttribute.nullValue))
            {
                _nullValue = _sheetContentBasicMultipleAttribute.nullValue.Split(',').ToList();
            }

            _describe = _sheetContentBasicMultipleAttribute.describe;
        }

        private void DoWork()
        {
            if (_dataTable.Rows.Count>=_row&&_dataTable.Columns.Count>=_column)
            {
                currentValue = _dataTable.Rows[_row-1][_column-1].ToString();
                _position.row += _row - 1;
                _position.column += _column - 1;
                
                if (string.IsNullOrEmpty(currentValue)&&_configModeValueEnum == ConfigModeValueEnum.EmptyFillPre)
                {
                    currentValue = _lastValue;
                }
            
                if (!string.IsNullOrEmpty(currentValue)|| _allowNull)
                {
                    bool isNull = false;
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
                    Console.WriteLine($"[Error] {_position.row}行{_position.column}列 内容为空");
                }
            }
        }
        
        
        private void SetValue()
        {
            IEnumerable<string> values = null;

            if (_spliter==null)
            {
                values = currentValue.Split(',');
            }
            else
            {
                values = currentValue.Split(_spliter[0]);
            }
            
            if (_patternRegex!=null)
            {
                foreach (var value in values)
                {
                    if (!_patternRegex.IsMatch(value))
                    {
                        Console.WriteLine($"[Error] {_position.row}行{_position.column}列 格式错误");
                    }
                }
            }

            if (_filterRegex!=null)
            {
                values = values.Select(item => _filterRegex.Match(item).Value);
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
            if (Enum.TryParse(_type.BaseType(),out BasicDataTypeEnum typeEnum))
            {
                foreach (var tempValue in values)
                {
                    object value = null;
                    switch (typeEnum)
                    {
                        case BasicDataTypeEnum.INT8:
                            if (sbyte.TryParse(tempValue,numberStyles,null,out var tempSbyte))
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
                            if (Int16.TryParse(tempValue,numberStyles,null,out var tempInt16))
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
                            if (Int32.TryParse(tempValue,numberStyles,null,out var tempInt32))
                            {
                                isParsePass = true;
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
                            if (Int64.TryParse(tempValue, numberStyles, null, out var tempInt64))
                            {
                                isParsePass = true;
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
                            if (byte.TryParse(tempValue, numberStyles, null, out var tempByte))
                            {
                                isParsePass = true;
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
                            if (UInt16.TryParse(tempValue, numberStyles, null, out var tempUInt16))
                            {
                                isParsePass = true;
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
                            if (UInt32.TryParse(tempValue, numberStyles, null, out var tempUInt32))
                            {
                                isParsePass = true;
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
                            if (UInt64.TryParse(tempValue, numberStyles, null, out var tempUInt64))
                            {
                                isParsePass = true;
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
                            if (float.TryParse(tempValue,out var tempFloat))
                            {
                                isParsePass = true;
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
                            if (double.TryParse(tempValue, out var tempDouble))
                            {
                                isParsePass = true;
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
                            if (Decimal.TryParse(tempValue, out var tempDecimal))
                            {
                                isParsePass = true;
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
                            value = tempValue;
                            if (_stringRange !=null)
                            {
                                if (!_stringRange.Contains(tempValue))
                                {
                                    isOutOfRange = true;
                                }
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (isOutOfRange)
                    {
                        Console.WriteLine("[Error] out of range!");
                    }

                    if (!isParsePass)
                    {
                        _stringBuilder.AppendLine($"[Error] {_position.row}行{_position.column}列 内容数据类型错误-[{tempValue}]");
                    }
                    
                    if (ResultObject is IList list)
                    {
                        list.Add(value);
                    }

                    isValid = true;
                }
            }
            else
            {
                Console.WriteLine("[ERROR] 数据类型不支持");
            }
        }
    }
}