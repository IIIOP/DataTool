using System;
using System.IO;
using DataTool.FormatAccess.Auxiliary;
using DataToolInterface.Format.File;

namespace DataTool.FormatAccess.CSharpAccess.CSharpRead
{
    [FormatAccess(format = FileFormatEnum.CSharp)]
    public class CSharpReader:FormatReader
    {
        //TODO 10years later
        public CSharpReader(Type paramType, FileInfo paramFileInfo) : base(paramType, paramFileInfo)
        {
        }

        private void ReadFormat()
        {
            throw new NotImplementedException();
        }
    }
}