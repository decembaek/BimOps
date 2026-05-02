using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BimOps.Modules.DataExchange.Excel;
using BimOps.RevitAddin.Revit;
using BimOps.RevitAddin.UI;
using RevitTaskDialog = Autodesk.Revit.UI.TaskDialog;

namespace BimOps.RevitAddin.Commands;

[Transaction(TransactionMode.Manual)]
public sealed class ExportDoorsToExcelCommand : IExternalCommand
{
    public Result Execute(
        ExternalCommandData commandData,
        ref string message,
        ElementSet elements)
    {
        try
        {
            var document = commandData.Application.ActiveUIDocument?.Document;

            if (document == null)
            {
                message = "열려 있는 Revit 문서가 없습니다.";
                return Result.Failed;
            }

            string defaultFileName = $"BimOps_Doors_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            string? filePath = FileDialogService.PickSaveExcelFile(defaultFileName);

            if (string.IsNullOrWhiteSpace(filePath))
                return Result.Cancelled;

            var collector = new DoorElementCollector();
            var mapper = new RevitDoorMapper(document);
            var exporter = new DoorExcelExporter();

            var doors = collector.Collect(document);
            var rows = doors.Select(mapper.Map).ToList();

            exporter.Export(rows, filePath);

            RevitTaskDialog.Show(
                "BimOps",
                $"Door Export 완료\n\n개수: {rows.Count}\n파일:\n{filePath}");

            return Result.Succeeded;
        }
        catch (Exception ex)
        {
            message = ex.ToString();

            RevitTaskDialog.Show(
                "BimOps - Export 실패",
                ex.Message);

            return Result.Failed;
        }
    }
}