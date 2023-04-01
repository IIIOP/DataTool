using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DataTool.FormatAccess.ExcelAccess.ExcelWrite;
using DataToolInterface.Format.Script;
using DataToolInterface.Methods;
using DataToolLog;
using DefaultConfig;

namespace DataTool.CaseTest
{
    public class CaseTester
    {
        private readonly object _object;
        private readonly List<TestCaseAttribute> _resultList;
        public CaseTester(object paramObject)
        {
            if (paramObject == null) throw new ArgumentNullException(nameof(paramObject));
            
            _object = paramObject;
            _resultList = new List<TestCaseAttribute>();
            
            TestCase();
            
        }

        
        private void TestCase()
        {
            LogHelper.DefaultLog.WriteLine($@"Case Test start.............");

            var testCases = _object.GetType().GetMethods()
                .Where(item => item.GetCustomAttribute<TestCaseAttribute>() != null)
                .OrderBy(p=>p.GetCustomAttribute<TestCaseAttribute>().id);
            foreach (var testCase in testCases)
            {
                if (testCase.GetCustomAttribute<TestCaseAttribute>() is TestCaseAttribute testCaseAttribute)
                {
                    LogHelper.DefaultLog.WriteLine($"############################## Test Case ##################################\r\n" +
                                                   $"#### ID:\t{testCaseAttribute.id}\r\n"+
                                                   $"#### Author:\t{testCaseAttribute.author}\r\n" +
                                                   $"#### Describe:\t{testCaseAttribute.describe}\r\n"+
                                                   $"###########################################################################");
                    testCaseAttribute.Result = (bool)testCase.Invoke(_object, null);
                    _resultList.Add(testCaseAttribute);
                    LogHelper.DefaultLog.WriteLine($@"************************************************** Final Result:\t{(testCaseAttribute.Result?"Pass":"Fail")}");
                }
            }
            
            LogHelper.DefaultLog.WriteLine($"**************************** CaseTest Summarize ***********************");
            LogHelper.DefaultLog.WriteLine($"total[{_resultList.Count}] pass[{_resultList.Count(p=>p.Result)}] fail[{_resultList.Count(p=>!p.Result)}]");
            foreach (var result in _resultList)
            {
                LogHelper.DefaultLog.WriteLine($"ID[{result.id}]=>[{(result.Result?"Pass":"Fail")}]");
            }
        }

        public void ExportResultToExcel(DirectoryInfo paramDirectoryInfo)
        {
            var excelTestCase = new Excel_TestCase();
            foreach (var result in _resultList)
            {
                var testCase = excelTestCase.Sheet_TestCase.TestCases.AddNewInstance();
                testCase.Designer = "ChenJilian";
                testCase.Index = "T";
                testCase.ID = result.id;
                testCase.Title = "我的用例我做主";
                testCase.PreCnd = "aaa";
                testCase.PostCnd = "bbb";
                testCase.ExpectedResult = result.Result?"Pass":"Fail";
                testCase.INParam = "ss";
                testCase.OUTParam = "dsdsd";
                testCase.Tracability = "dsdsd";
                testCase.ValidationType = "dsdsd";
                testCase.TestMethod = "dsdsd";
                testCase.TestType = "dsdsdsdsd";
            }

            var excelWriter = new ExcelWriter(excelTestCase, new FileInfo(Path.Combine(paramDirectoryInfo.FullName,"TestResult.xlsx")));
        }
    }
}