using System;
using System.Windows.Media;

namespace BimOps.UI.Models
{
    /// <summary>탭 1: 산출 이력 한 줄 = 한 차수의 산출 스냅샷</summary>
    public class RoundHistoryItem
    {
        public string RoundName { get; set; }
        public string Status { get; set; }              // "DRAFT" / "FROZEN"
        public DateTime CalcAt { get; set; }
        public int UnitCount { get; set; }
        public int AppliedCount { get; set; }
        public string Operator { get; set; }
        public bool ExcelExported { get; set; }

        public string CalcAtLabel => CalcAt.ToString("yyyy-MM-dd HH:mm");
        public string ExportedLabel => ExcelExported ? "출력 완료" : "—";
    }

    /// <summary>탭 2: Diff 행</summary>
    public enum DiffChangeType { Added, Removed, Relocated, QtyChanged }

    public class DiffChangeRow
    {
        public string Building { get; set; }
        public string UnitNo { get; set; }
        public DiffChangeType ChangeType { get; set; }
        public string OptionName { get; set; }
        public string ChangeDescription { get; set; }   // "침실2 → 침실3"
        public string FinishImpact { get; set; }        // "벽지 +1.8 / 마루 +0.4"

        public string UnitLabel => $"{Building}-{UnitNo}";

        public string ChangeTypeText
        {
            get
            {
                switch (ChangeType)
                {
                    case DiffChangeType.Added: return "추가";
                    case DiffChangeType.Removed: return "취소";
                    case DiffChangeType.Relocated: return "위치변경";
                    case DiffChangeType.QtyChanged: return "수량변경";
                    default: return "—";
                }
            }
        }

        public Brush ChangeTypeBgBrush
        {
            get
            {
                switch (ChangeType)
                {
                    case DiffChangeType.Added: return (Brush)new BrushConverter().ConvertFrom("#E1F5EE");
                    case DiffChangeType.Removed: return (Brush)new BrushConverter().ConvertFrom("#FCEBEB");
                    case DiffChangeType.Relocated: return (Brush)new BrushConverter().ConvertFrom("#FAEEDA");
                    case DiffChangeType.QtyChanged: return (Brush)new BrushConverter().ConvertFrom("#E6F1FB");
                    default: return Brushes.Transparent;
                }
            }
        }

        public Brush ChangeTypeFgBrush
        {
            get
            {
                switch (ChangeType)
                {
                    case DiffChangeType.Added: return (Brush)new BrushConverter().ConvertFrom("#0F6E56");
                    case DiffChangeType.Removed: return (Brush)new BrushConverter().ConvertFrom("#A32D2D");
                    case DiffChangeType.Relocated: return (Brush)new BrushConverter().ConvertFrom("#854F0B");
                    case DiffChangeType.QtyChanged: return (Brush)new BrushConverter().ConvertFrom("#0C447C");
                    default: return Brushes.Black;
                }
            }
        }
    }
}