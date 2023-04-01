using System;
using System.ComponentModel;
using System.IO;
using DataTool.FormatAccess.BinaryAccess.Auxiliary;
using DataToolInterface.Data;
using DataToolInterface.Format.File.Binary;
using DataToolLog;

namespace DataTool.FormatAccess.Auxiliary
{
    public static class ExtensionMethods
    {
        public static object ReadBasicDataTypeByString(this BasicDataTypeEnum paramBasicDataTypeEnum,string paramString)
        {
            if (string.IsNullOrWhiteSpace(paramString))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(paramString));

            object result;

            try
            {
                switch (paramBasicDataTypeEnum)
                {
                    case BasicDataTypeEnum.INT8:
                        result = sbyte.Parse(paramString);
                        break;
                    case BasicDataTypeEnum.INT16:
                        result = Int16.Parse(paramString);
                        break;
                    case BasicDataTypeEnum.INT32:
                        result = Int32.Parse(paramString);
                        break;
                    case BasicDataTypeEnum.INT64:
                        result = Int64.Parse(paramString);
                        break;
                    case BasicDataTypeEnum.UINT8:
                        result = byte.Parse(paramString);
                        break;
                    case BasicDataTypeEnum.UINT16:
                        result = UInt16.Parse(paramString);
                        break;
                    case BasicDataTypeEnum.UINT32:
                        result = UInt32.Parse(paramString);
                        break;
                    case BasicDataTypeEnum.UINT64:
                        result = UInt64.Parse(paramString);
                        break;
                    case BasicDataTypeEnum.FLOAT:
                        result = Single.Parse(paramString);
                        break;
                    case BasicDataTypeEnum.DOUBLE:
                        result = Double.Parse(paramString);
                        break;
                    case BasicDataTypeEnum.DECIMAL:
                        result = Decimal.Parse(paramString);
                        break;
                    case BasicDataTypeEnum.String:
                        result = paramString;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(paramBasicDataTypeEnum), paramBasicDataTypeEnum, null);
                }
            }
            catch (Exception)
            {
                LogHelper.DefaultLog.WriteLine($@"Parse [{paramString}] to [{paramBasicDataTypeEnum}] fail!!!");
                throw;
            }

            return result;
        }
        
        public static object ReadBasicDataTypeByBinary(this BasicDataTypeEnum paramBasicDataTypeEnum,BinaryReader paramBinaryReader,EndianEnum paramEndianEnum)
        {
            if (paramBinaryReader == null) throw new ArgumentNullException(nameof(paramBinaryReader));
            
            object result;
            switch (paramBasicDataTypeEnum)
            {
                case BasicDataTypeEnum.INT8:
                    result = paramBinaryReader.ReadSByte();
                    break;
                case BasicDataTypeEnum.INT16:
                    result = paramEndianEnum==EndianEnum.Big ? paramBinaryReader.ReadInt16().Reverse() : paramBinaryReader.ReadInt16();
                    break;
                case BasicDataTypeEnum.INT32:
                    result = paramEndianEnum==EndianEnum.Big ? paramBinaryReader.ReadInt32().Reverse() : paramBinaryReader.ReadInt32();
                    break;
                case BasicDataTypeEnum.UINT8:
                    result = paramBinaryReader.ReadByte();
                    break;
                case BasicDataTypeEnum.UINT16:
                    result = paramEndianEnum==EndianEnum.Big ? paramBinaryReader.ReadUInt16().Reverse() : paramBinaryReader.ReadUInt16();
                    break;
                case BasicDataTypeEnum.UINT32:
                    result = paramEndianEnum==EndianEnum.Big ? paramBinaryReader.ReadUInt32().Reverse() : paramBinaryReader.ReadUInt32();
                    break;
                case BasicDataTypeEnum.UINT64:
                    result = paramEndianEnum==EndianEnum.Big ? paramBinaryReader.ReadUInt64().Reverse() : paramBinaryReader.ReadUInt64();
                    break;
                case BasicDataTypeEnum.INT64:
                case BasicDataTypeEnum.FLOAT:
                case BasicDataTypeEnum.DOUBLE:
                case BasicDataTypeEnum.DECIMAL:
                case BasicDataTypeEnum.String:
                default:
                    throw new ArgumentOutOfRangeException(nameof(paramBasicDataTypeEnum), paramBasicDataTypeEnum, null);
            }
            return result;
        }
        
        public static void WriteBasicDataTypeByBinary(this BasicDataTypeEnum paramBasicDataTypeEnum,object paramObject,BinaryWriter paramBinaryWriter,EndianEnum paramEndianEnum)
        {
            if (paramObject == null) throw new ArgumentNullException(nameof(paramObject));
            if (paramBinaryWriter == null) throw new ArgumentNullException(nameof(paramBinaryWriter));
            
            switch (paramBasicDataTypeEnum)
            {
                case BasicDataTypeEnum.INT8:
                    paramBinaryWriter.Write((byte) paramObject);
                    break;
                case BasicDataTypeEnum.INT16:
                    switch (paramEndianEnum)
                    {
                        case EndianEnum.Big:
                            paramBinaryWriter.Write(((Int16) paramObject).Reverse());
                            break;
                        case EndianEnum.Little:
                            paramBinaryWriter.Write((Int16) paramObject);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case BasicDataTypeEnum.INT32:
                    switch (paramEndianEnum)
                    {
                        case EndianEnum.Big:
                            paramBinaryWriter.Write(((Int32) paramObject).Reverse());
                            break;
                        case EndianEnum.Little:
                            paramBinaryWriter.Write((Int32) paramObject);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case BasicDataTypeEnum.UINT8:
                    paramBinaryWriter.Write((byte) paramObject);
                    break;
                case BasicDataTypeEnum.UINT16:
                    switch (paramEndianEnum)
                    {
                        case EndianEnum.Big:
                            paramBinaryWriter.Write(((UInt16) paramObject).Reverse());
                            break;
                        case EndianEnum.Little:
                            paramBinaryWriter.Write((UInt16) paramObject);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case BasicDataTypeEnum.UINT32:
                    switch (paramEndianEnum)
                    {
                        case EndianEnum.Big:
                            paramBinaryWriter.Write(((UInt32) paramObject).Reverse());
                            break;
                        case EndianEnum.Little:
                            paramBinaryWriter.Write((UInt32) paramObject);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case BasicDataTypeEnum.UINT64:
                    switch (paramEndianEnum)
                    {
                        case EndianEnum.Big:
                            paramBinaryWriter.Write(((UInt64) paramObject).Reverse());
                            break;
                        case EndianEnum.Little:
                            paramBinaryWriter.Write((UInt64) paramObject);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case BasicDataTypeEnum.INT64:
                case BasicDataTypeEnum.FLOAT:
                case BasicDataTypeEnum.DOUBLE:
                case BasicDataTypeEnum.DECIMAL:
                case BasicDataTypeEnum.String:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}