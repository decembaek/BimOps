using Autodesk.Revit.DB;

namespace BimOps.RevitAddin.Revit;

public sealed class DoorImportResult
{
    public int InputRows { get; set; }
    public int UpdatedElements { get; set; }
    public int UpdatedParameters { get; set; }
    public int SkippedRows { get; set; }

    public List<ElementId> UpdatedElementIds { get; } = new();
    public List<string> ChangedItems { get; } = new();
    public List<string> Errors { get; } = new();

    public string ToSummaryMessage()
    {
        var message =
            $"입력 Row: {InputRows}\n" +
            $"수정된 Door: {UpdatedElements}\n" +
            $"수정된 Parameter: {UpdatedParameters}\n" +
            $"스킵 Row: {SkippedRows}\n" +
            $"오류: {Errors.Count}";

        if (ChangedItems.Count > 0)
        {
            message += "\n\n변경 내용 일부:\n";
            message += string.Join("\n", ChangedItems.Take(8));

            if (ChangedItems.Count > 8)
                message += $"\n... 외 {ChangedItems.Count - 8}개";
        }

        if (Errors.Count > 0)
        {
            message += "\n\n오류 일부:\n";
            message += string.Join("\n", Errors.Take(8));

            if (Errors.Count > 8)
                message += $"\n... 외 {Errors.Count - 8}개";
        }

        return message;
    }
}