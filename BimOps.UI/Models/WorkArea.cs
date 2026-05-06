
namespace BimOps.UI.Models
{
    /// <summary>
    /// 메인 윈도우의 라우팅 키. 화면 추가/변경 시 이 enum과 MainWindow의 AreaMeta를 함께 갱신.
    /// </summary>
    public enum WorkArea
    {
        Home,                // 프로젝트 홈 (허브)
        ReferenceData,       // 기준 데이터 (Revit 기본물량 + 평형 + 옵션 + 룩업)
        UnitOptionStatus,    // 세대 옵션 현황
        QuantityCalculation, // 옵션 물량 산출 (실행)
        QuantityResult,      // 산출 결과 (조회)
        HistoryReport        // 산출 이력 / 보고서 (이력 + Diff + 출력)
    }

    public class WorkAreaEventArgs : System.EventArgs
    {
        public WorkArea Area { get; }
        public WorkAreaEventArgs(WorkArea area) { Area = area; }
    }
}