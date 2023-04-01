using DataToolInterface.Format.File.Excel;

namespace DataTool.FormatAccess.ExcelAccess.ExcelRead.ContentRead.Auxiliary
{
    public static class StaticMethods
    {
        public static RowColumnModeEnum GetAutoMode(string paramRow,string paramColumn)
        {
            RowColumnModeEnum result = RowColumnModeEnum.Non;
            if (paramRow!=ConfigRowColumnValueEnum.Auto.ToString()&&paramColumn!=ConfigRowColumnValueEnum.Auto.ToString())
            {
                result = RowColumnModeEnum.Non;
            }
            else if (paramRow!=ConfigRowColumnValueEnum.Auto.ToString()&&paramColumn==ConfigRowColumnValueEnum.Auto.ToString())
            {
                result = RowColumnModeEnum.Column;
            }
            else if (paramRow==ConfigRowColumnValueEnum.Auto.ToString()&&paramColumn!=ConfigRowColumnValueEnum.Auto.ToString())
            {
                result = RowColumnModeEnum.Row;
            }
            else if (paramRow==ConfigRowColumnValueEnum.Auto.ToString()&&paramColumn==ConfigRowColumnValueEnum.Auto.ToString())
            {
                result = RowColumnModeEnum.RowColumn;
            }
            return result;
        }
    }
}