using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BimOps.UI.Models;

namespace BimOps.UI.Views
{
    public partial class ReferenceDataView : UserControl
    {
        // ===== 데이터 컬렉션 =====
        public ObservableCollection<UnitTypeInfo> UnitTypes { get; }
            = new ObservableCollection<UnitTypeInfo>();
        public ObservableCollection<UnitTypeListItem> UnitTypeListItems { get; }
            = new ObservableCollection<UnitTypeListItem>();

        public ObservableCollection<BaseQuantityRow> BaseQuantities { get; }
            = new ObservableCollection<BaseQuantityRow>();

        public ObservableCollection<OptionItem> Options { get; }
            = new ObservableCollection<OptionItem>();
        public ObservableCollection<OptionLookupRow> Lookups { get; }
            = new ObservableCollection<OptionLookupRow>();

        // 마감재 카테고리 목록 (프로젝트 설정에서 받아옴)
        public List<FinishCategory> FinishCategories { get; private set; }
            = new List<FinishCategory>();

        public ReferenceDataView()
        {
            InitializeComponent();
            LoadSampleData();
            BindGrids();
            UpdateCounts();
            UpdateSubtitle();
        }

        // =========================================================
        // 데이터 로드 / 바인딩
        // =========================================================
        public void LoadProject(string projectCode, string projectName,
                                IEnumerable<FinishCategory> finishCategories)
        {
            // 외부에서 호출 시 사용. 지금은 샘플 데이터로 시작.
            FinishCategories = finishCategories?.ToList() ?? FinishCategories;
            UpdateSubtitle(projectCode, projectName);
        }

        private void BindGrids()
        {
            UnitTypeGrid.ItemsSource = UnitTypes;
            UnitListForBQ.ItemsSource = UnitTypeListItems;
            BQGrid.ItemsSource = BaseQuantities;
            OptionGrid.ItemsSource = Options;
            OptionListForLookup.ItemsSource = Options;
            LookupGrid.ItemsSource = Lookups;
        }

        private void LoadSampleData()
        {
            // 마감재 카테고리 (프로젝트 설정에서 정의되었다고 가정)
            FinishCategories = new List<FinishCategory>
            {
                new FinishCategory { Code = "WALL",  Name = "벽지",     Uom = "㎡" },
                new FinishCategory { Code = "FLOOR", Name = "마루",     Uom = "㎡" },
                new FinishCategory { Code = "CEIL",  Name = "천장지",   Uom = "㎡" },
                new FinishCategory { Code = "BASE",  Name = "걸레받이", Uom = "m"  },
                new FinishCategory { Code = "TILE",  Name = "타일",     Uom = "㎡" },
            };

            // 평형타입
            UnitTypes.Add(new UnitTypeInfo { Code = "84A", Name = "84A형", NetArea = 84.92, Remark = "표준" });
            UnitTypes.Add(new UnitTypeInfo { Code = "84B", Name = "84B형", NetArea = 84.78, Remark = "" });
            UnitTypes.Add(new UnitTypeInfo { Code = "110", Name = "110형", NetArea = 109.65, Remark = "프리미엄" });

            // Revit 기본물량 (84A 예시)
            BaseQuantities.Add(new BaseQuantityRow { UnitTypeCode = "84A", FinishCode = "WALL", FinishName = "벽지", AppliedRoom = "전 실 벽면", Quantity = 82.8, Uom = "㎡", Source = "Revit" });
            BaseQuantities.Add(new BaseQuantityRow { UnitTypeCode = "84A", FinishCode = "FLOOR", FinishName = "마루", AppliedRoom = "거실, 침실 1·2·3", Quantity = 42.4, Uom = "㎡", Source = "Revit" });
            BaseQuantities.Add(new BaseQuantityRow { UnitTypeCode = "84A", FinishCode = "CEIL", FinishName = "천장지", AppliedRoom = "전 실 천장", Quantity = 40.6, Uom = "㎡", Source = "Revit" });
            BaseQuantities.Add(new BaseQuantityRow { UnitTypeCode = "84A", FinishCode = "BASE", FinishName = "걸레받이", AppliedRoom = "전 실 바닥 둘레", Quantity = 116.0, Uom = "m", Source = "Revit" });
            BaseQuantities.Add(new BaseQuantityRow { UnitTypeCode = "84A", FinishCode = "TILE", FinishName = "타일", AppliedRoom = "주방, 욕실 1·2", Quantity = 28.6, Uom = "㎡", Source = "수동" });

            // 옵션 카탈로그
            Options.Add(new OptionItem { Code = "BU01", Name = "붙박이장", Category = "가구", InstallRooms = "침실2, 침실3, 드레스룸" });
            Options.Add(new OptionItem { Code = "KF01", Name = "냉장고장", Category = "주방", InstallRooms = "주방" });
            Options.Add(new OptionItem { Code = "MD01", Name = "중문", Category = "도어", InstallRooms = "현관" });
            Options.Add(new OptionItem { Code = "EX01", Name = "베란다 확장", Category = "구조", InstallRooms = "거실, 침실1" });

            // 차감 룩업 (BU01 일부)
            Lookups.Add(new OptionLookupRow { OptionCode = "BU01", InstallRoom = "침실2", UnitTypeCode = "84A", FinishCode = "WALL", FinishName = "벽지", DeltaQty = -7.2, Uom = "㎡" });
            Lookups.Add(new OptionLookupRow { OptionCode = "BU01", InstallRoom = "침실2", UnitTypeCode = "84A", FinishCode = "FLOOR", FinishName = "마루", DeltaQty = -1.8, Uom = "㎡" });
            Lookups.Add(new OptionLookupRow { OptionCode = "BU01", InstallRoom = "침실2", UnitTypeCode = "84A", FinishCode = "CEIL", FinishName = "천장지", DeltaQty = -1.8, Uom = "㎡" });
            Lookups.Add(new OptionLookupRow { OptionCode = "BU01", InstallRoom = "침실2", UnitTypeCode = "84A", FinishCode = "BASE", FinishName = "걸레받이", DeltaQty = -5.4, Uom = "m" });

            RefreshUnitTypeListItems();
        }

        private void RefreshUnitTypeListItems()
        {
            UnitTypeListItems.Clear();
            foreach (var ut in UnitTypes)
            {
                UnitTypeListItems.Add(new UnitTypeListItem
                {
                    Code = ut.Code,
                    NetAreaLabel = $"전용 {ut.NetArea:0.##}㎡ · {FinishCategories.Count}종 마감재"
                });
            }
        }

        // =========================================================
        // UI 갱신
        // =========================================================
        private void UpdateCounts()
        {
            TxtUnitTypeCount.Text = UnitTypes.Count.ToString();
            TxtOptionCount.Text = Options.Count.ToString();
            TxtLookupCount.Text = Lookups.Count.ToString();
        }

        private void UpdateSubtitle(string projectCode = "JJ-A1", string projectName = "제주 A1블록")
        {
            TxtSubtitle.Text = $"{projectCode} / {projectName}";
        }

        // =========================================================
        // 탭 2: Revit 기본물량
        // =========================================================
        private void UnitListForBQ_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(UnitListForBQ.SelectedItem is UnitTypeListItem item))
            {
                BQGrid.ItemsSource = null;
                TxtBQTitle.Text = "평형을 선택하세요";
                TxtBQSubtitle.Text = "";
                return;
            }

            var rows = BaseQuantities.Where(b => b.UnitTypeCode == item.Code).ToList();
            BQGrid.ItemsSource = rows;
            TxtBQTitle.Text = $"{item.Code} · 기본 마감재 물량";
            TxtBQSubtitle.Text = $"옵션 미적용 기준 · {rows.Count}개 행";
        }

        private void BtnExtractFromRevit_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Revit API 연동 — 현재 평형의 마감재 물량 추출
            MessageBox.Show("Revit API 연동 (구현 예정): 모델에서 평형타입별 마감재 물량을 추출합니다.",
                "BimOps", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnAddBQRow_Click(object sender, RoutedEventArgs e)
        {
            if (!(UnitListForBQ.SelectedItem is UnitTypeListItem item))
            {
                MessageBox.Show("평형을 먼저 선택하세요.", "BimOps");
                return;
            }
            BaseQuantities.Add(new BaseQuantityRow
            {
                UnitTypeCode = item.Code,
                Source = "수동"
            });
            // 새로 추가된 행이 보이도록 필터 갱신
            UnitListForBQ_SelectionChanged(null, null);
        }

        // =========================================================
        // 탭 4: 차감 룩업
        // =========================================================
        private void OptionListForLookup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(OptionListForLookup.SelectedItem is OptionItem op))
            {
                LookupGrid.ItemsSource = null;
                TxtLookupTitle.Text = "옵션을 선택하세요";
                TxtLookupSubtitle.Text = "";
                return;
            }
            var rows = Lookups.Where(l => l.OptionCode == op.Code).ToList();
            LookupGrid.ItemsSource = rows;
            TxtLookupTitle.Text = $"{op.Code} · {op.Name}";
            TxtLookupSubtitle.Text = $"카테고리: {op.Category} · 설치 가능: {op.InstallRooms}";
        }

        private void BtnAddLookupRow_Click(object sender, RoutedEventArgs e)
        {
            if (!(OptionListForLookup.SelectedItem is OptionItem op))
            {
                MessageBox.Show("옵션을 먼저 선택하세요.", "BimOps");
                return;
            }
            Lookups.Add(new OptionLookupRow
            {
                OptionCode = op.Code,
            });
            OptionListForLookup_SelectionChanged(null, null);
        }

        // =========================================================
        // 탭 1, 3: CRUD 버튼
        // =========================================================
        private void BtnAddUnitType_Click(object sender, RoutedEventArgs e)
        {
            UnitTypes.Add(new UnitTypeInfo { Code = "신규", Name = "" });
            RefreshUnitTypeListItems();
            UpdateCounts();
        }

        private void BtnDeleteUnitType_Click(object sender, RoutedEventArgs e)
        {
            if (UnitTypeGrid.SelectedItem is UnitTypeInfo ut)
            {
                UnitTypes.Remove(ut);
                RefreshUnitTypeListItems();
                UpdateCounts();
            }
        }

        private void BtnAddOption_Click(object sender, RoutedEventArgs e)
        {
            Options.Add(new OptionItem { Code = "신규" });
            UpdateCounts();
        }

        private void BtnDeleteOption_Click(object sender, RoutedEventArgs e)
        {
            if (OptionGrid.SelectedItem is OptionItem op)
            {
                Options.Remove(op);
                UpdateCounts();
            }
        }

        private void BtnExcelImport_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Excel 일괄 임포트 다이얼로그 (구현 예정)", "BimOps");
        }
    }

    /// <summary>좌측 평형 리스트 표시용 가공 모델</summary>
    public class UnitTypeListItem
    {
        public string Code { get; set; }
        public string NetAreaLabel { get; set; }
    }
}