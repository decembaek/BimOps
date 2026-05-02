using BimOps.Modules.DataExchange.Models;
using ClosedXML.Excel;

namespace BimOps.Modules.DataExchange.Excel;

public sealed class DoorExcelExporter
{
    public void Export(IEnumerable<DoorExcelRow> rows, string filePath)
    {
        var list = rows.ToList();

        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Doors");

        WriteHeaders(sheet);
        WriteRows(sheet, list);

        sheet.SheetView.FreezeRows(1);
        sheet.Columns().AdjustToContents();

        var lastRow = Math.Max(1, list.Count + 1);
        var lastColumn = ExcelColumnNames.DoorHeaders.Length;
        var range = sheet.Range(1, 1, lastRow, lastColumn);
        range.CreateTable("DoorData");

        workbook.SaveAs(filePath);
    }

    private static void WriteHeaders(IXLWorksheet sheet)
    {
        for (int i = 0; i < ExcelColumnNames.DoorHeaders.Length; i++)
        {
            var cell = sheet.Cell(1, i + 1);
            cell.Value = ExcelColumnNames.DoorHeaders[i];
            cell.Style.Font.Bold = true;
        }
    }

    private static void WriteRows(IXLWorksheet sheet, IReadOnlyList<DoorExcelRow> rows)
    {
        for (int i = 0; i < rows.Count; i++)
        {
            int row = i + 2;
            var item = rows[i];

            sheet.Cell(row, 1).Value = item.UniqueId;
            sheet.Cell(row, 2).Value = item.ElementId;
            sheet.Cell(row, 3).Value = item.Level;
            sheet.Cell(row, 4).Value = item.Family;
            sheet.Cell(row, 5).Value = item.Type;
            sheet.Cell(row, 6).Value = item.Mark;
            sheet.Cell(row, 7).Value = item.Comments;
            sheet.Cell(row, 8).Value = item.Width;
            sheet.Cell(row, 9).Value = item.Height;
        }
    }
}