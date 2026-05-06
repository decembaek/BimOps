using System.Collections.Generic;

namespace BimOps.UI.Models
{
    /// <summary>탭 1: 동별 집계</summary>
    public class BuildingSummary
    {
        public string Building { get; set; }
        public bool IsTotal { get; set; }   // 합계 행 강조용
        public Dictionary<string, double> FinishQuantities { get; set; }
            = new Dictionary<string, double>();
    }

    /// <summary>탭 2: 세대별 명세</summary>
    public class UnitDetailRow
    {
        public string Building { get; set; }
        public string UnitNo { get; set; }
        public string UnitTypeCode { get; set; }
        public string SelectedOptions { get; set; }   // "붙박이장(침실2) · 냉장고장"
        public Dictionary<string, double> FinishQuantities { get; set; }
            = new Dictionary<string, double>();

        public string UnitLabel => $"{Building}-{UnitNo}";
    }

    /// <summary>탭 3: 옵션별 적용 통계</summary>
    public class OptionStatRow
    {
        public string OptionCode { get; set; }
        public string OptionName { get; set; }
        public int AppliedUnits { get; set; }
        // 옵션이 적용된 세대들에서 누적된 마감재별 차감 (절댓값)
        public Dictionary<string, double> FinishDeltas { get; set; }
            = new Dictionary<string, double>();
    }
}