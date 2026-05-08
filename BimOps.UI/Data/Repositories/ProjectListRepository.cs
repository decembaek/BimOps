using BimOps.UI.Models;
using BimOps.UI.Views;
using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BimOps.UI.Data.Repositories
{
    /// <summary>_projects.db의 project_list 테이블 CRUD</summary>
    public class ProjectListRepository
    {
        private readonly string _connStr;

        public ProjectListRepository(string connectionString)
        {
            _connStr = connectionString;
        }

        // ===== 조회 =====

        public List<ProjectCardItem> GetAll()
        {
            using (var conn = new SqliteConnection(_connStr))
            {
                conn.Open();
                var rows = conn.Query<ProjectListRow>(@"
                    SELECT code, name, building_count, unit_count, unit_types,
                           latest_round, latest_status, status, last_modified
                    FROM project_list
                    ORDER BY last_modified DESC").ToList();

                return rows.Select(MapToCard).ToList();
            }
        }

        public ProjectCardItem GetByCode(string code)
        {
            using (var conn = new SqliteConnection(_connStr))
            {
                conn.Open();
                var row = conn.QueryFirstOrDefault<ProjectListRow>(@"
                    SELECT code, name, building_count, unit_count, unit_types,
                           latest_round, latest_status, status, last_modified
                    FROM project_list
                    WHERE code = @code",
                    new { code });

                return row == null ? null : MapToCard(row);
            }
        }

        // ===== 등록 =====

        public void Insert(ProjectCardItem project)
        {
            using (var conn = new SqliteConnection(_connStr))
            {
                conn.Open();
                conn.Execute(@"
                    INSERT INTO project_list
                        (code, name, building_count, unit_count, unit_types,
                         latest_round, latest_status, status, last_modified)
                    VALUES
                        (@Code, @Name, @BuildingCount, @UnitCount, @UnitTypes,
                         @LatestRound, @LatestStatus, @Status, @LastModified)",
                    new
                    {
                        project.Code,
                        project.Name,
                        project.BuildingCount,
                        project.UnitCount,
                        project.UnitTypes,
                        project.LatestRound,
                        project.LatestStatus,
                        Status = project.Status.ToString(),
                        LastModified = project.LastModified.ToString("yyyy-MM-dd HH:mm:ss"),
                    });
            }
        }

        // ===== 수정 =====

        public void Update(ProjectCardItem project)
        {
            using (var conn = new SqliteConnection(_connStr))
            {
                conn.Open();
                conn.Execute(@"
                    UPDATE project_list SET
                        name = @Name,
                        building_count = @BuildingCount,
                        unit_count = @UnitCount,
                        unit_types = @UnitTypes,
                        latest_round = @LatestRound,
                        latest_status = @LatestStatus,
                        status = @Status,
                        last_modified = @LastModified
                    WHERE code = @Code",
                    new
                    {
                        project.Code,
                        project.Name,
                        project.BuildingCount,
                        project.UnitCount,
                        project.UnitTypes,
                        project.LatestRound,
                        project.LatestStatus,
                        Status = project.Status.ToString(),
                        LastModified = project.LastModified.ToString("yyyy-MM-dd HH:mm:ss"),
                    });
            }
        }

        public void TouchLastModified(string code)
        {
            using (var conn = new SqliteConnection(_connStr))
            {
                conn.Open();
                conn.Execute(@"
                    UPDATE project_list
                    SET last_modified = @LastModified
                    WHERE code = @Code",
                    new
                    {
                        Code = code,
                        LastModified = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    });
            }
        }

        // ===== 삭제 =====

        public void Delete(string code)
        {
            using (var conn = new SqliteConnection(_connStr))
            {
                conn.Open();
                conn.Execute(
                    "DELETE FROM project_list WHERE code = @Code",
                    new { Code = code });
            }
        }

        // ===== 매핑 =====

        private ProjectCardItem MapToCard(ProjectListRow row)
        {
            return new ProjectCardItem
            {
                Code = row.code,
                Name = row.name,
                BuildingCount = row.building_count,
                UnitCount = row.unit_count,
                UnitTypes = row.unit_types,
                LatestRound = row.latest_round,
                LatestStatus = row.latest_status,
                Status = ParseStatus(row.status),
                LastModified = ParseDateTime(row.last_modified),
            };
        }

        private static ProjectStatus ParseStatus(string s)
        {
            if (Enum.TryParse<ProjectStatus>(s, out var status))
                return status;
            return ProjectStatus.InProgress;
        }

        private static DateTime ParseDateTime(string s)
        {
            if (DateTime.TryParse(s, out var dt)) return dt;
            return DateTime.Now;
        }

        // ===== 내부 DTO =====

        private class ProjectListRow
        {
            public string code { get; set; }
            public string name { get; set; }
            public int building_count { get; set; }
            public int unit_count { get; set; }
            public string unit_types { get; set; }
            public string latest_round { get; set; }
            public string latest_status { get; set; }
            public string status { get; set; }
            public string last_modified { get; set; }
        }
    }
}