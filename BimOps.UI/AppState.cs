using System;
using System.Collections.Generic;
using System.IO;

using BimOps.UI.Views;
using BimOps.UI.Models;

namespace BimOps.UI
{
    /// <summary>
    /// 프로세스 단위 전역 상태 슬롯. ProjectSelectionWindow → MainWindow 전환 시 데이터 전달용.
    /// 실제 환경에서는 DI / 상태 관리 라이브러리로 교체.
    /// </summary>
    public static class AppState
    {
        public static ProjectCardItem SelectedProject { get; set; }
        public static IEnumerable<ProjectCardItem> AvailableProjects { get; set; }
        public static Func<ProjectCardItem, IEnumerable<RoundTimelineItem>> LoadRounds { get; set; }
            = _ => null;

        // ===== DB 경로 관리 =====
        public static string DataRoot { get; } = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "BimOps", "Projects");

        public static string ProjectsListPath => Path.Combine(DataRoot, "_projects.db");

        /// <summary>현재 선택된 단지의 SQLite 파일 경로</summary>
        public static string CurrentProjectDbPath
        {
            get
            {
                if (SelectedProject == null) return null;
                return Path.Combine(DataRoot, $"{SelectedProject.Code}.db");
            }
        }
        public static string CurrentConnectionString
        {
            get
            {
                var path = CurrentProjectDbPath;
                if (string.IsNullOrEmpty(path)) return null;
                return $"Data Source={path}";
            }
        }

        public static string ProjectsListConnectionString
            => $"Data Source={ProjectsListPath}";
        /// <summary>앱 시작 시 1회 호출. 데이터 폴더 생성.</summary>
        public static void EnsureDataRoot()
        {
            if (!Directory.Exists(DataRoot))
                Directory.CreateDirectory(DataRoot);
        }
    }
}