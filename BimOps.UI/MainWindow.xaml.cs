using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BimOps.UI.Views;
using BimOps.UI.Models;

using BimOps.UI;

namespace BimOps.UI
{
    public partial class MainWindow : Window
    {
        // ===== 상태 =====
        private ProjectCardItem _currentProject;
        private readonly ObservableCollection<RoundTimelineItem> _rounds
            = new ObservableCollection<RoundTimelineItem>();
        private RoundTimelineItem _currentRound;
        private WorkArea _currentArea = WorkArea.Home;
        private bool _suppressEvents;

        // 작업 화면별 메타 (제목 + 차수 드롭다운 표시 여부)
        private static readonly Dictionary<WorkArea, WorkAreaInfo> AreaMeta
             = new Dictionary<WorkArea, WorkAreaInfo>
         {
            { WorkArea.ReferenceData,        new WorkAreaInfo("기준 데이터",          false) },
            { WorkArea.UnitOptionStatus,     new WorkAreaInfo("세대 옵션 현황",       true ) },
            { WorkArea.QuantityCalculation,  new WorkAreaInfo("옵션 물량 산출",       true ) },
            { WorkArea.QuantityResult,       new WorkAreaInfo("산출 결과",            true ) },
            { WorkArea.HistoryReport,        new WorkAreaInfo("산출 이력 / 보고서",   false) },
         };

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        // =========================================================
        // 진입 / 초기화
        // =========================================================
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var project = AppState.SelectedProject ?? GetSampleProject();
            LoadProjectContext(project);
            NavigateTo(WorkArea.Home);
        }

        private void LoadProjectContext(ProjectCardItem project)
        {
            _currentProject = project ?? throw new ArgumentNullException(nameof(project));

            // 헤더 단지명 갱신
            TxtHeaderProject.Text = $"{project.Code} / {project.Name}";

            // 차수 목록 로드
            _rounds.Clear();
            var rounds = (AppState.LoadRounds?.Invoke(project) ?? GetSampleRounds()).ToList();
            for (int i = 0; i < rounds.Count; i++)
            {
                rounds[i].IsLast = (i == rounds.Count - 1);
                _rounds.Add(rounds[i]);
            }

            _suppressEvents = true;
            CboRound.ItemsSource = _rounds;
            _currentRound = _rounds.LastOrDefault();
            CboRound.SelectedItem = _currentRound;
            _suppressEvents = false;

            UpdateStatus();
        }

        // =========================================================
        // 화면 라우팅 (단일 진입점)
        // =========================================================
        private void NavigateTo(WorkArea area)
        {
            _currentArea = area;

            if (area == WorkArea.Home)
            {
                ContextBar.Visibility = Visibility.Collapsed;
                MainContent.Content = BuildHomeView();
            }
            else
            {
                var meta = AreaMeta[area];
                ContextBar.Visibility = Visibility.Visible;
                TxtBreadcrumb.Text = meta.Title;
                RoundContextPanel.Visibility = meta.ShowRound
                    ? Visibility.Visible : Visibility.Collapsed;
                MainContent.Content = BuildWorkView(area, meta.Title);
            }

            UpdateStatus();
        }

        private ProjectHomeView BuildHomeView()
        {
            var home = new ProjectHomeView();
            home.WorkAreaSelected += Home_WorkAreaSelected;
            home.RoundSelected += Home_RoundSelected;
            home.RequestNewRound += (_, __) => CreateNewRound();
            home.RequestEditProject += (_, __) => OpenProjectSettings();
            home.LoadProject(_currentProject, _rounds);
            return home;
        }

        // 작업 화면 플레이스홀더 (구현되면 실제 UserControl로 교체)
        private UIElement BuildWorkView(WorkArea area, string title)
        {
            switch (area)
            {
                case WorkArea.ReferenceData:
                    return new ReferenceDataView();

                case WorkArea.UnitOptionStatus:
                    var status = new UnitOptionStatusView();
                    status.GoToCalculation += (_, __) => NavigateTo(WorkArea.QuantityCalculation);
                    return status;

                case WorkArea.QuantityCalculation:
                    var calc = new QuantityCalculationView();
                    calc.GoToResult += (_, __) => NavigateTo(WorkArea.QuantityResult);
                    return calc;

                case WorkArea.QuantityResult:
                    var result = new QuantityResultView();
                    result.GoToHistory += (_, __) => NavigateTo(WorkArea.HistoryReport);
                    result.RecalcRequested += (_, __) => NavigateTo(WorkArea.QuantityCalculation);
                    return result;

                case WorkArea.HistoryReport:
                    var history = new HistoryReportView();
                    history.RoundOpenRequested += (_, item) =>
                    {
                        // 차수 더블클릭 시 해당 차수 컨텍스트로 산출 결과로 이동
                        // TODO: _currentRound 동기화
                        NavigateTo(WorkArea.QuantityResult);
                    };
                    return history;

                default:
                    // placeholder
                    var sp = new StackPanel { Margin = new Thickness(8) };
                    sp.Children.Add(new TextBlock
                    {
                        Text = title,
                        FontSize = 18,
                        FontWeight = FontWeights.SemiBold,
                        Margin = new Thickness(0, 0, 0, 8)
                    });
                    sp.Children.Add(new TextBlock
                    {
                        Text = $"{area} 화면 (구현 예정)",
                        Foreground = System.Windows.Media.Brushes.Gray
                    });
                    return sp;
            }
        }

        // =========================================================
        // 이벤트 핸들러
        // =========================================================
        private void Home_WorkAreaSelected(object sender, WorkAreaEventArgs e)
            => NavigateTo(e.Area);

        private void Home_RoundSelected(object sender, RoundTimelineItem round)
        {
            _suppressEvents = true;
            CboRound.SelectedItem = round;
            _currentRound = round;
            _suppressEvents = false;
            NavigateTo(WorkArea.UnitOptionStatus);
        }

        private void BtnHome_Click(object sender, RoutedEventArgs e)
            => NavigateTo(WorkArea.Home);

        private void BtnChangeProject_Click(object sender, RoutedEventArgs e)
        {
            var sel = new ProjectSelectionWindow();
            sel.Show();
            this.Close();
        }

        private void CboRound_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_suppressEvents) return;
            if (CboRound.SelectedItem is RoundTimelineItem r)
            {
                _currentRound = r;
                UpdateStatus();
                // TODO: 현재 작업 화면에 차수 변경 알림 (재로드)
            }
        }

        private void OpenProjectSettings()
        {
            try
            {
                var w = new Window
                {
                    Title = "테스트",
                    Width = 400,
                    Height = 300,
                    Content = new TextBlock { Text = "테스트 윈도우입니다" }
                };

                // Revit 메인 윈도우 핸들을 Owner로 직접 지정
                var helper = new System.Windows.Interop.WindowInteropHelper(w);
                helper.Owner = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;

                w.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CreateNewRound()
        {
            MessageBox.Show("새 차수 생성 다이얼로그 (구현 예정)",
                "BimOps", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // =========================================================
        // 유틸
        // =========================================================
        private void UpdateStatus()
        {
            string project = _currentProject?.Code ?? "프로젝트 없음";
            string round = _currentRound?.Name ?? "—";
            string area = _currentArea == WorkArea.Home ? "홈" : AreaMeta[_currentArea].Title;
            StatusText.Text = $"Status: {project} · {round} · {area}";
        }

        // =========================================================
        // 샘플 데이터 (실제 환경에서는 제거)
        // =========================================================
        private static ProjectCardItem GetSampleProject() => new ProjectCardItem
        {
            Code = "JJ-A1",
            Name = "제주 A1블록",
            BuildingCount = 24,
            UnitCount = 1248,
            UnitTypes = "84A/84B/110",
        };

        private static List<RoundTimelineItem> GetSampleRounds() => new List<RoundTimelineItem>
        {
            new RoundTimelineItem { Name = "1차",   Date = new DateTime(2026,4,12), Status = "FROZEN" },
            new RoundTimelineItem { Name = "변경1", Date = new DateTime(2026,4,28), Status = "FROZEN" },
            new RoundTimelineItem { Name = "2차",   Date = new DateTime(2026,5, 6), Status = "DRAFT", IsCurrent = true },
        };

        private class WorkAreaInfo
        {
            public string Title { get; }
            public bool ShowRound { get; }
            public WorkAreaInfo(string title, bool showRound) { Title = title; ShowRound = showRound; }
        }
    }
}