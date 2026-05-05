using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace BimOps.UI.Views
{
    public partial class ResultView : UserControl
    {
        public class ResultRow
        {
            public string OptionName { get; set; }
            public string ItemName { get; set; }
            public string PreviousQty { get; set; }
            public string CurrentQty { get; set; }
            public string Delta { get; set; }
        }

        public ObservableCollection<ResultRow> Rows { get; }
            = new ObservableCollection<ResultRow>();

        public ResultView()
        {
            InitializeComponent();
            GridResult.ItemsSource = Rows;

            // 더미 데이터 (확인용)
            Rows.Add(new ResultRow { OptionName = "침실2 붙박이장", ItemName = "후면 벽지", PreviousQty = "-384.0", CurrentQty = "-432.0", Delta = "-48.0" });
            Rows.Add(new ResultRow { OptionName = "침실2 붙박이장", ItemName = "붙박이장 본체", PreviousQty = "120", CurrentQty = "135", Delta = "+15" });
            Rows.Add(new ResultRow { OptionName = "현관 중문", ItemName = "중문 본체", PreviousQty = "0", CurrentQty = "20", Delta = "+20" });
            TxtRowCount.Text = $"총 {Rows.Count}건";

            // 두 번째 탭(이전 대비 변경분)을 기본 선택
            ResultTabs.SelectedIndex = 1;
        }

        private void ResultTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded) return;

            var tab = ResultTabs.SelectedItem as TabItem;
            if (tab == null) return;

            TxtCurrentTabName.Text = tab.Header?.ToString();
            // TODO: 탭별 데이터 로드 + 그리드 컬럼 변경
        }

        // ===== 출력 =====
        private void BtnExportExcel_Click(object sender, RoutedEventArgs e) { }
        private void BtnExportCsv_Click(object sender, RoutedEventArgs e) { }
        private void BtnSaveJson_Click(object sender, RoutedEventArgs e) { }
        private void BtnMarkAsDelivered_Click(object sender, RoutedEventArgs e) { }
    }
}