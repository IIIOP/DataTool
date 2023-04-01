using System.Collections;
using System.Data;
using DataTool.FormatAccess.ExcelAccess.ExcelWrite.ExcelSheetContentWrite.ContentAdvancedSingleWrite;
using DataToolInterface.Data;
using DataToolInterface.Format.File.Excel;
using DataToolLog;

namespace DataTool.FormatAccess.ExcelAccess.ExcelWrite.ExcelSheetContentWrite.ContentAdvancedMultipleWrite
{
    public class ContentAdvancedMultipleWriter
    {
        private int _row;
        private int _column;
        private readonly object _object;
        private readonly DataTable _dataTable;
        private readonly SheetContentAdvancedMultipleAttribute _sheetContentAdvancedMultipleAttribute;
        
        private int  itemRow;
        private int  itemColumn;
        public int edgeRow { get; private set; }
        public int edgeColumn { get; private set; }
        
        
        public ContentAdvancedMultipleWriter(int paramRow,int paramColumn,object paramObject, DataTable paramDataTable, SheetContentAdvancedMultipleAttribute paramSheetContentAdvancedMultipleAttribute)
        {
            _row = paramRow;
            _column = paramColumn;
            _object = paramObject;
            _dataTable = paramDataTable;
            _sheetContentAdvancedMultipleAttribute = paramSheetContentAdvancedMultipleAttribute;

            DoWork();
        }

        private void DoWork()
        {
            var row = int.Parse(_sheetContentAdvancedMultipleAttribute.row);
            var column = int.Parse(_sheetContentAdvancedMultipleAttribute.column);
            _row = _row + row - 1;
            _column = _column + column - 1;

            itemRow = 0;
            itemColumn = 0;
            
            if (_object is IList list)
            {
                foreach (var item in list)
                {
                    var sheetContentAdvancedSingleAttribute = new SheetContentAdvancedSingleAttribute()
                    {
                        row = (itemRow+1).ToString(),
                        column = (itemColumn+1).ToString(),
                        type = AdvancedDataTypeEnum.Single.ToString(),
                        modelRowCount = _sheetContentAdvancedMultipleAttribute.modelRowCount,
                        modelColumnCount = _sheetContentAdvancedMultipleAttribute.modelColumnCount,
                        describe = _sheetContentAdvancedMultipleAttribute.describe+"ListItem",
                        
                    };
                    var contentAdvancedSingleWriter = new ContentAdvancedSingleWriter(_row, _column, item, _dataTable, sheetContentAdvancedSingleAttribute);
                    if (contentAdvancedSingleWriter.edgeRow>edgeRow)
                    {
                        edgeRow = contentAdvancedSingleWriter.edgeRow;
                    }
                    if (contentAdvancedSingleWriter.edgeColumn>edgeColumn)
                    {
                        edgeColumn = contentAdvancedSingleWriter.edgeColumn;
                    }
                    CalculateNext(contentAdvancedSingleWriter);
                }
            }
        }

        private void CalculateNext(ContentAdvancedSingleWriter contentAdvancedSingleWriter)
        {
            if (int.TryParse(_sheetContentAdvancedMultipleAttribute.totalColumnCount,out var totalColumnCount)&&
                int.TryParse(_sheetContentAdvancedMultipleAttribute.modelColumnCount,out var modelColumnCount)&&
                (_sheetContentAdvancedMultipleAttribute.totalRowCount==ConfigRowColumnValueEnum.Auto.ToString()||
                 (int.TryParse(_sheetContentAdvancedMultipleAttribute.totalRowCount,out var totalRowCount))))
            {
                if (totalColumnCount==modelColumnCount)
                {
                    itemRow = contentAdvancedSingleWriter.edgeRow-_row;
                    itemColumn = 0;
                }
                else if(int.TryParse(_sheetContentAdvancedMultipleAttribute.modelRowCount,out var modelRowCount))
                {
                    if (contentAdvancedSingleWriter.edgeColumn+modelColumnCount<=_column+totalColumnCount)
                    {
                        itemColumn = contentAdvancedSingleWriter.edgeColumn- _column;
                    }
                    else
                    {
                        itemRow += modelRowCount;
                        itemColumn = 0;
                    }
                }
                else
                {
                    LogHelper.DefaultLog.WriteLine("[Error] modelRowCount has to be a digital");
                }
            }
            else if (int.TryParse(_sheetContentAdvancedMultipleAttribute.totalRowCount,out totalRowCount)&&
                     int.TryParse(_sheetContentAdvancedMultipleAttribute.modelRowCount,out var modelRowCount)&&
                     (_sheetContentAdvancedMultipleAttribute.totalColumnCount==ConfigRowColumnValueEnum.Auto.ToString()||
                      (int.TryParse(_sheetContentAdvancedMultipleAttribute.totalColumnCount,out totalColumnCount))))
            {
                if (totalRowCount==modelRowCount)
                {
                    itemRow = 0;
                    itemColumn = contentAdvancedSingleWriter.edgeColumn- _column;
                }
                else
                {
                    LogHelper.DefaultLog.WriteLine("[Error] totalRowCount has to be equal to modelRowCount !!!");
                }
            }
            else
            {
                LogHelper.DefaultLog.WriteLine("[Error] Unsupported Excel format describe!!!");
            }
        }
    }
}