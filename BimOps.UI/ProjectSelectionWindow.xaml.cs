using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using BimOps.UI.Data;
using BimOps.UI.Views;

namespace BimOps.UI
{
    public partial class ProjectSelectionWindow : Window
    {


        private readonly ObservableCollection<ProjectCardItem> _allProjects = new ObservableCollection<ProjectCardItem>();
        private ICollectionView _view;
        private string _statusFilter = "All";

        /// <summary>
        /// 사용자가 선택한 프로젝트. 카드 클릭 후 MainWindow로 전달.
        /// </summary>
        public ProjectCardItem SelectedProject { get; private set; }

        public ProjectSelectionWindow()
        {
            InitializeComponent();
            LoadSampleData();   // 실제 환경에서는 DB·서비스에서 로드
            InitializeView();
            UpdateCount();

            // DB 생성
            AppState.EnsureDataRoot();
            Database.EnsureProjectsListDb(AppState.ProjectsListPath);

        }

        // ===== 데이터 초기화 =====
        private void LoadSampleData()
        {
            _allProjects.Add(new ProjectCardItem
            {
                Code = "JJ-A1", Name = "제주 A1블록",
                BuildingCount = 24, UnitCount = 1248,
                UnitTypes = "84A/84B/110",
                LatestRound = "2차", LatestStatus = "DRAFT",
                Status = ProjectStatus.InProgress,
                LastModified = new DateTime(2026, 5, 6, 14, 22, 0),
            });
            _allProjects.Add(new ProjectCardItem
            {
                Code = "SE-B2", Name = "서울 B2블록",
                BuildingCount = 18, UnitCount = 968,
                UnitTypes = "59/74/84",
                LatestRound = "변경1", LatestStatus = "FROZEN",
                Status = ProjectStatus.InProgress,
                LastModified = new DateTime(2026, 5, 4, 9, 15, 0),
            });
            _allProjects.Add(new ProjectCardItem
            {
                Code = "BS-C3", Name = "부산 C3블록",
                BuildingCount = 12, UnitCount = 624,
                UnitTypes = "84/110",
                LatestRound = "1차", LatestStatus = "DRAFT",
                Status = ProjectStatus.InProgress,
                LastModified = new DateTime(2026, 4, 30, 16, 48, 0),
            });
            _allProjects.Add(new ProjectCardItem
            {
                Code = "IC-D4", Name = "인천 D4블록",
                BuildingCount = 8, UnitCount = 412,
                UnitTypes = "59/84",
                LatestRound = "3차", LatestStatus = "FROZEN",
                Status = ProjectStatus.Completed,
                LastModified = new DateTime(2026, 2, 12, 11, 30, 0),
            });
        }

        private void InitializeView()
        {
            _view = CollectionViewSource.GetDefaultView(_allProjects);
            _view.Filter = FilterProject;
            ProjectList.ItemsSource = _view;
        }

        private bool FilterProject(object obj)
        {
            if (!(obj is ProjectCardItem p)) return false;

            // 상태 필터
            if (_statusFilter == "InProgress" && p.Status != ProjectStatus.InProgress) return false;
            if (_statusFilter == "Completed" && p.Status != ProjectStatus.Completed) return false;

            // 검색어 필터
            string keyword = TxtSearch?.Text?.Trim();
            if (!string.IsNullOrEmpty(keyword))
            {
                if (!(p.Code?.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0) &&
                    !(p.Name?.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;
            }
            return true;
        }

        // ===== 이벤트 핸들러 =====
        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtSearchPlaceholder != null)
                TxtSearchPlaceholder.Visibility = string.IsNullOrEmpty(TxtSearch.Text)
                    ? Visibility.Visible : Visibility.Collapsed;

            _view?.Refresh();
            UpdateCount();
        }

        private void FilterToggle_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is ToggleButton clicked)) return;

            // 라디오 그룹처럼 단일 선택 강제
            BtnFilterAll.IsChecked = clicked == BtnFilterAll;
            BtnFilterInProgress.IsChecked = clicked == BtnFilterInProgress;
            BtnFilterCompleted.IsChecked = clicked == BtnFilterCompleted;

            _statusFilter = clicked.Tag?.ToString() ?? "All";
            _view?.Refresh();
            UpdateCount();
        }

        private void ProjectCard_Click(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is FrameworkElement el)) return;
            if (!(el.Tag is ProjectCardItem project)) return;

            SelectedProject = project;

            // 컨텍스트 주입 후 MainWindow 진입
            AppState.SelectedProject = project;
            AppState.AvailableProjects = _allProjects;

            Database.EnsureProjectDb(AppState.CurrentProjectDbPath);

            var main = new MainWindow();
            main.Show();
            this.Close();
        }

        private void BtnNewProject_Click(object sender, RoutedEventArgs e)
        {
            // TODO: 새 프로젝트 생성 다이얼로그 호출
            MessageBox.Show("새 프로젝트 등록 다이얼로그 (구현 예정)",
                "BimOps", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void UpdateCount()
        {
            int total = _allProjects.Count;
            int visible = _view?.Cast<object>().Count() ?? total;
            CountText.Text = $"표시 {visible} / 전체 {total}";

            EmptyMessage.Visibility = visible == 0 ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    // ===== 모델 =====
    public enum ProjectStatus { InProgress, Completed }

    public class ProjectCardItem
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int BuildingCount { get; set; }
        public int UnitCount { get; set; }
        public string UnitTypes { get; set; }
        public string LatestRound { get; set; }
        public string LatestStatus { get; set; }    // "DRAFT" / "FROZEN"
        public ProjectStatus Status { get; set; }
        public DateTime LastModified { get; set; }

        // 카드 표시용 가공 프로퍼티
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