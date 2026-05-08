using System.Collections.Generic;
using System.Linq;
using Dapper;
using Microsoft.Data.Sqlite;
using BimOps.UI.Models;

namespace BimOps.UI.Data.Repositories
{
    /// <summary>단지 DB의 finish_category 테이블 CRUD</summary>
    public class FinishCategoryRepository
    {
        private readonly string _connStr;

        public FinishCategoryRepository(string connectionString)
        {
            _connStr = connectionString;
        }

        // ===== 조회 =====

        public List<FinishCategory> GetAll()
        {
            using (var conn = new SqliteConnection(_connStr))
            {
                conn.Open();
                var rows = conn.Query<FinishCategoryRow>(@"
                    SELECT code, name, uom, remark
                    FROM finish_category
                    ORDER BY code").ToList();

                return rows.Select(MapToModel).ToList();
            }
        }

        public FinishCategory GetByCode(string code)
        {
            using (var conn = new SqliteConnection(_connStr))
            {
                conn.Open();
                var row = conn.QueryFirstOrDefault<FinishCategoryRow>(@"
                    SELECT code, name, uom, remark
                    FROM finish_category
                    WHERE code = @code",
                    new { code });

                return row == null ? null : MapToModel(row);
            }
        }

        public bool Exists(string code)
        {
            using (var conn = new SqliteConnection(_connStr))
            {
                conn.Open();
                int count = conn.ExecuteScalar<int>(
                    "SELECT COUNT(*) FROM finish_category WHERE code = @code",
                    new { code });
                return count > 0;
            }
        }

        // ===== 등록 / 수정 =====

        public void Insert(FinishCategory item)
        {
            using (var conn = new SqliteConnection(_connStr))
            {
                conn.Open();
                conn.Execute(@"
                    INSERT INTO finish_category (code, name, uom, remark)
                    VALUES (@Code, @Name, @Uom, @Remark)",
                    new { item.Code, item.Name, item.Uom, item.Remark });
            }
        }

        public void Update(FinishCategory item)
        {
            using (var conn = new SqliteConnection(_connStr))
            {
                conn.Open();
                conn.Execute(@"
                    UPDATE finish_category SET
                        name = @Name,
                        uom = @Uom,
                        remark = @Remark
                    WHERE code = @Code",
                    new { item.Code, item.Name, item.Uom, item.Remark });
            }
        }

        public void Delete(string code)
        {
            using (var conn = new SqliteConnection(_connStr))
            {
                conn.Open();
                conn.Execute(
                    "DELETE FROM finish_category WHERE code = @code",
                    new { code });
            }
        }

        /// <summary>전체를 새 목록으로 교체. 저장 버튼 클릭 시 사용.</summary>
        public void ReplaceAll(IEnumerable<FinishCategory> items)
        {
            using (var conn = new SqliteConnection(_connStr))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    conn.Execute("DELETE FROM finish_category", transaction: tx);

                    foreach (var item in items)
                    {
                        conn.Execute(@"
                            INSERT INTO finish_category (code, name, uom, remark)
                            VALUES (@Code, @Name, @Uom, @Remark)",
                            new { item.Code, item.Name, item.Uom, item.Remark },
                            transaction: tx);
                    }

                    tx.Commit();
                }
            }
        }

        // ===== 매핑 =====

        private FinishCategory MapToModel(FinishCategoryRow row)
        {
            return new FinishCategory
            {
                Code = row.code,
                Name = row.name,
                Uom = row.uom,
                Remark = row.remark,
            };
        }

        // ===== 내부 DTO =====

        private class FinishCategoryRow
        {
            public string code { get; set; }
            public string name { get; set; }
            public string uom { get; set; }
            public string remark { get; set; }
        }
    }
}