using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DataToolInterface.Format.Script;
using DataToolLog;

namespace DataTool.ConsistencyCheck
{
    public class ConsistencyChecker
    {
        private readonly object _object;
        private readonly Dictionary<bool, string> _dictionary;
        
        public ConsistencyChecker(object paramObject)
        {
            if (paramObject == null) throw new ArgumentNullException(nameof(paramObject));
            
            _object = paramObject;
            _dictionary = new Dictionary<bool, string>()
            {
                {true,"Pass"},
                {false,"Fail"},
            };
            
            CheckConsistency();
        }

        
        private void CheckConsistency()
        {
            LogHelper.DefaultLog.WriteLine($@"Consistency Check start.............");

            var resultDictionary = new Dictionary<string, bool>();

            var checkConsistencies = _object.GetType().GetMethods()
                .Where(item => item.GetCustomAttribute<CheckConsistencyAttribute>() != null)
                .OrderBy(p=>p.GetCustomAttribute<CheckConsistencyAttribute>().id);
            foreach (var checkConsistency in checkConsistencies)
            {
                if (checkConsistency.GetCustomAttribute<CheckConsistencyAttribute>() is CheckConsistencyAttribute checkConsistencyAttribute)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    LogHelper.DefaultLog.WriteLine($"############################## Check Consistency ####################################\r\n" +
                                                   $"#### ID:\t{checkConsistencyAttribute.id}\r\n"+
                                                   $"#### Author:\t{checkConsistencyAttribute.author}\r\n" +
                                                   $"#### Describe:\t{checkConsistencyAttribute.describe}\r\n"+
                                                   $"###########################################################################");
                    Console.ForegroundColor = ConsoleColor.White;
                    var result = (bool)checkConsistency.Invoke(_object, null);
                    resultDictionary.Add(checkConsistencyAttribute.id,result);
                    LogHelper.DefaultLog.WriteLine($"************************************************** Final Result:\t{_dictionary[result]}");
                }
            }
            
            LogHelper.DefaultLog.WriteLine($"**************************** ConsistencyCheck Summarize ***********************");
            LogHelper.DefaultLog.WriteLine($"total[{resultDictionary.Count}] pass[{resultDictionary.Values.Count(p=>p)}] fail[{resultDictionary.Values.Count(p=>!p)}]");
            foreach (var result in resultDictionary)
            {
                LogHelper.DefaultLog.WriteLine($"ID[{result.Key}]=>[{_dictionary[result.Value]}]");
            }
        }
    }
}