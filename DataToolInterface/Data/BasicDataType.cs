using System;
using System.Collections.Generic;

namespace DataToolInterface.Data
{
    public enum BasicDataTypeEnum
    {
        INT8,
        INT16,
        INT32,
        INT64,
        UINT8,
        UINT16,
        UINT32,
        UINT64,
        FLOAT,
        DOUBLE,
        DECIMAL,
        String,
    }
    public static class BasicDataType
    {
        public static readonly Dictionary<BasicDataTypeEnum, string> BasicDataTypeNameConvertDictionary = new Dictionary<BasicDataTypeEnum, string>
        {
            {BasicDataTypeEnum.INT8, nameof(SByte)},
            {BasicDataTypeEnum.INT16, nameof(Int16)},
            {BasicDataTypeEnum.INT32, nameof(Int32)},
            {BasicDataTypeEnum.INT64, nameof(Int64)},
            {BasicDataTypeEnum.UINT8, nameof(Byte)},
            {BasicDataTypeEnum.UINT16, nameof(UInt16)},
            {BasicDataTypeEnum.UINT32, nameof(UInt32)},
            {BasicDataTypeEnum.UINT64, nameof(UInt64)},
            {BasicDataTypeEnum.FLOAT, nameof(Single)},
            {BasicDataTypeEnum.DOUBLE, nameof(Double)},
            {BasicDataTypeEnum.DECIMAL, nameof(Decimal)},
            {BasicDataTypeEnum.String, nameof(String)},
        };
    }
}