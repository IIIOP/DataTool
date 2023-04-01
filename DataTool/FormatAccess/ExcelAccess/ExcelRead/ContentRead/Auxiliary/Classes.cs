namespace DataTool.FormatAccess.ExcelAccess.ExcelRead.ContentRead.Auxiliary
{
    public class Position
    {
        public int row { get; set; }
        public int column { get; set; }

        public Position(int paramRow = 1,int paramColumn = 1)
        {
            row = paramRow;
            column = paramColumn;
        }

        public Position Clone()
        {
            return new Position
            {
                row = this.row,
                column = this.column,
            };
        }
    }
}