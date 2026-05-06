using System.ComponentModel;
using System.Windows.Media;

namespace BimOps.UI.Models
{
    public enum ValidationStatus { Pass, Warn, Fail }

    public class ValidationItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public ValidationStatus Status { get; set; }
        public string Detail { get; set; }

        public string StatusText
        {
            get
            {
                switch (Status)
                {
                    case ValidationStatus.Pass: return "통과";
                    case ValidationStatus.Warn: return "경고";
                    case ValidationStatus.Fail: return "실패";
                    default: return "—";
                }
            }
        }

        public Brush StatusBgBrush
        {
            get
            {
                switch (Status)
                {
                    case ValidationStatus.Pass: return (Brush)new BrushConverter().ConvertFrom("#E1F5EE");
                    case ValidationStatus.Warn: return (Brush)new BrushConverter().ConvertFrom("#FAEEDA");
                    case ValidationStatus.Fail: return (Brush)new BrushConverter().ConvertFrom("#FCEBEB");
                    default: return Brushes.Transparent;
                }
            }
        }

        public Brush StatusFgBrush
        {
            get
            {
                switch (Status)
                {
                    case ValidationStatus.Pass: return (Brush)new BrushConverter().ConvertFrom("#0F6E56");
                    case ValidationStatus.Warn: return (Brush)new BrushConverter().ConvertFrom("#854F0B");
                    case ValidationStatus.Fail: return (Brush)new BrushConverter().ConvertFrom("#A32D2D");
                    default: return Brushes.Black;
                }
            }
        }
    }

    public enum CalculationStageStatus { Pending, Running, Done, Failed }

    public class CalculationStage : INotifyPropertyChanged
    {
        private CalculationStageStatus _status = CalculationStageStatus.Pending;

        public string Title { get; set; }
        public string Description { get; set; }

        public CalculationStageStatus Status
        {
            get { return _status; }
            set
            {
                _status = value;
                Raise(nameof(Status));
                Raise(nameof(StatusIcon));
                Raise(nameof(StatusBrush));
            }
        }

        public string StatusIcon
        {
            get
            {
                switch (Status)
                {
                    case CalculationStageStatus.Pending: return "○";
                    case CalculationStageStatus.Running: return "●";
                    case CalculationStageStatus.Done: return "✓";
                    case CalculationStageStatus.Failed: return "✕";
                    default: return "○";
                }
            }
        }

        public Brush StatusBrush
        {
            get
            {
                switch (Status)
                {
                    case CalculationStageStatus.Pending: return (Brush)new BrushConverter().ConvertFrom("#9E9E9E");
                    case CalculationStageStatus.Running: return (Brush)new BrushConverter().ConvertFrom("#1E5A8E");
                    case CalculationStageStatus.Done: return (Brush)new BrushConverter().ConvertFrom("#0F6E56");
                    case CalculationStageStatus.Failed: return (Brush)new BrushConverter().ConvertFrom("#A32D2D");
                    default: return Brushes.Gray;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void Raise(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class FinishSummary
    {
        public string FinishCode { get; set; }
        public string FinishName { get; set; }
        public double TotalQty { get; set; }
        public string Uom { get; set; }
        public string DisplayQty => TotalQty.ToString("N1");
    }
}