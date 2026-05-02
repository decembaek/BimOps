using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using RevitTaskDialog = Autodesk.Revit.UI.TaskDialog;

namespace BimOps.RevitAddin.App;



public class Application_DialogBoxShowing : IExternalApplication
{
    // Implement the OnStartup method to register events when Revit starts.
    public Result OnStartup(UIControlledApplication application)
    {
        RevitTaskDialog.Show("BimOps", "Application loaded");
        application.DialogBoxShowing += AppDialogShowing;

        RibbonPanel panel = application.CreateRibbonPanel("BimOps");

        PushButtonData exportButton = new PushButtonData(
            "ExportDoorsToExcel",
            "Export\nDoors",
            @"C:\RevitAddins\BimOps\BimOps.RevitAddin.dll",
            "BimOps.RevitAddin.Commands.ExportDoorsToExcelCommand"
        );

        panel.AddItem(exportButton);

        PushButtonData importButton = new PushButtonData(
            "ImportDoorsFromExcel",
            "Import\nDoors",
            @"C:\RevitAddins\BimOps\BimOps.RevitAddin.dll",
            "BimOps.RevitAddin.Commands.ImportDoorsFromExcelCommand"
        );

        panel.AddItem(importButton);

        return Result.Succeeded;
    }

    // Implement this method to unregister the subscribed events when Revit exits.
    public Result OnShutdown(UIControlledApplication application)
    {

        // unregister events
        application.DialogBoxShowing -=
new EventHandler<DialogBoxShowingEventArgs>(AppDialogShowing);
        return Result.Succeeded;
    }

    // The DialogBoxShowing event handler, which allow you to 
    // do some work before the dialog shows
    void AppDialogShowing(object sender, DialogBoxShowingEventArgs args)
    {
        // Get the help id of the showing dialog
        string dialogId = args.DialogId;

        // return if the dialog has no DialogId (such as with a Task Dialog)
        if (dialogId == "")
            return;

        // Show the prompt message and allow the user to close the dialog directly.
        RevitTaskDialog taskDialog = new RevitTaskDialog("Revit");
        taskDialog.MainContent = "A Revit dialog is about to be opened.\n" +
            "The DialogId of this dialog is " + dialogId + "\n" +
            "Press 'Cancel' to immediately dismiss the dialog";
        taskDialog.CommonButtons = TaskDialogCommonButtons.Ok |
                                     TaskDialogCommonButtons.Cancel;
        TaskDialogResult result = taskDialog.Show();
        if (TaskDialogResult.Cancel == result)
        {
            // dismiss the Revit dialog 
            args.OverrideResult(1);
        }
    }
}