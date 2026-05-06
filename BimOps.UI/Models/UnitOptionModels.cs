using System.Collections.Generic;

namespace BimOps.UI.Models
{
    /// <summary>매트릭스 한 행 = 한 세대</summary>
    public class UnitOptionRow
    {
        public string Building { get; set; }
        public string UnitNo { get; set; }
        public string UnitTypeCode { get; set; }
        public bool HasIssue { get; set; }
        public string IssueMessage { get; set; }

        // 옵션코드 → 셀 정보
        public Dictionary<string, OptionCell> OptionCells { get; set; }
            = new Dictionary<string, OptionCell>();

        public string UnitLabel => $"{Building}-{UnitNo}";
    }

    public class OptionCell
    {
        public bool Selected { get; set; }
        public string InstallRoom { get; set; }

        /// <summary>매트릭스 셀에 표시할 텍스트</summary>
        public string DisplayText
        {
            get
            {
                if (!Selected) return "—";
                return string.IsNullOrEmpty(InstallRoom) ? "✓" : InstallRoom;
            }
        }
    }
}