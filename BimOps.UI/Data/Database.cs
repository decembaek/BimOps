using System;
using System.IO;
using System.Reflection;
using Microsoft.Data.Sqlite;

namespace BimOps.UI.Data
{
    /// <summary>SQLite 연결 생성 + 스키마 초기화</summary>
    public static class Database
    {
        static Database()
        {
            SQLitePCL.Batteries_V2.Init();
        }
        /// <summary>단지 DB 파일이 없으면 생성하고 스키마 적용</summary>
        public static void EnsureProjectDb(string dbPath)
        {
            bool isNew = !File.Exists(dbPath);
            EnsureFile(dbPath);

            using (var conn = new SqliteConnection($"Data Source={dbPath}"))
            {
                conn.Open();
                ExecuteEmbeddedScript(conn, "BimOps.UI.Data.Schema.sql");
            }

            // 새로 생성된 DB라면 기본 데이터 시딩
            if (isNew)
            {
                SeedDefaultFinishCategories(dbPath);
            }
        }
        private static void SeedDefaultFinishCategories(string dbPath)
        {
            string connStr = $"Data Source={dbPath}";
            using (var conn = new SqliteConnection(connStr))
            {
                    conn.Open();
                    var defaults = new[]
                    {
                new { Code = "WALL",  Name = "벽지",     Uom = "㎡", Remark = (string)null },
                new { Code = "FLOOR", Name = "마루",     Uom = "㎡", Remark = (string)null },
                new { Code = "CEIL",  Name = "천장지",   Uom = "㎡", Remark = (string)null },
                new { Code = "BASE",  Name = "걸레받이", Uom = "m",  Remark = (string)null },
                new { Code = "TILE",  Name = "타일",     Uom = "㎡", Remark = (string)null },
            };

                using (var cmd = conn.CreateCommand())
                {
                    foreach (var d in defaults)
                    {
                        cmd.CommandText = @"
                    INSERT INTO finish_category (code, name, uom, remark)
                    VALUES (@code, @name, @uom, @remark)";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@code", d.Code);
                        cmd.Parameters.AddWithValue("@name", d.Name);
                        cmd.Parameters.AddWithValue("@uom", d.Uom);
                        cmd.Parameters.AddWithValue("@remark", (object)d.Remark ?? System.DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>단지 목록 DB가 없으면 생성</summary>
        public static void EnsureProjectsListDb(string dbPath)
        {
            EnsureFile(dbPath);
            using (var conn = new SqliteConnection($"Data Source={dbPath}"))
            {
                conn.Open();
                ExecuteEmbeddedScript(conn, "BimOps.UI.Data.SchemaProjects.sql");
            }
        }

        /// <summary>새 SQLite 연결 생성 (호출자가 using으로 관리)</summary>
        public static SqliteConnection OpenConnection(string connectionString)
        {
            var conn = new SqliteConnection(connectionString);
            conn.Open();

            // 외래키 제약 활성화 (SQLite 기본은 OFF)
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "PRAGMA foreign_keys = ON;";
                cmd.ExecuteNonQuery();
            }
            return conn;
        }

        // ===== 내부 유틸 =====

        private static void EnsureFile(string dbPath)
        {
            // Microsoft.Data.Sqlite는 연결 시점에 파일이 없으면 자동 생성하지만,
            // 폴더는 자동 생성 안 함. 폴더만 보장하면 됨.
            string dir = Path.GetDirectoryName(dbPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        private static void ExecuteEmbeddedScript(SqliteConnection conn, string resourceName)
        {
            var asm = Assembly.GetExecutingAssembly();
            using (var stream = asm.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new InvalidOperationException(
                        $"임베디드 리소스를 찾을 수 없습니다: {resourceName}");

                using (var reader = new StreamReader(stream))
                {
                    string sql = reader.ReadToEnd();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}