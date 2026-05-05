using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace BimOps.UI.Views
{
    public partial class VerificationView : UserControl
    {
        public class ErrorRow
        {
            public int No { get; set; }
            public string ErrorType { get; set; }
            public string Building { get; set; }
            public string UnitNo { get; set; }
            public string Message { get; set; }
        }

        public class ChangeRow
        {
            public string Building { get; set; }
            public string UnitNo { get; set; }
            public string ChangeType { get; set; }
            public string Before { get; set; }
            public string After { get; set; }
        }

        public ObservableCollection<ErrorRow> Errors { get; }
            = new ObservableCollection<ErrorRow>();

        public ObservableCollection<ChangeRow> Changes { get; }
            = new ObservableCollection<ChangeRow>();

        public VerificationView()
        {
            InitializeComponent();

            GridErrors.ItemsSource = Errors;
            GridChanges.ItemsSource = Changes;

            // 더미 데이터 (확인용 - 실제 구현 시 제거)
            Errors.Add(new ErrorRow { No = 1, ErrorType = "룰 누락", Building = "102", UnitNo = "0803", Message = "냉장고장 옵션 물량룰 없음" });
            Errors.Add(new ErrorRow { No = 2, ErrorType = "공간 매칭 실패", Building = "101", UnitNo = "1001", Message = "침실3 RoomCode 매칭 실패" });
            TxtErrorCount.Text = $"총 {Errors.Count}건";

            Changes.Add(new ChangeRow { Building = "101", UnitNo = "1001", ChangeType = "위치변경", Before = "침실2 붙박이장", After = "침실3 붙박이장" });
            Changes.Add(new ChangeRow { Building = "101", UnitNo = "1002", ChangeType = "신규선택", Before = "없음", After = "현관 중문" });
            TxtChangeCount.Text = $"총 {Changes.Count}건";

            TxtNewVersion.Text = "V3 - 2차 판매 반영";
        }

        // ===== ComboBox =====
        private void CboSalesRound_SelectionChanged(object sender, SelectionChangedEventArgs e) { }
        private void CboCompareVersion_SelectionChanged(object sender, SelectionChangedEventArgs e) { }

        // ===== 액션 =====
        private void BtnRunValidation_Click(object sender, RoutedEventArgs e) { }
        private void BtnRunCalculation_Click(object sender, RoutedEventArgs e) { }
        private void BtnViewResult_Click(object sender, RoutedEventArgs e) { }
    }
}