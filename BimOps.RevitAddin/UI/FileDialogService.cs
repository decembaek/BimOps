using System.Windows.Forms;

namespace BimOps.RevitAddin.UI;

public static class FileDialogService
{
    public static string? PickSaveExcelFile(string defaultFileName)
    {
        using var dialog = new SaveFileDialog
        {
            Title = "Export Doors To Excel",
            Filter = "Excel Workbook (*.xlsx)|*.xlsx",
            FileName = defaultFileName,
            DefaultExt = "xlsx",
            AddExtension = true,
            OverwritePrompt = true
        };

        return dialog.ShowDialog() == DialogResult.OK
            ? dialog.FileName
            : null;
    }

    public static string? PickOpenExcelFile()
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Import Doors From Excel",
            Filter = "Excel Workbook (*.xlsx)|*.xlsx",
            Multiselect = false,
            CheckFileExists = true
        };

        return dialog.ShowDialog() == DialogResult.OK
            ? dialog.FileName
            : null;
    }
}