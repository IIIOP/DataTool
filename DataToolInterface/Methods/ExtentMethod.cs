using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataToolInterface.Data;
using DataToolLog;

namespace DataToolInterface.Methods
{
    public static class ExtentMethod
    {
        public const int offset1 = -50;
        public const int offset2 = -50;
        /// <summary>
        /// 仅适用于XML描述的结构体哦
        /// </summary>
        /// <param name="paramObject"></param>
        /// <returns></returns>
        public static object DeepClone(this object paramObject)
        {
            var type = paramObject.GetType();
            object result = null;
            if (paramObject is IList origin)
            {
                result = Activator.CreateInstance(type);
                var list = result as IList;
                foreach (var item in origin)
                {
                    list?.Add(item.DeepClone());
                }
            }
            else if (BasicDataType.BasicDataTypeNameConvertDictionary.ContainsValue(type.Name))
            {
                result = paramObject;
            }
            else
            {
                result = Activator.CreateInstance(type);
                foreach (var fieldInfo in paramObject.GetType().GetFields())
                {
                    if (fieldInfo.GetValue(paramObject) is object subObject)
                    {
                        fieldInfo.SetValue(result,subObject.DeepClone());
                    }
                    else
                    {
                        fieldInfo.SetValue(result, null);
                    }
                }
            }

            return result;
        }

        public static bool ContentEqual(this object paramSelf,object paramOppo,string paramPrefix="")
        {
            var result = true;

            if (paramSelf == paramOppo)
            {
                result = true;
                if (string.IsNullOrWhiteSpace(paramPrefix)&&paramSelf!=null)
                {
                    paramPrefix = paramSelf.GetType().Name;
                }
                LogHelper.DefaultLog.WriteLine($"[{result}]=>{paramPrefix.Split('.').Last(),offset1}={$"[the same Pointer:{paramSelf}]",offset2} {paramPrefix}");
            }
            else if (paramSelf==null||paramOppo==null)
            {
                result = false;
                if (string.IsNullOrWhiteSpace(paramPrefix))
                {
                    paramPrefix = (paramSelf??paramOppo).GetType().Name;
                }
                if ((paramSelf??paramOppo).GetType().Name==BasicDataTypeEnum.String.ToString())
                {
                    if (string.IsNullOrEmpty(paramSelf as string)&&string.IsNullOrEmpty(paramOppo as string))
                    {
                        result = true;
                    }
                    LogHelper.DefaultLog.WriteLine($"[{result}]=>{paramPrefix.Split('.').Last(),offset1}={$"[{paramSelf}]:[{paramOppo}]",offset2} {paramPrefix}");
                }
                else
                {
                    if ((paramSelf??paramOppo) is IList list)
                    {
                        LogHelper.DefaultLog.WriteLine($"[{result}]=>{paramPrefix.Split('.').Last(),offset1}={$"[{(paramSelf == null ? "null" : "non-null")}]:[{(paramOppo == null ? "null" : "non-null")}]",offset2} {paramPrefix}");
                        for (int i = 0; i < list.Count; i++)
                        {
                            result &= (paramSelf==null?null:list[i]).ContentEqual((paramOppo==null?null:list[i]),paramPrefix+$@"[{i}]");
                        }
                    }
                    else if (BasicDataType.BasicDataTypeNameConvertDictionary.ContainsValue((paramSelf??paramOppo).GetType().Name))
                    {
                        LogHelper.DefaultLog.WriteLine($"[{result}]=>{paramPrefix.Split('.').Last(),offset1}={$"[{paramSelf ?? "null"}]:[{paramOppo ?? "null"}]",offset2} {paramPrefix}");
                    }
                    else
                    {
                        LogHelper.DefaultLog.WriteLine($"[{result}]=>{paramPrefix.Split('.').Last(),offset1}={$"[{(paramSelf == null ? "null" : "non-null")}]:[{(paramOppo == null ? "null" : "non-null")}]",offset2} {paramPrefix}");
                        foreach (var fieldInfo in (paramSelf??paramOppo).GetType().GetFields())
                        {
                            var fieldValue = fieldInfo.GetValue(paramSelf??paramOppo);
                            result &= (paramSelf==null?null:fieldValue).ContentEqual((paramOppo==null?null:fieldValue),$@"{paramPrefix}.{fieldInfo.Name}");
                        }
                    }
                }
            }
            else if (BasicDataType.BasicDataTypeNameConvertDictionary.ContainsValue(paramSelf.GetType().Name))
            {
                result = paramSelf.ToString() == paramOppo.ToString();
                LogHelper.DefaultLog.WriteLine($"[{result}]=>{paramPrefix.Split('.').Last(),offset1}={$"[{paramSelf}]:[{paramOppo}]",offset2} {paramPrefix}");
            }
            else if (paramSelf.GetType() == paramOppo.GetType())
            {
                if (string.IsNullOrWhiteSpace(paramPrefix))
                {
                    paramPrefix = paramSelf.GetType().Name;
                }
                if (paramSelf is IList listSelf && paramOppo is IList listOppo)
                {
                    if (listSelf.Count==listOppo.Count)
                    {
                        for (var i = 0; i < listSelf.Count; i++)
                        {
                            result &= listSelf[i].ContentEqual(listOppo[i],paramPrefix+$@"[{i}]");
                            // if (!result)
                            // {
                            //     break;
                            // }
                        }
                    }
                    else
                    {
                        result = false;
                        LogHelper.DefaultLog.WriteLine($@"[{result}]=>{paramPrefix.Split('.').Last(),offset1}={$"[count:{listSelf.Count}]:[count:{listOppo.Count}]",offset2} {paramPrefix}");
                        for (var i = 0; i < Math.Max(listSelf.Count,listOppo.Count); i++)
                        {
                            result &= (i<listSelf.Count?listSelf[i]:null).ContentEqual((i<listOppo.Count?listOppo[i]:null),paramPrefix+$@"[{i}]");
                            // if (!result)
                            // {
                            //     break;
                            // }
                        }
                    }
                }
                else
                {
                    var type = paramSelf.GetType();
                    if (BasicDataType.BasicDataTypeNameConvertDictionary.ContainsValue(type.Name))
                    {
                        var key = BasicDataType.BasicDataTypeNameConvertDictionary
                            .Single(i => i.Value == type.Name).Key;
                        switch (key)
                        {
                            case BasicDataTypeEnum.INT8:
                                result &= (sbyte) paramSelf == (sbyte) paramOppo;
                                break;
                            case BasicDataTypeEnum.INT16:
                                result &= (Int16) paramSelf == (Int16) paramOppo;
                                break;
                            case BasicDataTypeEnum.INT32:
                                result &= (Int32) paramSelf == (Int32) paramOppo;
                                break;
                            case BasicDataTypeEnum.INT64:
                                result &= (Int64) paramSelf == (Int64) paramOppo;
                                break;
                            case BasicDataTypeEnum.UINT8:
                                result &= (byte) paramSelf == (byte) paramOppo;
                                break;
                            case BasicDataTypeEnum.UINT16:
                                result &= (UInt16) paramSelf == (UInt16) paramOppo;
                                break;
                            case BasicDataTypeEnum.UINT32:
                                result &= (UInt32) paramSelf == (UInt32) paramOppo;
                                break;
                            case BasicDataTypeEnum.UINT64:
                                result &= (UInt64) paramSelf == (UInt64) paramOppo;
                                break;
                            case BasicDataTypeEnum.FLOAT:
                                result &= paramSelf.ToString()==paramOppo.ToString();
                                break;
                            case BasicDataTypeEnum.DOUBLE:
                                result &= paramSelf.ToString()==paramOppo.ToString();
                                break;
                            case BasicDataTypeEnum.DECIMAL:
                                result &= (decimal) paramSelf == (decimal) paramOppo;
                                break;
                            case BasicDataTypeEnum.String:
                                result &= (string) paramSelf == (string) paramOppo;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        LogHelper.DefaultLog.WriteLine($@"[{result}]=>{paramPrefix.Split('.').Last(),offset1}={$"[{paramSelf}]:[{paramOppo}]",offset2} {paramPrefix}");
                    }
                    else
                    {
                        foreach (var fieldInfo in type.GetFields())
                        {
                            var objSelf = fieldInfo.GetValue(paramSelf);
                            var objOppo = fieldInfo.GetValue(paramOppo);
                            result &= objSelf.ContentEqual(objOppo,$@"{paramPrefix}.{fieldInfo.Name}");
                            // if (!result)
                            // {
                            //     break;
                            // }
                        }
                    }
                }
            }
            else
            {
                result = false;
                LogHelper.DefaultLog.WriteLine($@"[{result}]=>{paramPrefix.Split('.').Last(),offset1}={$"[tye different!!!]",offset2} {paramPrefix}");
            }

            return result;
        }

        public static List<byte> ToBytes(this object paramObject,bool isBigEndian = false)
        {
            var bytes = new List<byte>();

            if (paramObject is IList list)
            {
                foreach (var t in list)
                {
                    bytes.AddRange(t.ToBytes(isBigEndian));
                }
            }
            else
            {
                var type = paramObject.GetType();
                if (BasicDataType.BasicDataTypeNameConvertDictionary.ContainsValue(type.Name))
                {
                    var key = BasicDataType.BasicDataTypeNameConvertDictionary
                        .Single(i => i.Value == type.Name).Key;
                    switch (key)
                    {
                        case BasicDataTypeEnum.INT8:
                            bytes.Add(Convert.ToByte((sbyte) paramObject));
                            break;
                        case BasicDataTypeEnum.INT16:
                            bytes.AddRange(BitConverter.GetBytes((Int16) paramObject).ToList());
                            break;
                        case BasicDataTypeEnum.INT32:
                            bytes.AddRange(BitConverter.GetBytes((Int32) paramObject).ToList());
                            break;
                        case BasicDataTypeEnum.INT64:
                            bytes.AddRange(BitConverter.GetBytes((Int64) paramObject).ToList());
                            break;
                        case BasicDataTypeEnum.UINT8:
                            bytes.Add(Convert.ToByte((byte) paramObject));
                            break;
                        case BasicDataTypeEnum.UINT16:
                            bytes.AddRange(BitConverter.GetBytes((UInt16) paramObject).ToList());
                            break;
                        case BasicDataTypeEnum.UINT32:
                            bytes.AddRange(BitConverter.GetBytes((UInt32) paramObject).ToList());
                            break;
                        case BasicDataTypeEnum.UINT64:
                            bytes.AddRange(BitConverter.GetBytes((UInt64) paramObject).ToList());
                            break;
                        case BasicDataTypeEnum.FLOAT:
                        case BasicDataTypeEnum.DOUBLE:
                        case BasicDataTypeEnum.DECIMAL:
                        case BasicDataTypeEnum.String:
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (isBigEndian)
                    {
                        bytes.Reverse();
                    }
                }
                else
                {
                    foreach (var fieldInfo in type.GetFields())
                    {
                        var subObject = fieldInfo.GetValue(paramObject);
                        bytes.AddRange(subObject.ToBytes(isBigEndian));
                    }
                }
            }
            return bytes;
        }
        public static T AddNewInstance<T>(this List<T> paramList) where T : new()
        {
            var instance = new T();
            paramList.Add(instance);
            return instance;
        }
        public static T1 AddNewInstance<T,T1>(this List<T> paramList) where T1 : T, new()
        {
            var instance = new T1();
            paramList.Add(instance);
            return instance;
        }
        public static T NewInstance<T>(this T _) where T : new()
        {
            var instance = new T();
            return instance;
        }
    }
}