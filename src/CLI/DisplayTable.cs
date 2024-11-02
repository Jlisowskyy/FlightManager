using System.Text;

namespace proj.CLI;

public class DisplayTable
{
// ------------------------------
// Class creation
// ------------------------------

    public DisplayTable(params string[] columns)
    {
        if (columns.Length == 0)
            throw new Exception("No columns provided");
        
        _columns = new List<string>[columns.Length];
        _columnNames = new string[columns.Length];
        _columnWidths = new int[columns.Length];
        
        columns.CopyTo(_columnNames, 0);
        for (int i = 0; i < columns.Length; i++)
        {
            _columns[i] = new List<string>();
            _columnWidths[i] = columns[i].Length;
        }
    }

// ------------------------------
// Class interaction
// ------------------------------

    public int ColumnCount => _columnNames.Length;

    public void AddRows(params string[] rows)
    {
        if (rows.Length != ColumnCount)
            throw new Exception("Invalid number of columns");

        for (int i = 0; i < ColumnCount; ++i)
        {
            _columns[i].Add(rows[i]);
            _columnWidths[i] = Math.Max(_columnWidths[i], rows[i].Length);
        }
        
        ++_rowCount;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < ColumnCount - 1; ++i)
            sb.Append($" {_columnNames[i].PadRight(_columnWidths[i])} |");
        sb.Append($" {_columnNames[ColumnCount - 1].PadRight(_columnWidths[ColumnCount - 1])}\n");

        for (int i = 0; i < ColumnCount-1; ++i)
            sb.Append(new string('-', _columnWidths[i]+2) + "+");
        sb.Append(new string('-', _columnWidths[ColumnCount-1]+2) + "\n");
        
        for (int row = 0; row < _rowCount; ++row)
        {
            for (int col = 0; col < ColumnCount-1; ++col)
                sb.Append($" {_columns[col][row].PadLeft(_columnWidths[col])} |");
            sb.Append($" {_columns[ColumnCount-1][row].PadLeft(_columnWidths[ColumnCount-1])}\n");
        }
        
        for (int i = 0; i < ColumnCount-1; ++i)
            sb.Append(new string(' ', _columnWidths[i]+2) + "|");
        sb.Append("\n");

        return sb.ToString();
    }

// ------------------------------
// Class fields
// ------------------------------

    private int _rowCount;
    private readonly List<string>[] _columns;
    private readonly string[] _columnNames;
    private readonly int[] _columnWidths;
}