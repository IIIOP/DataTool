using System;
using System.Linq;
using System.Reflection;
using DataToolInterface.Format.Script;
using DataToolLog;

namespace DataTool.OutputGenerate
{
    public class OutputGenerator
    {
        private readonly object _object;
        public OutputGenerator(object paramObject)
        {
            if (paramObject == null) throw new ArgumentNullException(nameof(paramObject));
            
            _object = paramObject;
            
            GenerateOutput();
        }

        private void GenerateOutput()
        {
            LogHelper.DefaultLog.WriteLine($@"Output Generate start.............");

            var generateOutput = _object.GetType().GetMethods()
                .Single(item => item.GetCustomAttribute<GenerateOutputAttribute>() != null);

            generateOutput.Invoke(_object, null);
        }
    }
}