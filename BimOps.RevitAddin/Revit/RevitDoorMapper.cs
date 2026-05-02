using Autodesk.Revit.DB;
using BimOps.Modules.DataExchange.Models;

namespace BimOps.RevitAddin.Revit;

public sealed class RevitDoorMapper
{
    private readonly Document _document;

    public RevitDoorMapper(Document document)
    {
        _document = document;
    }

    public DoorExcelRow Map(FamilyInstance door)
    {
        var type = _document.GetElement(door.GetTypeId()) as ElementType;
        var level = _document.GetElement(door.LevelId);

        return new DoorExcelRow
        {
            UniqueId = door.UniqueId,
            ElementId = door.Id.ToString(),
            Level = level?.Name ?? "",
            Family = type?.FamilyName ?? "",
            Type = type?.Name ?? "",

            Mark = ReadParameter(door, BuiltInParameter.ALL_MODEL_MARK),
            Comments = ReadParameter(door, BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS),

            Width = ReadParameter(type, BuiltInParameter.DOOR_WIDTH),
            Height = ReadParameter(type, BuiltInParameter.DOOR_HEIGHT)
        };
    }

    private static string ReadParameter(Element? element, BuiltInParameter builtInParameter)
    {
        if (element == null)
            return "";

        var parameter = element.get_Parameter(builtInParameter);

        if (parameter == null)
            return "";

        return ReadParameterValue(parameter);
    }

    private static string ReadParameterValue(Parameter parameter)
    {
        return parameter.StorageType switch
        {
            StorageType.String => parameter.AsString() ?? "",
            StorageType.Integer => parameter.AsValueString() ?? parameter.AsInteger().ToString(),
            StorageType.Double => parameter.AsValueString() ?? parameter.AsDouble().ToString("G"),
            StorageType.ElementId => parameter.AsValueString() ?? parameter.AsElementId().ToString(),
            _ => ""
        };
    }
}