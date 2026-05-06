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
    public partial class QuantityResultView : UserControl
    {
        public ObservableCollection<BuildingSummary> Buildings { get; }
            = new ObservableCollection<BuildingSummary>();
        public ObservableCollection<UnitDetailRow> Units { get; }
            = new ObservableCollection<UnitDetailRow>();
        public ObservableCollection<OptionStatRow> OptionStats { get; }
            = new ObservableCollection<OptionStatRow>();

        // 마감재 카테고리 목록 (실제로는 ProjectSettings에서 가져옴)
        private List<FinishCategory> _finishes = new List<FinishCategory>();

        public event EventHandler GoToHistory;
        public event EventHandler RecalcRequested;

        public QuantityResultView()
        {
            InitializeComponent();
            LoadSampleData();
            BuildAllColumns();
            BindData();
            UpdateStats();
            BuildBuildingFilter();
            TxtSubtitle.Text = "JJ-A1 / 제주 A1블록 · 차수: 2차 (DRAFT)";
        }

        // =========================================================
        // 컬럼 동적 생성 (3개 탭 모두)
        // =========================================================
        private void BuildAllColumns()
        {
            BuildBuildingColumns();
            BuildUnitColumns();
            BuildOptionStatColumns();
        }

        private void BuildBuildingColumns()
        {
            BuildingGrid.Columns.Clear();
            BuildingGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "동",
                Binding = new Binding("Building"),
                Width = 100,
                IsReadOnly = true,
            });
            foreach (var f in _finishes)
            {
                BuildingGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = $"{f.Name} ({f.Uom})",
                    Binding = new Binding($"FinishQuantities[{f.Code}]")
                    {
                        StringFormat = "N1",
                    },
                    Width = 120,
                    ElementStyle = NumericRightStyle(),
                    IsReadOnly = true,
                });
            }
        }

        private void BuildUnitColumns()
        {
            UnitGrid.Columns.Clear();
            UnitGrid.Columns.Add(new DataGridTextColumn { Header = "동·호", Binding = new Binding("UnitLabel"), Width = 90, IsReadOnly = true });
            UnitGrid.Columns.Add(new DataGridTextColumn { Header = "평형", Binding = new Binding("UnitTypeCode"), Width = 60, IsReadOnly = true });
            UnitGrid.Columns.Add(new DataGridTextColumn { Header = "선택 옵션", Binding = new Binding("SelectedOptions"), Width = 280, IsReadOnly = true });
            foreach (var f in _finishes)
            {
                UnitGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = $"{f.Name} ({f.Uom})",
                    Binding = new Binding($"FinishQuantities[{f.Code}]") { StringFormat = "N1" },
                    Width = 90,
                    ElementStyle = NumericRightStyle(),
                    IsReadOnly = true,
                });
            }
        }

        private void BuildOptionStatColumns()
        {
            OptionStatGrid.Columns.Clear();
            OptionStatGrid.Columns.Add(new DataGridTextColumn { Header = "옵션", Binding = new Binding("OptionName"), Width = 140, IsReadOnly = true });
            OptionStatGrid.Columns.Add(new DataGridTextColumn { Header = "적용 세대", Binding = new Binding("AppliedUnits") { StringFormat = "N0" }, Width = 100, ElementStyle = NumericRightStyle(), IsReadOnly = true });
            foreach (var f in _finishes)
            {
                OptionStatGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = $"{f.Name} 차감 ({f.Uom})",
                    Binding = new Binding($"FinishDeltas[{f.Code}]") { StringFormat = "N1" },
                    Width = 130,
                    ElementStyle = NumericRightStyle(),
                    IsReadOnly = true,
                });
            }
        }

        private Style NumericRightStyle()
        {
            var style = new Style(typeof(TextBlock));
            style.Setters.Add(new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Right));
            style.Setters.Add(new Setter(TextBlock.FontFamilyProperty,
                new System.Windows.Media.FontFamily("Consolas")));
            return style;
        }

        // =========================================================
        // 데이터 바인딩 / 외부 주입
        // =========================================================
        private void BindData()
        {
            BuildingGrid.ItemsSource = Buildings;
            UnitGrid.ItemsSource = Units;
            OptionStatGrid.ItemsSource = OptionStats;
            TxtUnitCount.Text = Units.Count.ToString("N0");
        }

        public void LoadData(IEnumerable<FinishCategory> finishes,
                             IEnumerable<BuildingSummary> buildings,
                             IEnumerable<UnitDetailRow> units,
                             IEnumerable<OptionStatRow> optionStats)
        {
            _finishes = finishes?.ToList() ?? new List<FinishCategory>();
            Buildings.Clear(); foreach (var b in buildings ?? Enumerable.Empty<BuildingSummary>()) Buildings.Add(b);
            Units.Clear(); foreach (var u in units ?? Enumerable.Empty<UnitDetailRow>()) Units.Add(u);
            OptionStats.Clear(); foreach (var s in optionStats ?? Enumerable.Empty<OptionStatRow>()) OptionStats.Add(s);

            BuildAllColumns();
            BindData();
            UpdateStats();
            BuildBuildingFilter();
        }

        // =========================================================
        // 샘플 데이터
        // =========================================================
        private void LoadSampleData()
        {
            _finishes = new List<FinishCategory>
            {
                new FinishCategory { Code = "WALL",  Name = "벽지",     Uom = "㎡" },
                new FinishCategory { Code = "FLOOR", Name = "마루",     Uom = "㎡" },
                new FinishCategory { Code = "CEIL",  Name = "천장지",   Uom = "㎡" },
                new FinishCategory { Code = "BASE",  Name = "걸레받이", Uom = "m"  },
                new FinishCategory { Code = "TILE",  Name = "타일",     Uom = "㎡" },
            };

            // 동별 집계
            Buildings.Add(MakeBuilding("101동", 1823.4, 923.6, 880.2, 2560.0, 580.4));
            Buildings.Add(MakeBuilding("102동", 1910.8, 952.4, 906.5, 2640.2, 602.8));
            Buildings.Add(MakeBuilding("103동", 1889.6, 942.8, 897.7, 2604.8, 595.2));
            Buildings.Add(MakeBuilding("104동", 1795.2, 901.3, 863.4, 2520.6, 573.0));
            var total = MakeBuilding("합계", 11623.4, 6056.2, 5688.0, 18180.4, 3214.6);
            total.IsTotal = true;
            Buildings.Add(total);

            // 세대별 명세 (일부)
            Units.Add(MakeUnit("101", "1001", "84A", "붙박이장(침실2) · 냉장고장 · 베란다확장", 75.6, 39.4, 37.8, 108.2, 28.6));
            Units.Add(MakeUnit("101", "1002", "84A", "붙박이장(침실3) · 중문", 79.4, 41.0, 39.4, 112.4, 28.6));
            Units.Add(MakeUnit("101", "1003", "84B", "냉장고장 · 중문 · 베란다확장", 86.2, 45.8, 43.6, 122.8, 30.4));
            Units.Add(MakeUnit("101", "1004", "84A", "붙박이장(침실2) · 베란다확장", 77.8, 40.6, 38.8, 110.4, 28.6));
            Units.Add(MakeUnit("101", "1005", "84A", "옵션 선택 없음", 82.4, 42.6, 40.6, 116.0, 28.6));

            // 옵션별 통계
            OptionStats.Add(MakeOptionStat("BU01", "붙박이장", 872, 6278.4, 1569.6, 1569.6, 4708.8, 0.0));
            OptionStats.Add(MakeOptionStat("KF01", "냉장고장", 643, 385.8, 128.6, 128.6, 385.8, 0.0));
            OptionStats.Add(MakeOptionStat("MD01", "중문", 512, 0.0, 0.0, 0.0, 0.0, 0.0));
            OptionStats.Add(MakeOptionStat("EX01", "베란다 확장", 1124, 3597.0, 9554.0, 3597.0, 0.0, 0.0));
        }

        private BuildingSummary MakeBuilding(string b, double w, double f, double c, double bs, double t)
        {
            return new BuildingSummary
            {
                Building = b,
                FinishQuantities = new Dictionary<string, double>
                {
                    ["WALL"] = w,
                    ["FLOOR"] = f,
                    ["CEIL"] = c,
                    ["BASE"] = bs,
                    ["TILE"] = t
                }
            };
        }

        private UnitDetailRow MakeUnit(string b, string u, string ut, string opts,
                                       double w, double f, double c, double bs, double t)
        {
            return new UnitDetailRow
            {
                Building = b,
                UnitNo = u,
                UnitTypeCode = ut,
                SelectedOptions = opts,
                FinishQuantities = new Dictionary<string, double>
                {
                    ["WALL"] = w,
                    ["FLOOR"] = f,
                    ["CEIL"] = c,
                    ["BASE"] = bs,
                    ["TILE"] = t
                }
            };
        }

        private OptionStatRow MakeOptionStat(string code, string name, int units,
                                             double w, double f, double c, double bs, double t)
        {
            return new OptionStatRow
            {
                OptionCode = code,
                OptionName = name,
                AppliedUnits = units,
                FinishDeltas = new Dictionary<string, double>
                {
                    ["WALL"] = w,
                    ["FLOOR"] = f,
                    ["CEIL"] = c,
                    ["BASE"] = bs,
                    ["TILE"] = t
                }
            };
        }

        // =========================================================
        // 통계 / 필터
        // =========================================================
        private void UpdateStats()
        {
            int totalUnits = Units.Count;
            int applied = Units.Count(u => !string.IsNullOrEmpty(u.SelectedOptions)
                                          && u.SelectedOptions != "옵션 선택 없음");
            int selectionCount = OptionStats.Sum(s => s.AppliedUnits);

            TxtTotalUnits.Text = totalUnits.ToString("N0");
            TxtAppliedUnits.Text = applied.ToString("N0");
            TxtSelectionCount.Text = selectionCount.ToString("N0");
            TxtCalcTime.Text = DateTime.Now.ToString("MM-dd HH:mm");
        }

        private void BuildBuildingFilter()
        {
            CboBuildingFilter.Items.Clear();
            CboBuildingFilter.Items.Add("전체 동");
            foreach (var b in Buildings.Where(x => !x.IsTotal).Select(x => x.Building))
                CboBuildingFilter.Items.Add(b);
            CboBuildingFilter.SelectedIndex = 0;
        }

        private void CboBuildingFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string sel = CboBuildingFilter.SelectedItem as string;
            if (sel == null || sel == "전체 동")
                UnitGrid.ItemsSource = Units;
            else
                UnitGrid.ItemsSource = Units.Where(u => $"{u.Building}동" == sel).ToList();

            TxtUnitCount.Text = (UnitGrid.ItemsSource as IEnumerable<UnitDetailRow>)?.Count().ToString("N0")
                                ?? Units.Count.ToString("N0");
        }

        // =========================================================
        // 액션
        // =========================================================
        private void BtnRecalc_Click(object sender, RoutedEventArgs e)
            => RecalcRequested?.Invoke(this, EventArgs.Empty);

        private void BtnGoHistory_Click(object sender, RoutedEventArgs e)
            => GoToHistory?.Invoke(this, EventArgs.Empty);
    }
}