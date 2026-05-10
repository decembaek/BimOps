using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using BimOps.UI.Data;
using BimOps.UI.Data.Repositories;
using BimOps.UI.Models;

namespace BimOps.UI.Views
{
    public partial class ProjectSettingsView : UserControl
    {
        public ObservableCollection<FinishCategory> FinishCategories { get; }
            = new ObservableCollection<FinishCategory>();

        private readonly FinishCategoryRepository _finishRepo;
        private DataGrid _grid;

        // 부모(MainWindow)에게 화면 이탈 요청
        public event EventHandler RequestExit;

        public ProjectSettingsView()
        {
            InitializeComponent();

            _finishRepo = new FinishCategoryRepository(AppState.CurrentConnectionString);
            LoadFinishCategories();

            TxtSubtitle.Text = AppState.SelectedProject != null
                ? $"{AppState.SelectedProject.Code} / {AppState.SelectedProject.Name}"
                : "";

            ShowFinishCategoryPanel();
        }

        private void LoadFinishCategories()
        {
            FinishCategories.Clear();
            foreach (var fc in _finishRepo.GetAll())
                FinishCategories.Add(fc);
        }

        // ===== 사이드바 =====

        private void NavMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(NavMenu.SelectedItem is ListBoxItem item)) return;
            switch (item.Tag as string)
            {
                case "FinishCategory": ShowFinishCategoryPanel(); break;
                case "ProjectInfo": ShowPlaceholder("단지 정보 (구현 예정)"); break;
                case "Permissions": ShowPlaceholder("사용자 권한 (Phase 3)"); break;
            }
        }

        private void ShowFinishCategoryPanel()
        {
            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            // 제목
            grid.Children.Add(new TextBlock
            {
                Text = "마감재 카테고리",
                FontSize = 16,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 4),
            });

            // 설명
            var desc = new TextBlock
            {
                Text = "이 단지에서 관리할 마감재 종류를 정의합니다. 기준 데이터·산출 결과 등 모든 화면에서 이 목록을 참조합니다.",
                FontSize = 12,
                Foreground = TryFindResource("TextTertiaryBrush") as System.Windows.Media.Brush
                             ?? System.Windows.Media.Brushes.Gray,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 12),
            };
            Grid.SetRow(desc, 1);
            grid.Children.Add(desc);

            // 버튼 + DataGrid
            var contentGrid = new Grid();
            contentGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            contentGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            var smallBtnStyle = TryFindResource("SmallButtonStyle") as Style;

            var btnPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 8) };
            var btnAdd = new Button { Content = "+ 카테고리 추가", Style = smallBtnStyle };
            btnAdd.Click += (s, e) => FinishCategories.Add(new FinishCategory { Code = "신규", Uom = "㎡" });
            var btnDel = new Button { Content = "선택 행 삭제", Style = smallBtnStyle };
            btnDel.Click += (s, e) =>
            {
                if (_grid?.SelectedItem is FinishCategory fc) FinishCategories.Remove(fc);
            };
            btnPanel.Children.Add(btnAdd);
            btnPanel.Children.Add(btnDel);
            contentGrid.Children.Add(btnPanel);

            _grid = new DataGrid { ItemsSource = FinishCategories };
            _grid.Columns.Add(new DataGridTextColumn { Header = "코드", Binding = new System.Windows.Data.Binding("Code"), Width = 120 });
            _grid.Columns.Add(new DataGridTextColumn { Header = "이름", Binding = new System.Windows.Data.Binding("Name"), Width = 160 });
            _grid.Columns.Add(new DataGridTextColumn { Header = "단위", Binding = new System.Windows.Data.Binding("Uom"), Width = 80 });
            _grid.Columns.Add(new DataGridTextColumn { Header = "비고", Binding = new System.Windows.Data.Binding("Remark"), Width = new DataGridLength(1, DataGridLengthUnitType.Star) });
            Grid.SetRow(_grid, 1);
            contentGrid.Children.Add(_grid);

            Grid.SetRow(contentGrid, 2);
            grid.Children.Add(contentGrid);

            ContentArea.Content = grid;
        }

        private void ShowPlaceholder(string text)
        {
            ContentArea.Content = new TextBlock
            {
                Text = text,
                FontSize = 14,
                Foreground = System.Windows.Media.Brushes.Gray,
            };
        }

        // ===== 저장 / 이탈 =====

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            _finishRepo.ReplaceAll(FinishCategories);

            // 저장 후 홈으로 자동 복귀
            RequestExit?.Invoke(this, EventArgs.Empty);
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            // 저장 안 하고 그냥 복귀
            RequestExit?.Invoke(this, EventArgs.Empty);
        }
    }
}