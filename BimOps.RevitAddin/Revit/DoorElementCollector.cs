using Autodesk.Revit.DB;

namespace BimOps.RevitAddin.Revit;

public sealed class DoorElementCollector
{
    public IReadOnlyList<FamilyInstance> Collect(Document document)
    {
        return new FilteredElementCollector(document)
            .OfCategory(BuiltInCategory.OST_Doors)
            .WhereElementIsNotElementType()
            .OfType<FamilyInstance>()
            .ToList();
    }
}