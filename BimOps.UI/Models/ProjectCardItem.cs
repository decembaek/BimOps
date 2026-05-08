using System;
using System.Windows.Media;

namespace BimOps.UI.Models
{
    public enum ProjectStatus
    {
        InProgress,
        Completed,
    }

    public class ProjectCardItem
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int BuildingCount { get; set; }
        public int UnitCount { get; set; }
        public string UnitTypes { get; set; }
        public string LatestRound { get; set; }
        public string LatestStatus { get; set; }      // "DRAFT" / "FROZEN"
        public ProjectStatus Status { get; set; }
        public DateTime LastModified { get; set; }

        // ===== 카드 표시용 가공 프로퍼티 =====

        public string MetaSummary
            => $"{BuildingCount}동 · {UnitCount:N0}세대 · 평형 {UnitTypes}";

        public string LatestRoundLabel
            => $"최신: {LatestRound} ({LatestStatus})";

        public string LastModifiedLabel
            => LastModified.ToString("MM-dd HH:mm");

        public string StatusText
            => Status == ProjectStatus.InProgress ? "진행중" : "완료";

        public Brush StatusBgBrush => Status == ProjectStatus.InProgress
            ? (Brush)new BrushConverter().ConvertFrom("#E6F1FB")
            : (Brush)new BrushConverter().ConvertFrom("#F1EFE8");

        public Brush StatusFgBrush => Status == ProjectStatus.InProgress
            ? (Brush)new BrushConverter().ConvertFrom("#0C447C")
            : (Brush)new BrushConverter().ConvertFrom("#444441");
    }
}