using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BimOps.UI.Data;
using System;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Interop;

namespace BimOps.UI.Commands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class LaunchBimOpsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
                              ref string message,
                              ElementSet elements)
        {
            try
            {
                // 1. 데이터 폴더 / 단지 목록 DB 초기화 (첫 실행 시 1회)
                AppState.EnsureDataRoot();
                Database.EnsureProjectsListDb(AppState.ProjectsListPath);

                // 2. 프로젝트 선택 화면 띄우기
                var window = new ProjectSelectionWindow();

                // 3. Revit 메인 윈도우를 Owner로 설정 (모달리스라도 Revit 앞에 떠야 함)
                var helper = new WindowInteropHelper(window)
                {
                    Owner = commandData.Application.MainWindowHandle
                };

                // 4. 모달리스로 띄우기 (Revit 작업 병행 가능)
                window.Show();

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                System.Windows.MessageBox.Show(
                    $"BimOps 시작 중 오류:\n\n{ex.Message}\n\n{ex.StackTrace}",
                    "BimOps", MessageBoxButton.OK, MessageBoxImage.Error);
                return Result.Failed;
            }
        }
    }
}