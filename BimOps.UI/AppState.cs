using System;
using System.Collections.Generic;
using BimOps.UI.Views;

namespace BimOps.UI
{
    /// <summary>
    /// 프로세스 단위 전역 상태 슬롯. ProjectSelectionWindow → MainWindow 전환 시 데이터 전달용.
    /// 실제 환경에서는 DI / 상태 관리 라이브러리로 교체.
    /// </summary>
    public static class AppState
    {
        public static ProjectCardItem SelectedProject { get; set; }
        public static IEnumerable<ProjectCardItem> AvailableProjects { get; set; }

        // 차수 로더 (서비스 주입 자리)
        public static Func<ProjectCardItem, IEnumerable<RoundTimelineItem>> LoadRounds { get; set; }
            = _ => null;
    }
}