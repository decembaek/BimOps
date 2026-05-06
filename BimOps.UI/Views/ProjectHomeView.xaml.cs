using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using BimOps.UI.Models;


namespace BimOps.UI.Views
{
    
    public partial class ProjectHomeView : UserControl
    {
        public ProjectCardItem CurrentProject { get; private set; }
        public ObservableCollection<RoundTimelineItem> Rounds { get; }
            = new ObservableCollection<RoundTimelineItem>();

        // 외부(MainWindow)로 사용자의 의도를 알리는 이벤트
        public event EventHandler<WorkAreaEventArgs> WorkAreaSelected;
        public event EventHandler<RoundTimelineItem> RoundSelected;
        public event EventHandler RequestNewRound;
        public event EventHandler RequestEditProject;

        public ProjectHomeView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// MainWindow가 ProjectSelectionWindow에서 받은 프로젝트를 주입.
        /// </summary>
        public void LoadProject(ProjectCardItem project, IEnumerable<RoundTimelineItem> rounds)
        {
            CurrentProject = project ?? throw new ArgumentNullException(nameof(project));

            // 1. 단지 메타
            TxtProjectTitle.Text = $"{project.Code} / {project.Name}";
            TxtProjectMeta.Text = $"총 {project.BuildingCount}동 {project.UnitCount:N0}세대 · 평형타입 {project.UnitTypes}";

            // 2. 타임라인
            Rounds.Clear();
            var list = new List<RoundTimelineItem>(rounds ?? Array.Empty<RoundTimelineItem>());
            for (int i = 0; i < list.Count; i++)
            {
                list[i].IsLast = (i == list.Count - 1);
                Rounds.Add(list[i]);
            }
            RoundTimeline.ItemsSource = Rounds;

            // 3. 작업 카드 메타 (실제 환경에서는 서비스에서 통계 조회)
            UpdateWorkCardMeta();
        }

        private void UpdateWorkCardMeta()
        {
            // TODO: 서비스 호출로 교체. 지금은 예시 값.
            var current = FindCurrentRound();
            string roundLabel = current?.Name ?? "—";

            TxtReferenceMeta.Text = "옵션 27종 · 차감 룩업 312행";
            TxtUnitOptionMeta.Text = CurrentProject != null
                ? $"{roundLabel} · {CurrentProject.UnitCount:N0}세대 · 이슈 0건"
                : "데이터 없음";
            TxtCalcMeta.Text = current != null ? $"{roundLabel} · 산출 가능" : "산출 대상 없음";
            TxtResultMeta.Text = current != null ? $"최종 산출 {current.Date:MM-dd HH:mm}" : "산출 이력 없음";
            TxtHistoryMeta.Text = Rounds.Count >= 2
                ? $"비교 가능 · {Rounds.Count}개 차수"
                : "이력 부족";
        }

        private RoundTimelineItem FindCurrentRound()
        {
            // 가장 최근 차수 (DRAFT 우선, 없으면 마지막 FROZEN)
            for (int i = Rounds.Count - 1; i >= 0; i--)
                if (Rounds[i].Status == "DRAFT") return Rounds[i];
            return Rounds.Count > 0 ? Rounds[Rounds.Count - 1] : null;
        }

        // ===== 이벤트 핸들러 =====
        private void RoundCard_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement el && el.Tag is RoundTimelineItem round)
                RoundSelected?.Invoke(this, round);
        }

        private void BtnAddRound_Click(object sender, RoutedEventArgs e)
            => RequestNewRound?.Invoke(this, EventArgs.Empty);

        private void BtnEditProject_Click(object sender, RoutedEventArgs e)
            => RequestEditProject?.Invoke(this, EventArgs.Empty);

        private void WorkCard_Click(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is FrameworkElement el)) return;
            if (!(el.Tag is string tag)) return;
            if (!Enum.TryParse(tag, out WorkArea area)) return;

            WorkAreaSelected?.Invoke(this, new WorkAreaEventArgs(area));
        }
    }

    public class RoundTimelineItem
    {
        public string Name { get; set; }            // "1차", "변경1", "2차"
        public DateTime Date { get; set; }
        public string Status { get; set; }          // "DRAFT" / "FROZEN"
        public bool IsCurrent { get; set; }         // DRAFT인 작업 중 차수
        public bool IsLast { get; set; }            // 화살표 표시 제어용

        // 표시용 가공 프로퍼티
        public string DateLabel => Date.ToString("MM-dd");

        public Visibility ArrowVisibility
            => IsLast ? Visibility.Collapsed : Visibility.Visible;

        public Visibility LockIconVisibility
            => Status == "FROZEN" ? Visibility.Visible : Visibility.Collapsed;

        public Brush BorderBrush => IsCurrent
            ? (Brush)new BrushConverter().ConvertFrom("#1E5A8E")
            : (Brush)new BrushConverter().ConvertFrom("#D0D0D0");

        public Thickness BorderThickness => new Thickness(IsCurrent ? 2 : 1);

        public Brush StatusBgBrush => Status == "DRAFT"
            ? (Brush)new BrushConverter().ConvertFrom("#E6F1FB")
            : (Brush)new BrushConverter().ConvertFrom("#F1EFE8");

        public Brush StatusFgBrush => Status == "DRAFT"
            ? (Brush)new BrushConverter().ConvertFrom("#0C447C")
            : (Brush)new BrushConverter().ConvertFrom("#444441");
    }
}