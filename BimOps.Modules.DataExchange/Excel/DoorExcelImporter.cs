using BimOps.Modules.DataExchange.Models;
using ClosedXML.Excel;

namespace BimOps.Modules.DataExchange.Excel;

public sealed class DoorExcelImporter
{
    public IReadOnlyList<DoorExcelRow> Import(string filePath)
    {
        using var workbook = new XLWorkbook(filePath);
        var sheet = workbook.Worksheets.First();

        var columns = ReadHeaderColumns(sheet);
        ValidateRequiredColumns(columns);

        var rows = new List<DoorExcelRow>();
        var lastRow = sheet.LastRowUsed()?.RowNumber() ?? 1;

        for (int rowNumber = 2; rowNumber <= lastRow; rowNumber++)
        {
            string uniqueId = ReadCell(sheet, rowNumber, columns, ExcelColumnNames.UniqueId);

            if (string.IsNullOrWhiteSpace(uniqueId))
                continue;

            rows.Add(new DoorExcelRow
            {
                RowNumber = rowNumber,
                UniqueId = uniqueId,
                ElementId = ReadCell(sheet, rowNumber, columns, ExcelColumnNames.ElementId),
                Level = ReadCell(sheet, rowNumber, columns, ExcelColumnNames.Level),
                Family = ReadCell(sheet, rowNumber, columns, ExcelColumnNames.Family),
                Type = ReadCell(sheet, rowNumber, columns, ExcelColumnNames.Type),
                Mark = ReadCell(sheet, rowNumber, columns, ExcelColumnNames.Mark),
                Comments = ReadCell(sheet, rowNumber, columns, ExcelColumnNames.Comments),
                Width = ReadCell(sheet, rowNumber, columns, ExcelColumnNames.Width),
                Height = ReadCell(sheet, rowNumber, columns, ExcelColumnNames.Height)
            });
        }

        return rows;
    }

    private static Dictionary<string, int> ReadHeaderColumns(IXLWorksheet sheet)
    {
        var result = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var headerRow = sheet.Row(1);

        foreach (var cell in headerRow.CellsUsed())
        {
            string header = cell.GetString().Trim();

            if (string.IsNullOrWhiteSpace(header))
                continue;

            result[header] = cell.Address.ColumnNumber;
        }

        return result;
    }

    private static void ValidateRequiredColumns(Dictionary<string, int> columns)
    {
        string[] required =
        {
            ExcelColumnNames.UniqueId,
            ExcelColumnNames.Mark,
            ExcelColumnNames.Comments
        };

        foreach (string column in required)
        {
            if (!columns.ContainsKey(column))
                throw new InvalidOperationException($"필수 컬럼이 없습니다: {column}");
        }
    }

    private static string ReadCell(
        IXLWorksheet sheet,
        int rowNumber,
        Dictionary<string, int> columns,
        string columnName)
    {
        if (!columns.TryGetValue(columnName, out int columnNumber))
            return "";

        return sheet.Cell(rowNumber, columnNumber).GetString().Trim();
    }
}