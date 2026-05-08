using BimOps.UI.Models;
using BimOps.UI.Data;
using BimOps.UI.Data.Repositories;
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

namespace BimOps.UI
{
    public partial class ProjectSelectionWindow : Window
    {
        private readonly ObservableCollection<ProjectCardItem> _allProjects
            = new ObservableCollection<ProjectCardItem>();
        private ICollectionView _view;
        private string _statusFilter = "All";
        private ProjectListRepository _repo;

        /// <summary>사용자가 선택한 프로젝트. 카드 클릭 후 MainWindow로 전달.</summary>
        public ProjectCardItem SelectedProject { get; private set; }

        public ProjectSelectionWindow()
        {
            InitializeComponent();

            // DB 초기화는 데이터 로드보다 먼저
            AppState.EnsureDataRoot();
            Database.EnsureProjectsListDb(AppState.ProjectsListPath);
            _repo = new ProjectListRepository(AppState.ProjectsListConnectionString);

            LoadProjects();
            InitializeView();
            UpdateCount();
        }

        // ===== 데이터 로드 =====

        private void LoadProjects()
        {
            var fromDb = _repo.GetAll();

            // 첫 실행 시 DB가 비어있으면 샘플 4개 시딩
            if (fromDb.Count == 0)
            {
                SeedSampleProjects();
                fromDb = _repo.GetAll();
            }

            _allProjects.Clear();
            foreach (var p in fromDb)
                _allProjects.Add(p);
        }

        private void SeedSampleProjects()
        {
            _repo.Insert(new ProjectCardItem
            {
                Code = "JJ-A1",
                Name = "제주 A1블록",
                BuildingCount = 24,
                UnitCount = 1248,
                UnitTypes = "84A/84B/110",
                LatestRound = "2차",
                LatestStatus = "DRAFT",
                Status = ProjectStatus.InProgress,
                LastModified = new DateTime(2026, 5, 6, 14, 22, 0),
            });
            _repo.Insert(new ProjectCardItem
            {
                Code = "SE-B2",
                Name = "서울 B2블록",
                BuildingCount = 18,
                UnitCount = 968,
                UnitTypes = "59/74/84",
                LatestRound = "변경1",
                LatestStatus = "FROZEN",
                Status = ProjectStatus.InProgress,
                LastModified = new DateTime(2026, 5, 4, 9, 15, 0),
            });
            _repo.Insert(new ProjectCardItem
            {
                Code = "BS-C3",
                Name = "부산 C3블록",
                BuildingCount = 12,
                UnitCount = 624,
                UnitTypes = "84/110",
                LatestRound = "1차",
                LatestStatus = "DRAFT",
                Status = ProjectStatus.InProgress,
                LastModified = new DateTime(2026, 4, 30, 16, 48, 0),
            });
            _repo.Insert(new ProjectCardItem
            {
                Code = "IC-D4",
                Name = "인천 D4블록",
                BuildingCount = 8,
                UnitCount = 412,
                UnitTypes = "59/84",
                LatestRound = "3차",
                LatestStatus = "FROZEN",
                Status = ProjectStatus.Completed,
                LastModified = new DateTime(2026, 2, 12, 11, 30, 0),
            });
        }

        // ===== 뷰 / 필터 =====

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
            AppState.SelectedProject = project;
            AppState.AvailableProjects = _allProjects;

            Database.EnsureProjectDb(AppState.CurrentProjectDbPath);

            // 마지막 접근 시각 갱신 (DB)
            _repo.TouchLastModified(project.Code);

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

        // ===== 보조 =====

        private void UpdateCount()
        {
            int total = _allProjects.Count;
            int visible = _view?.Cast<object>().Count() ?? total;
            CountText.Text = $"표시 {visible} / 전체 {total}";

            EmptyMessage.Visibility = visible == 0 ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}