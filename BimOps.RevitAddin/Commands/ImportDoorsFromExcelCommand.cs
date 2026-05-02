using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BimOps.Modules.DataExchange.Excel;
using BimOps.RevitAddin.Revit;
using BimOps.RevitAddin.UI;
using RevitTaskDialog = Autodesk.Revit.UI.TaskDialog;

namespace BimOps.RevitAddin.Commands;

[Transaction(TransactionMode.Manual)]
public sealed class ImportDoorsFromExcelCommand : IExternalCommand
{
    public Result Execute(
        ExternalCommandData commandData,
        ref string message,
        ElementSet elements)
    {
        try
        {
            UIDocument? uidoc = commandData.Application.ActiveUIDocument;
            Document? document = uidoc?.Document;

            if (uidoc == null || document == null)
            {
                message = "열려 있는 Revit 문서가 없습니다.";
                return Result.Failed;
            }

            string? filePath = FileDialogService.PickOpenExcelFile();

            if (string.IsNullOrWhiteSpace(filePath))
                return Result.Cancelled;

            var importer = new DoorExcelImporter();
            var rows = importer.Import(filePath);

            var updater = new RevitDoorUpdater(document);
            var result = updater.UpdateMarkAndComments(rows);

            if (result.UpdatedElementIds.Count > 0)
            {
                uidoc.Selection.SetElementIds(result.UpdatedElementIds);

                try
                {
                    uidoc.ShowElements(result.UpdatedElementIds);
                }
                catch
                {
                    // 현재 View에서 표시가 어려운 경우도 있으니 선택만 유지
                }
            }

            RevitTaskDialog.Show(
                "BimOps - Import 완료",
                result.ToSummaryMessage());

            return Result.Succeeded;
        }
        catch (Exception ex)
        {
            message = ex.ToString();

            RevitTaskDialog.Show(
                "BimOps - Import 실패",
                ex.Message);

            return Result.Failed;
        }
    }
}