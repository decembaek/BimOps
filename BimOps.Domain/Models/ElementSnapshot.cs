namespace BimOps.Domain.Models
{
    public class ElementSnapshot
    {
        public int ElementId { get; set; }
        public string UniqueId { get; set; }
        public string Category { get; set; }
        public string FamilyName { get; set; }
        public string TypeName { get; set; }
        public string LevelName { get; set; }
        public Dictionary<string, string> Parameters { get; set; } = new();
    }
}
