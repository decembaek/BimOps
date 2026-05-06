using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using BimOps.UI.Models;

namespace BimOps.UI.Views
{
    public partial class QuantityCalculationView : UserControl
    {
        public ObservableCollection<ValidationItem> Validations { get; }
            = new ObservableCollection<ValidationItem>();
        public ObservableCollection<CalculationStage> Stages { get; }
            = new ObservableCollection<CalculationStage>();
        public ObservableCollection<FinishSummary> Results { get; }
            = new ObservableCollection<FinishSummary>();

        private readonly DispatcherTimer _elapsedTimer;
        private readonly Stopwatch _watch = new Stopwatch();
        private bool _isRunning;

        public event EventHandler GoToResult;

        public QuantityCalculationView()
        {
            InitializeComponent();
            BindData();
            LoadValidations();
            LoadStages();

            _elapsedTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            _elapsedTimer.Tick += (s, e) =>
                TxtElapsed.Text = $"경과 {_watch.Elapsed.TotalSeconds:0.0}초";
        }

        // =========================================================
        // 데이터 바인딩 / 초기 로드
        // =========================================================
        private void BindData()
        {
            ValidationList.ItemsSource = Validations;
            StageList.ItemsSource = Stages;
            ResultList.ItemsSource = Results;

            TxtSubtitle.Text = "JJ-A1 / 제주 A1블록 · 차수: 2차 (DRAFT)";
        }

        private void LoadValidations()
        {
            Validations.Clear();
            // 실제로는 ReferenceData + UnitOptionStatus를 검사한 결과를 받아옴
            Validations.Add(new ValidationItem
            {
                Title = "동·호 매칭",
                Description = "분양 데이터의 모든 동·호가 단지 마스터에 존재",
                Status = ValidationStatus.Pass,
                Detail = "1,248세대 매칭 성공"
            });
            Validations.Add(new ValidationItem
            {
                Title = "옵션 코드 매칭",
                Description = "선택된 옵션 코드가 카탈로그에 등록되어 있음",
                Status = ValidationStatus.Pass,
                Detail = "27종 모두 매칭"
            });
            Validations.Add(new ValidationItem
            {
                Title = "평형타입 매칭",
                Description = "각 세대의 평형타입이 단지 마스터에 정의됨",
                Status = ValidationStatus.Pass,
                Detail = "84A, 84B, 110 모두 매칭"
            });
            Validations.Add(new ValidationItem
            {
                Title = "설치위치 유효성",
                Description = "옵션이 정의된 설치 가능 위치에 배치됨",
                Status = ValidationStatus.Warn,
                Detail = "1건 위치 누락 (101-1003 붙박이장)"
            });
            Validations.Add(new ValidationItem
            {
                Title = "충돌 옵션 검사",
                Description = "동일 위치에 상충되는 옵션 동시 선택 없음",
                Status = ValidationStatus.Pass,
                Detail = "충돌 0건"
            });
            Validations.Add(new ValidationItem
            {
                Title = "차감 룩업 누락 검사",
                Description = "(옵션 × 위치 × 평형) 조합의 룩업이 모두 존재",
                Status = ValidationStatus.Pass,
                Detail = "312조합 모두 정의됨"
            });

            UpdateValidationSummary();
        }

        private void UpdateValidationSummary()
        {
            int pass = Validations.Count(v => v.Status == ValidationStatus.Pass);
            int warn = Validations.Count(v => v.Status == ValidationStatus.Warn);
            int fail = Validations.Count(v => v.Status == ValidationStatus.Fail);
            TxtValidationSummary.Text = $"통과 {pass} · 경고 {warn} · 실패 {fail}";

            // 실패 시 산출 실행 비활성화
            BtnRunCalc.IsEnabled = (fail == 0);
        }

        private void LoadStages()
        {
            Stages.Clear();
            Stages.Add(new CalculationStage { Title = "데이터 로드", Description = "옵션 선택 + 차감 룩업 로드" });
            Stages.Add(new CalculationStage { Title = "평형 매칭", Description = "각 세대의 평형타입 확인" });
            Stages.Add(new CalculationStage { Title = "차감 계산", Description = "옵션별 delta 적용" });
            Stages.Add(new CalculationStage { Title = "집계", Description = "동·세대·마감재별 합산" });
            Stages.Add(new CalculationStage { Title = "스냅샷 저장", Description = "차수 산출 결과 동결" });
        }

        // =========================================================
        // 검증 / 산출 실행
        // =========================================================
        private void BtnRevalidate_Click(object sender, RoutedEventArgs e)
        {
            // TODO: 실제 검증 로직 호출
            LoadValidations();
        }

        private async void BtnRunCalc_Click(object sender, RoutedEventArgs e)
        {
            if (_isRunning) return;

            int warn = Validations.Count(v => v.Status == ValidationStatus.Warn);
            if (warn > 0)
            {
                var ans = MessageBox.Show(
                    $"검증 경고 {warn}건이 있습니다. 그래도 산출을 실행할까요?",
                    "BimOps", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (ans != MessageBoxResult.OK) return;
            }

            await RunCalculationAsync();
        }

        private async Task RunCalculationAsync()
        {
            _isRunning = true;
            BtnRunCalc.IsEnabled = false;
            BtnRevalidate.IsEnabled = false;
            ResultSection.Visibility = Visibility.Collapsed;
            CalcProgress.Value = 0;

            // 단계 초기화
            foreach (var st in Stages) st.Status = CalculationStageStatus.Pending;

            _watch.Restart();
            _elapsedTimer.Start();

            try
            {
                for (int i = 0; i < Stages.Count; i++)
                {
                    Stages[i].Status = CalculationStageStatus.Running;
                    await Task.Delay(700); // 실제 산출 로직으로 대체
                    Stages[i].Status = CalculationStageStatus.Done;
                    CalcProgress.Value = (i + 1) * 100.0 / Stages.Count;
                }

                LoadSampleResults();
                ResultSection.Visibility = Visibility.Visible;
            }
            finally
            {
                _watch.Stop();
                _elapsedTimer.Stop();
                TxtElapsed.Text = $"완료 · {_watch.Elapsed.TotalSeconds:0.0}초";
                _isRunning = false;
                BtnRunCalc.IsEnabled = true;
                BtnRevalidate.IsEnabled = true;
            }
        }

        private void LoadSampleResults()
        {
            Results.Clear();
            Results.Add(new FinishSummary { FinishCode = "WALL", FinishName = "벽지", TotalQty = 11623.4, Uom = "㎡" });
            Results.Add(new FinishSummary { FinishCode = "FLOOR", FinishName = "마루", TotalQty = 6056.2, Uom = "㎡" });
            Results.Add(new FinishSummary { FinishCode = "CEIL", FinishName = "천장지", TotalQty = 5688.0, Uom = "㎡" });
            Results.Add(new FinishSummary { FinishCode = "BASE", FinishName = "걸레받이", TotalQty = 18180.4, Uom = "m" });
            Results.Add(new FinishSummary { FinishCode = "TILE", FinishName = "타일", TotalQty = 3214.6, Uom = "㎡" });
        }

        private void BtnGoResult_Click(object sender, RoutedEventArgs e)
            => GoToResult?.Invoke(this, EventArgs.Empty);
    }
}