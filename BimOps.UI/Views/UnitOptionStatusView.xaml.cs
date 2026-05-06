using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using BimOps.UI.Models;

namespace BimOps.UI.Views
{
    public partial class UnitOptionStatusView : UserControl
    {
        public ObservableCollection<UnitOptionRow> Rows { get; }
            = new ObservableCollection<UnitOptionRow>();
        private List<OptionItem> _options = new List<OptionItem>();

        public event EventHandler GoToCalculation;

        public UnitOptionStatusView()
        {
            InitializeComponent();
            LoadSampleData();
            BuildColumns();
            MatrixGrid.ItemsSource = Rows;
            UpdateStats();
            TxtSubtitle.Text = "JJ-A1 / 제주 A1블록 · 차수: 2차 (DRAFT)";
        }

        // =========================================================
        // 컬럼 동적 생성
        // =========================================================
        private void BuildColumns()
        {
            MatrixGrid.Columns.Clear();

            // 고정 컬럼
            MatrixGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "동·호",
                Binding = new Binding("UnitLabel"),
                Width = 90,
                IsReadOnly = true,
            });
            MatrixGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "평형",
                Binding = new Binding("UnitTypeCode"),
                Width = 70,
                IsReadOnly = true,
            });

            // 옵션별 동적 컬럼
            foreach (var op in _options)
            {
                MatrixGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = op.Name,
                    Binding = new Binding($"OptionCells[{op.Code}].DisplayText"),
                    Width = 100,
                    ElementStyle = BuildCenteredCellStyle(),
                });
            }

            // 검증 메시지 컬럼
            MatrixGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "검증",
                Binding = new Binding("IssueMessage"),
                Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                IsReadOnly = true,
                ElementStyle = BuildIssueCellStyle(),
            });
        }

        private Style BuildCenteredCellStyle()
        {
            var style = new Style(typeof(TextBlock));
            style.Setters.Add(new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center));
            return style;
        }

        private Style BuildIssueCellStyle()
        {
            var style = new Style(typeof(TextBlock));
            style.Setters.Add(new Setter(TextBlock.ForegroundProperty,
                (System.Windows.Media.Brush)FindResource("BadgeDangerFgBrush")));
            style.Setters.Add(new Setter(TextBlock.FontSizeProperty, 11.0));
            return style;
        }

        // =========================================================
        // 데이터
        // =========================================================
        public void LoadData(IEnumerable<OptionItem> options, IEnumerable<UnitOptionRow> rows)
        {
            _options = options?.ToList() ?? new List<OptionItem>();
            Rows.Clear();
            foreach (var r in rows ?? Enumerable.Empty<UnitOptionRow>())
                Rows.Add(r);
            BuildColumns();
            UpdateStats();
        }

        private void LoadSampleData()
        {
            _options = new List<OptionItem>
            {
                new OptionItem { Code = "BU01", Name = "붙박이장" },
                new OptionItem { Code = "KF01", Name = "냉장고장" },
                new OptionItem { Code = "MD01", Name = "중문" },
                new OptionItem { Code = "EX01", Name = "베란다 확장" },
            };

            Rows.Add(MakeRow("101", "1001", "84A", new[] {
                ("BU01", "침실2"), ("KF01", "✓"), ("EX01", "✓")
            }));
            Rows.Add(MakeRow("101", "1002", "84A", new[] {
                ("BU01", "침실3"), ("MD01", "✓")
            }));

            // 검증 이슈 예시
            var issue = MakeRow("101", "1003", "84B", new[] {
                ("BU01", null), ("KF01", "✓"), ("MD01", "✓"), ("EX01", "✓")
            });
            issue.HasIssue = true;
            issue.IssueMessage = "붙박이장 설치위치 누락";
            Rows.Add(issue);

            Rows.Add(MakeRow("101", "1004", "84A", new[] {
                ("BU01", "침실2"), ("EX01", "✓")
            }));
            Rows.Add(MakeRow("102", "0903", "84A", new[] {
                ("BU01", "침실3"), ("KF01", "✓"), ("MD01", "✓"), ("EX01", "✓")
            }));
            Rows.Add(MakeRow("102", "1001", "84A", Array.Empty<(string, string)>()));
        }

        private UnitOptionRow MakeRow(string building, string unitNo, string unitType,
                                      IEnumerable<(string code, string room)> selections)
        {
            var row = new UnitOptionRow
            {
                Building = building,
                UnitNo = unitNo,
                UnitTypeCode = unitType,
            };
            foreach (var op in _options)
                row.OptionCells[op.Code] = new OptionCell { Selected = false };
            foreach (var (code, room) in selections)
            {
                if (row.OptionCells.TryGetValue(code, out var cell))
                {
                    cell.Selected = true;
                    cell.InstallRoom = room == "✓" ? null : room;
                }
            }
            return row;
        }

        // =========================================================
        // 통계 / 검증
        // =========================================================
        private void UpdateStats()
        {
            int total = Rows.Count;
            int issue = Rows.Count(r => r.HasIssue);
            TxtTotal.Text = total.ToString("N0");
            TxtValid.Text = (total - issue).ToString("N0");
            TxtIssue.Text = issue.ToString("N0");
            TxtLastImport.Text = DateTime.Now.ToString("MM-dd HH:mm");
        }

        // =========================================================
        // 이벤트
        // =========================================================
        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Excel 임포트 다이얼로그 (구현 예정)", "BimOps");
        }

        private void BtnValidate_Click(object sender, RoutedEventArgs e)
        {
            // TODO: 옵션 코드 매칭, 평형 매칭, 충돌 검증 등
            UpdateStats();
            StatusUpdate("검증 완료");
        }

        private void BtnGoCalc_Click(object sender, RoutedEventArgs e)
        {
            int issue = Rows.Count(r => r.HasIssue);
            if (issue > 0)
            {
                var ans = MessageBox.Show(
                    $"검증 이슈 {issue}건이 남아있습니다. 그래도 산출 화면으로 이동할까요?",
                    "BimOps", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (ans != MessageBoxResult.OK) return;
            }
            GoToCalculation?.Invoke(this, EventArgs.Empty);
        }

        private void MatrixGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 향후: 선택된 행의 상세 패널 갱신
        }

        private void StatusUpdate(string msg)
        {
            // 부모 윈도우에 상태 전달이 필요하면 이벤트로 노출
        }
    }
}