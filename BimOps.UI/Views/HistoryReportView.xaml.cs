using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BimOps.UI.Models;

namespace BimOps.UI.Views
{
    public partial class HistoryReportView : UserControl
    {
        public ObservableCollection<RoundHistoryItem> HistoryItems { get; }
            = new ObservableCollection<RoundHistoryItem>();
        public ObservableCollection<DiffChangeRow> DiffRows { get; }
            = new ObservableCollection<DiffChangeRow>();
        public ObservableCollection<FinishSummary> DeltaSummaryItems { get; }
            = new ObservableCollection<FinishSummary>();

        public event EventHandler<RoundHistoryItem> RoundOpenRequested;

        public HistoryReportView()
        {
            InitializeComponent();
            LoadSampleData();
            BindAll();
            InitDiffSelectors();
            InitExportTab();
            TxtSubtitle.Text = "JJ-A1 / 제주 A1블록";
        }

        // =========================================================
        // 데이터 / 바인딩
        // =========================================================
        private void BindAll()
        {
            HistoryGrid.ItemsSource = HistoryItems;
            DiffGrid.ItemsSource = DiffRows;
            DeltaSummary.ItemsSource = DeltaSummaryItems;
            TxtHistoryCount.Text = HistoryItems.Count.ToString();
        }

        private void LoadSampleData()
        {
            HistoryItems.Add(new RoundHistoryItem
            {
                RoundName = "1차",
                Status = "FROZEN",
                CalcAt = new DateTime(2026, 4, 12, 14, 22, 0),
                UnitCount = 1248,
                AppliedCount = 1037,
                Operator = "BIM팀 박OO",
                ExcelExported = true,
            });
            HistoryItems.Add(new RoundHistoryItem
            {
                RoundName = "변경1",
                Status = "FROZEN",
                CalcAt = new DateTime(2026, 4, 28, 9, 15, 0),
                UnitCount = 1248,
                AppliedCount = 1062,
                Operator = "BIM팀 박OO",
                ExcelExported = true,
            });
            HistoryItems.Add(new RoundHistoryItem
            {
                RoundName = "2차",
                Status = "DRAFT",
                CalcAt = new DateTime(2026, 5, 6, 14, 22, 0),
                UnitCount = 1248,
                AppliedCount = 1089,
                Operator = "BIM팀 박OO",
                ExcelExported = false,
            });

            // Diff 샘플 (변경1 → 2차)
            DiffRows.Add(new DiffChangeRow
            {
                Building = "101",
                UnitNo = "1001",
                ChangeType = DiffChangeType.Relocated,
                OptionName = "붙박이장",
                ChangeDescription = "침실2 → 침실3",
                FinishImpact = "벽지 +1.8 / 마루 +0.4"
            });
            DiffRows.Add(new DiffChangeRow
            {
                Building = "101",
                UnitNo = "1052",
                ChangeType = DiffChangeType.Added,
                OptionName = "중문",
                ChangeDescription = "— → 설치",
                FinishImpact = "영향 없음"
            });
            DiffRows.Add(new DiffChangeRow
            {
                Building = "101",
                UnitNo = "1108",
                ChangeType = DiffChangeType.Removed,
                OptionName = "베란다 확장",
                ChangeDescription = "설치 → —",
                FinishImpact = "벽지 −3.2 / 마루 −8.5"
            });
            DiffRows.Add(new DiffChangeRow
            {
                Building = "102",
                UnitNo = "0903",
                ChangeType = DiffChangeType.Added,
                OptionName = "붙박이장",
                ChangeDescription = "— → 침실2",
                FinishImpact = "벽지 −7.2 / 마루 −1.8"
            });
            DiffRows.Add(new DiffChangeRow
            {
                Building = "103",
                UnitNo = "1204",
                ChangeType = DiffChangeType.Relocated,
                OptionName = "냉장고장",
                ChangeDescription = "주방A → 주방B",
                FinishImpact = "벽지 −0.6 / 마루 −0.2"
            });

            // 마감재별 누적 증감
            DeltaSummaryItems.Add(new FinishSummary { FinishCode = "WALL", FinishName = "벽지", TotalQty = -218.4, Uom = "㎡" });
            DeltaSummaryItems.Add(new FinishSummary { FinishCode = "FLOOR", FinishName = "마루", TotalQty = -72.6, Uom = "㎡" });
            DeltaSummaryItems.Add(new FinishSummary { FinishCode = "CEIL", FinishName = "천장지", TotalQty = -54.0, Uom = "㎡" });
            DeltaSummaryItems.Add(new FinishSummary { FinishCode = "BASE", FinishName = "걸레받이", TotalQty = -146.2, Uom = "m" });
            DeltaSummaryItems.Add(new FinishSummary { FinishCode = "TILE", FinishName = "타일", TotalQty = 0, Uom = "㎡" });

            // Diff 통계
            TxtChangedUnits.Text = "87";
            TxtAdded.Text = "+32";
            TxtRemoved.Text = "−14";
            TxtRelocated.Text = "41";
        }

        // =========================================================
        // Diff 탭
        // =========================================================
        private void InitDiffSelectors()
        {
            CboFrom.ItemsSource = HistoryItems.Select(h => h.RoundName).ToList();
            CboTo.ItemsSource = HistoryItems.Select(h => h.RoundName).ToList();
            if (HistoryItems.Count >= 2)
            {
                CboFrom.SelectedIndex = HistoryItems.Count - 2;
                CboTo.SelectedIndex = HistoryItems.Count - 1;
            }
        }

        private void DiffSelection_Changed(object sender, SelectionChangedEventArgs e)
        {
            // TODO: 실제 from/to 비교 로직 호출
            // 현재는 샘플 데이터만 표시
        }

        private void BtnExportNotice_Click(object sender, RoutedEventArgs e)
        {
            string from = CboFrom.SelectedItem as string;
            string to = CboTo.SelectedItem as string;
            MessageBox.Show($"{from} → {to} 변경 통보서를 출력합니다. (구현 예정)",
                "BimOps", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // =========================================================
        // 산출 이력 탭
        // =========================================================
        private void HistoryGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (HistoryGrid.SelectedItem is RoundHistoryItem item)
                RoundOpenRequested?.Invoke(this, item);
        }

        // =========================================================
        // Excel 출력 탭
        // =========================================================
        private void InitExportTab()
        {
            CboExportRound.ItemsSource = HistoryItems.Select(h => h.RoundName).ToList();
            CboExportRound.SelectedIndex = HistoryItems.Count - 1;
            TxtSavePath.Text = $@"C:\BimOps\Export\JJ-A1_{DateTime.Now:yyyyMMdd}.xlsx";
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = System.IO.Path.GetFileName(TxtSavePath.Text),
                InitialDirectory = System.IO.Path.GetDirectoryName(TxtSavePath.Text),
                Filter = "Excel 파일 (*.xlsx)|*.xlsx",
                DefaultExt = ".xlsx",
            };
            if (dlg.ShowDialog() == true)
                TxtSavePath.Text = dlg.FileName;
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            var sheets = new List<string>();
            if (ChkResult.IsChecked == true) sheets.Add("산출 결과");
            if (ChkNotice.IsChecked == true) sheets.Add("변경 통보서");
            if (ChkMaster.IsChecked == true) sheets.Add("누적 마감재 마스터");

            if (sheets.Count == 0)
            {
                MessageBox.Show("출력 항목을 하나 이상 선택하세요.", "BimOps");
                return;
            }
            if (string.IsNullOrWhiteSpace(TxtSavePath.Text))
            {
                MessageBox.Show("저장 경로를 지정하세요.", "BimOps");
                return;
            }

            // TODO: 실제 Excel 출력 (NPOI/EPPlus/ClosedXML 등)
            MessageBox.Show(
                $"차수 {CboExportRound.SelectedItem}\n출력 항목: {string.Join(", ", sheets)}\n경로: {TxtSavePath.Text}",
                "BimOps - 출력 (구현 예정)", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}