using System.Collections.Generic;

namespace BimOps.UI.Models
{
    /// <summary>평형타입 정보 (예: 84A, 84.92㎡)</summary>
    public class UnitTypeInfo
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public double NetArea { get; set; }
        public string Remark { get; set; }
    }

    /// <summary>마감재 카테고리 (예: 벽지/㎡)</summary>
    public class FinishCategory
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Uom { get; set; }
        public string Remark { get; set; }
    }

    /// <summary>평형타입 × 마감재 단위의 기본 물량</summary>
    public class BaseQuantityRow
    {
        public string UnitTypeCode { get; set; }    // 84A
        public string FinishCode { get; set; }      // WALL
        public string FinishName { get; set; }      // 벽지 (표시용 캐시)
        public string AppliedRoom { get; set; }     // 적용 위치 메모
        public double Quantity { get; set; }
        public string Uom { get; set; }
        public string Source { get; set; }          // "Revit" / "수동"
    }

    /// <summary>옵션 카탈로그 항목</summary>
    public class OptionItem
    {
        public string Code { get; set; }            // BU01
        public string Name { get; set; }            // 붙박이장
        public string Category { get; set; }        // 가구
        public string InstallRooms { get; set; }    // "침실2, 침실3, 드레스룸" (UI용 문자열)
    }

    /// <summary>옵션 차감 룩업 행</summary>
    public class OptionLookupRow
    {
        public string OptionCode { get; set; }
        public string InstallRoom { get; set; }
        public string UnitTypeCode { get; set; }
        public string FinishCode { get; set; }
        public string FinishName { get; set; }
        public double DeltaQty { get; set; }        // 음수 = 차감
        public string Uom { get; set; }
    }
}