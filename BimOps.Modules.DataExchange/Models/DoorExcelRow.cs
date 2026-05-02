namespace BimOps.Modules.DataExchange.Models;

public sealed class DoorExcelRow
{
    public int RowNumber { get; set; }

    public string UniqueId { get; set; } = "";
    public string ElementId { get; set; } = "";
    public string Level { get; set; } = "";
    public string Family { get; set; } = "";
    public string Type { get; set; } = "";

    public string Mark { get; set; } = "";
    public string Comments { get; set; } = "";

    public string Width { get; set; } = "";
    public string Height { get; set; } = "";
}