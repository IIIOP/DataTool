using System;
using System.IO;
using DataTool.FormatAccess.Auxiliary;
using DataToolInterface.Format.File;

namespace DataTool.FormatAccess.LuaAccess.LuaRead
{
    [FormatAccess(format = FileFormatEnum.Lua)]
    public class LuaReader:FormatReader
    {
        //TODO 100years later
        public LuaReader(Type paramType, FileInfo paramFileInfo) : base(paramType, paramFileInfo)
        {
        }

        private void ReadFormat()
        {
            throw new NotImplementedException();
        }
    }
}