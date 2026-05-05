using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace BimOps.UI.Views
{
    public partial class ImportDataView : UserControl
    {
        // 미리보기 행 모델
        public class PreviewRow
        {
            public string Building { get; set; }
            public string UnitNo { get; set; }
            public string UnitType { get; set; }
            public string Space { get; set; }
            public string OptionName { get; set; }
            public string Status { get; set; }
        }

        public ObservableCollection<PreviewRow> PreviewRows { get; }
            = new ObservableCollection<PreviewRow>();

        public ImportDataView()
        {
            InitializeComponent();
            GridPreview.ItemsSource = PreviewRows;

            // 화면 확인용 더미 데이터 (나중에 제거)
            PreviewRows.Add(new PreviewRow { Building = "101", UnitNo = "1001", UnitType = "84A", Space = "침실2", OptionName = "침실2 붙박이장", Status = "Selected" });
            PreviewRows.Add(new PreviewRow { Building = "101", UnitNo = "1002", UnitType = "84A", Space = "현관", OptionName = "현관 중문", Status = "Selected" });
            PreviewRows.Add(new PreviewRow { Building = "102", UnitNo = "0803", UnitType = "84B", Space = "침실3", OptionName = "침실3 붙박이장", Status = "Changed" });
            TxtPreviewSummary.Text = $"총 {PreviewRows.Count}건";
        }

        // ===== ComboBox 이벤트 =====
        private void CboProject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO: 프로젝트 선택 시 처리
        }

        private void CboSalesRound_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO: 판매 차수 선택 시 처리
        }

        private void CboCompareBaseline_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO: 비교 기준 선택 시 처리
        }

        // ===== 파일 찾기 =====
        private void BtnFindSalesOption_Click(object sender, RoutedEventArgs e)
        {
            // TODO: 분양 옵션 엑셀 파일 다이얼로그
        }

        private void BtnFindRevitQuantity_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Revit 물량 엑셀 파일 다이얼로그
        }

        private void BtnFindOptionRules_Click(object sender, RoutedEventArgs e)
        {
            // TODO: 옵션 룰 엑셀 파일 다이얼로그
        }

        // ===== 액션 =====
        private void BtnPreview_Click(object sender, RoutedEventArgs e)
        {
            // TODO: 미리보기 - 파일 파싱 후 그리드 갱신
        }

        private void BtnValidate_Click(object sender, RoutedEventArgs e)
        {
            // TODO: 검증
        }

        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            // TODO: 가져오기
        }
    }
}