using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BimOps.UI.Views;

namespace BimOps.UI
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MainContent.Content = new DashboardView();
            StatusText.Text = "Status: 대시보드";
        }
        // ↑ 여기서 클래스를 닫지 말 것!

        private void NavigationMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded) return;

            var item = NavigationMenu.SelectedItem as ListBoxItem;
            if (item == null)
                return;

            switch (item.Content?.ToString())
            {
                case "대시보드":
                    MainContent.Content = new DashboardView();
                    StatusText.Text = "Status: 대시보드";
                    break;
                case "데이터 가져오기":
                    MainContent.Content = new ImportDataView();
                    StatusText.Text = "Status: 데이터 가져오기";
                    break;
                case "데이터 관리":
                    MainContent.Content = null;
                    StatusText.Text = "Status: 데이터 관리";
                    break;
                case "계산 / 검증":
                    MainContent.Content = null;
                    StatusText.Text = "Status: 계산 / 검증";
                    break;
                case "결과 / 보고서":
                    MainContent.Content = null;
                    StatusText.Text = "Status: 결과 / 보고서";
                    break;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: 저장 로직
            StatusText.Text = "Status: 저장됨";
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    } // ← MainWindow 클래스 닫는 중괄호
} // ← namespace 닫는 중괄호