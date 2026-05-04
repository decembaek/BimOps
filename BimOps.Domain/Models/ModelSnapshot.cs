namespace BimOps.Domain.Models;

public class ModelSnapshot
{
    public string ProjectName { get; set; }
    public string DocumentTitle { get; set; }
    public string RevitVersion { get; set; }
    public DateTime CapturedAt { get; set; }
    public List<ElementSnapshot> Elements { get; set; } = new();
    public List<ViewSnapshot> Views { get; set; } = new();
    public List<SheetSnapshot> Sheets { get; set; } = new();
    public List<LinkSnapshot> Links { get; set; } = new();

}