using Autodesk.Revit.DB;
using BimOps.Modules.DataExchange.Models;

namespace BimOps.RevitAddin.Revit;

public sealed class RevitDoorUpdater
{
    private readonly Document _document;

    public RevitDoorUpdater(Document document)
    {
        _document = document;
    }

    public DoorImportResult UpdateMarkAndComments(IReadOnlyList<DoorExcelRow> rows)
    {
        var result = new DoorImportResult
        {
            InputRows = rows.Count
        };

        var seenUniqueIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        using var transaction = new Transaction(_document, "BimOps - Import Door Mark/Comments");
        transaction.Start();

        foreach (var row in rows)
        {
            if (string.IsNullOrWhiteSpace(row.UniqueId))
            {
                result.SkippedRows++;
                result.Errors.Add($"Row {row.RowNumber}: UniqueId가 비어 있습니다.");
                continue;
            }

            if (!seenUniqueIds.Add(row.UniqueId))
            {
                result.SkippedRows++;
                result.Errors.Add($"Row {row.RowNumber}: 중복 UniqueId입니다. {row.UniqueId}");
                continue;
            }

            var element = _document.GetElement(row.UniqueId);

            if (element == null)
            {
                result.SkippedRows++;
                result.Errors.Add($"Row {row.RowNumber}: Revit에서 Element를 찾지 못했습니다. {row.UniqueId}");
                continue;
            }

            int changedParameters = 0;
            var changedDescriptions = new List<string>();

            if (TrySetTextParameter(
                    element,
                    BuiltInParameter.ALL_MODEL_MARK,
                    row.Mark,
                    row.RowNumber,
                    "Mark",
                    result,
                    out string oldMark))
            {
                changedParameters++;
                changedDescriptions.Add($"Mark: '{oldMark}' → '{row.Mark}'");
            }

            if (TrySetTextParameter(
                    element,
                    BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS,
                    row.Comments,
                    row.RowNumber,
                    "Comments",
                    result,
                    out string oldComments))
            {
                changedParameters++;
                changedDescriptions.Add($"Comments: '{oldComments}' → '{row.Comments}'");
            }

            if (changedParameters > 0)
            {
                result.UpdatedElements++;
                result.UpdatedParameters += changedParameters;
                result.UpdatedElementIds.Add(element.Id);

                result.ChangedItems.Add(
                    $"Row {row.RowNumber}, ElementId {element.Id}: {string.Join(", ", changedDescriptions)}");
            }
            else
            {
                result.SkippedRows++;
            }
        }

        transaction.Commit();

        return result;
    }

    private static bool TrySetTextParameter(
        Element element,
        BuiltInParameter builtInParameter,
        string newValue,
        int rowNumber,
        string displayName,
        DoorImportResult result,
        out string oldValue)
    {
        oldValue = "";

        var parameter = element.get_Parameter(builtInParameter);

        if (parameter == null)
        {
            result.Errors.Add($"Row {rowNumber}: {displayName} Parameter를 찾지 못했습니다.");
            return false;
        }

        if (parameter.IsReadOnly)
        {
            result.Errors.Add($"Row {rowNumber}: {displayName} Parameter가 읽기 전용입니다.");
            return false;
        }

        oldValue = parameter.AsString() ?? "";
        newValue ??= "";

        if (string.Equals(oldValue, newValue, StringComparison.Ordinal))
            return false;

        try
        {
            parameter.Set(newValue);
            return true;
        }
        catch (Exception ex)
        {
            result.Errors.Add($"Row {rowNumber}: {displayName} 수정 실패 - {ex.Message}");
            return false;
        }
    }
}