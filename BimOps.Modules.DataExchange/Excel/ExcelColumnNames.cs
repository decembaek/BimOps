namespace BimOps.Modules.DataExchange.Excel;

public static class ExcelColumnNames
{
    public const string UniqueId = "UniqueId";
    public const string ElementId = "ElementId";
    public const string Level = "Level";
    public const string Family = "Family";
    public const string Type = "Type";
    public const string Mark = "Mark";
    public const string Comments = "Comments";
    public const string Width = "Width";
    public const string Height = "Height";

    public static readonly string[] DoorHeaders =
    {
        UniqueId,
        ElementId,
        Level,
        Family,
        Type,
        Mark,
        Comments,
        Width,
        Height
    };
}