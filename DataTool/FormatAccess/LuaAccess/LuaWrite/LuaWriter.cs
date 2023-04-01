using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.IO;
using System.Text;
using DataTool.FormatAccess.Auxiliary;
using DataToolInterface.Format.File;
using DataToolInterface.Format.File.Lua;

namespace DataTool.FormatAccess.LuaAccess.LuaWrite
{
    [FormatAccess(format = FileFormatEnum.Lua)]
    public class LuaWriter:FormatWriter
    {
        public LuaWriter(object paramObject,FileInfo paramFileInfo):base(paramObject,paramFileInfo)
        {
            WriteLua();
        }

        private void WriteLua()
        {
            var textWriter = new StreamWriter(FileInfo.FullName, false, Encoding.UTF8);
            var indentedTextWriter = new IndentedTextWriter(textWriter);

            foreach (var chunk in (IList) Object.GetType().GetField(LuaFieldNameEnum.body.ToString()).GetValue(Object))
            {
                WriteLuaChunk(chunk,indentedTextWriter);
            }
            
            indentedTextWriter.Flush();
        }

        private void WriteLuaChunk(object paramObject, IndentedTextWriter paramIndentedTextWriter)
        {
            if (paramObject == null) throw new ArgumentNullException(nameof(paramObject));
            if (paramIndentedTextWriter == null) throw new ArgumentNullException(nameof(paramIndentedTextWriter));
            
            var typeName = paramObject.GetType().Name;
            switch (typeName)
            {
                case nameof(LuaTypeNameEnum.LuaStatement):
                    paramIndentedTextWriter.WriteLine(paramObject.GetType().GetField(LuaFieldNameEnum.body.ToString()).GetValue(paramObject));
                    break;
                case nameof(LuaTypeNameEnum.LuaBlock):
                    paramIndentedTextWriter.WriteLine(paramObject.GetType().GetField(LuaFieldNameEnum.head.ToString()).GetValue(paramObject));
                    paramIndentedTextWriter.Indent++;
                    foreach (var chunk in (IList)paramObject.GetType().GetField(LuaFieldNameEnum.body.ToString()).GetValue(paramObject))
                    {
                        WriteLuaChunk(chunk, paramIndentedTextWriter);
                    }
                    paramIndentedTextWriter.Indent--;
                    paramIndentedTextWriter.WriteLine(paramObject.GetType().GetField(LuaFieldNameEnum.tail.ToString()).GetValue(paramObject));
                    break;
                case nameof(LuaTypeNameEnum.LuaIntentBlock):
                    paramIndentedTextWriter.Indent++;
                    foreach (var chunk in (IList)paramObject.GetType().GetField(LuaFieldNameEnum.body.ToString()).GetValue(paramObject))
                    {
                        WriteLuaChunk(chunk, paramIndentedTextWriter);
                    }
                    paramIndentedTextWriter.Indent--;
                    break;
            }
        }
    }
}