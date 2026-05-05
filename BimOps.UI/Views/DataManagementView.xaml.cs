using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace BimOps.UI.Views
{
    public partial class DataManagementView : UserControl
    {
        public class DataRow
        {
            public int No { get; set; }
            public string OptionCode { get; set; }
            public string OptionName { get; set; }
            public string Category { get; set; }
            public string RoomCode { get; set; }
            public string IsUsed { get; set; }
        }

        public ObservableCollection<DataRow> Rows { get; }
            = new ObservableCollection<DataRow>();

        public DataManagementView()
        {
            InitializeComponent();
            GridData.ItemsSource = Rows;

            // 더미 데이터 (옵션마스터 탭 기준)
            Rows.Add(new DataRow { No = 1, OptionCode = "OPT-BED2-WD", OptionName = "침실2 붙박이장", Category = "가구", RoomCode = "BED-02", IsUsed = "Y" });
            Rows.Add(new DataRow { No = 2, OptionCode = "OPT-BED3-WD", OptionName = "침실3 붙박이장", Category = "가구", RoomCode = "BED-03", IsUsed = "Y" });
            Rows.Add(new DataRow { No = 3, OptionCode = "OPT-ENT-DOOR", OptionName = "현관 중문", Category = "건축", RoomCode = "ENT", IsUsed = "Y" });
            Rows.Add(new DataRow { No = 4, OptionCode = "OPT-REF-CAB", OptionName = "냉장고장", Category = "가구", RoomCode = "KIT", IsUsed = "Y" });
        }

        private void DataTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded) return;
            // TODO: 선택된 탭에 맞는 데이터 로드
        }

        // ===== 툴바 =====
        private void BtnExcelImport_Click(object sender, RoutedEventArgs e) { }
        private void BtnExcelExport_Click(object sender, RoutedEventArgs e) { }
        private void BtnAddRow_Click(object sender, RoutedEventArgs e) { }
        private void BtnDeleteRow_Click(object sender, RoutedEventArgs e) { }
        private void BtnValidate_Click(object sender, RoutedEventArgs e) { }
        private void BtnSave_Click(object sender, RoutedEventArgs e) { }
    }
}