using System;
using System.Data;
using System.Text;
using DataTool.FormatAccess.ExcelAccess.ExcelRead.ContentRead.AdvancedSingleRead;
using DataTool.FormatAccess.ExcelAccess.ExcelRead.ContentRead.Auxiliary;
using DataToolInterface.Format.File.Excel;
using DataToolLog;

namespace DataTool.FormatAccess.ExcelAccess.ExcelRead.ContentRead
{
    public class ContentReader
    {
        private readonly Type _resultType;
        private readonly DataTable _dataTable;
        private readonly SheetContentAdvancedSingleAttribute _sheetContentBasicMultipleAttribute;

        private Position _position;
        private StringBuilder _stringBuilder;

        public object resultObject { get; private set; }

        public ContentReader(Type paramType,DataTable paramDataTable,SheetContentAdvancedSingleAttribute sheetContentBasicMultipleAttribute)
        {
            _resultType = paramType;
            _dataTable = paramDataTable;
            _sheetContentBasicMultipleAttribute = sheetContentBasicMultipleAttribute;
            
            ReadContent();
        }

        private void ReadContent()
        {
            _position = new Position()
            {
                row = 1,
                column = 1,
            };
            _stringBuilder = new StringBuilder();
            
            var contentAdvancedSingleReader = new AdvancedSingleReader(_resultType,_dataTable,_sheetContentBasicMultipleAttribute,_position,_stringBuilder);
            if (contentAdvancedSingleReader.isValid)
            {
                resultObject = contentAdvancedSingleReader.resultObject;
            }
            
            if (_stringBuilder.Length>0)
            {
                LogHelper.DefaultLog.WriteLine(_stringBuilder.ToString());
            }
        }
    }
}