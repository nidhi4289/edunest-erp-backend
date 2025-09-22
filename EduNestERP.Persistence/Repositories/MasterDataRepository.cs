using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;
using EduNestERP.Persistence.Entities;

namespace EduNestERP.Persistence.Repositories
{
    public class MasterDataRepository : IMasterDataRepository

    {
        private readonly ITenantDataSourceProvider _dataSource;

        public MasterDataRepository(ITenantDataSourceProvider dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<List<Class>> GetAllClassesAsync()
        {
            await using var conn = await _dataSource.OpenConnectionAsync();
            const string sql = @"SELECT class_id, class_name, grade, section, class_subject_id, subject_id, subject_name FROM class_subject_view ORDER BY class_id, class_subject_id";
            await using var cmd = new NpgsqlCommand(sql, conn);
            await using var reader = await cmd.ExecuteReaderAsync();
            var classDict = new Dictionary<Guid, Class>();
            while (await reader.ReadAsync())
            {
                var classId = reader.GetGuid(0);
                if (!classDict.TryGetValue(classId, out var cls))
                {
                    cls = new Class
                    {
                        Id = classId,
                        Name = reader.GetString(1),
                        Grade = reader.GetString(2),
                        Section = reader.GetString(3),
                        ClassSubjects = new List<ClassSubject>()
                    };
                    classDict[classId] = cls;
                }
                // Add subject if present
                if (!reader.IsDBNull(4) && !reader.IsDBNull(5))
                {
                    var classSubjectId = reader.GetGuid(4);
                    var subjectId = reader.GetGuid(5);
                    var subjectName = reader.IsDBNull(6) ? null : reader.GetString(6);
                    cls.ClassSubjects.Add(new ClassSubject
                    {
                        Id = classSubjectId,
                        ClassId = classId,
                        SubjectId = subjectId,
                        SubjectName = subjectName
                    });
                }
            }
            return new List<Class>(classDict.Values);
        }

    public async Task AddClassAsync(Class newClass, List<Guid> subjectIds)
        {
            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var tx = await conn.BeginTransactionAsync();
            try
            {
                // Upsert class by grade+section
                Guid classId;
                const string selectClassSql = @"SELECT id FROM classes WHERE grade = @grade AND section = @section LIMIT 1";
                await using (var selectCmd = new NpgsqlCommand(selectClassSql, conn, tx))
                {
                    selectCmd.Parameters.AddWithValue("grade", newClass.Grade);
                    selectCmd.Parameters.AddWithValue("section", newClass.Section);
                    var result = await selectCmd.ExecuteScalarAsync();
                    if (result != null && Guid.TryParse(result.ToString(), out var existingId))
                    {
                        classId = existingId;
                        // Update name if changed
                        const string updateSql = @"UPDATE classes SET name = @name WHERE id = @id";
                        await using (var updateCmd = new NpgsqlCommand(updateSql, conn, tx))
                        {
                            updateCmd.Parameters.AddWithValue("id", classId);
                            updateCmd.Parameters.AddWithValue("name", newClass.Name);
                            await updateCmd.ExecuteNonQueryAsync();
                        }
                    }
                    else
                    {
                        classId = newClass.Id == Guid.Empty ? Guid.NewGuid() : newClass.Id;
                        const string insertSql = @"INSERT INTO classes (id, name, grade, section) VALUES (@id, @name, @grade, @section)";
                        await using (var insertCmd = new NpgsqlCommand(insertSql, conn, tx))
                        {
                            insertCmd.Parameters.AddWithValue("id", classId);
                            insertCmd.Parameters.AddWithValue("name", newClass.Name);
                            insertCmd.Parameters.AddWithValue("grade", newClass.Grade);
                            insertCmd.Parameters.AddWithValue("section", newClass.Section);
                            await insertCmd.ExecuteNonQueryAsync();
                        }
                    }
                }

                // Upsert class_subjects for each subject
                if (subjectIds != null)
                {
                    foreach (var subjectId in subjectIds)
                    {
                        // Check if exists
                        const string checkSql = @"SELECT id FROM class_subjects WHERE class_id = @classId AND subject_id = @subjectId LIMIT 1";
                        await using (var checkCmd = new NpgsqlCommand(checkSql, conn, tx))
                        {
                            checkCmd.Parameters.AddWithValue("classId", classId);
                            checkCmd.Parameters.AddWithValue("subjectId", subjectId);
                            var result = await checkCmd.ExecuteScalarAsync();
                            if (result != null && Guid.TryParse(result.ToString(), out var csId))
                            {
                                // Update updated_at
                                const string updateSql = @"UPDATE class_subjects SET updated_at = @updatedAt WHERE id = @id";
                                await using (var updateCmd = new NpgsqlCommand(updateSql, conn, tx))
                                {
                                    updateCmd.Parameters.AddWithValue("id", csId);
                                    updateCmd.Parameters.AddWithValue("updatedAt", DateTime.UtcNow);
                                    await updateCmd.ExecuteNonQueryAsync();
                                }
                            }
                            else
                            {
                                // Insert new
                                const string insertSql = @"INSERT INTO class_subjects (id, class_id, subject_id, created_at, updated_at) VALUES (@id, @classId, @subjectId, @createdAt, @updatedAt)";
                                await using (var insertCmd = new NpgsqlCommand(insertSql, conn, tx))
                                {
                                    insertCmd.Parameters.AddWithValue("id", Guid.NewGuid());
                                    insertCmd.Parameters.AddWithValue("classId", classId);
                                    insertCmd.Parameters.AddWithValue("subjectId", subjectId);
                                    insertCmd.Parameters.AddWithValue("createdAt", DateTime.UtcNow);
                                    insertCmd.Parameters.AddWithValue("updatedAt", DateTime.UtcNow);
                                    await insertCmd.ExecuteNonQueryAsync();
                                }
                            }
                        }
                    }
                }

                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<List<Assessment>> GetAllAssessmentsAsync(
            string? academicYear,
            string? grade,
            string? section,
            string? assessmentName,
            string? subjectName)
        {
            await using var conn = await _dataSource.OpenConnectionAsync();

            var sqlBuilder = new StringBuilder(@"SELECT grade, section, academic_year, assessment_id, assessment_name, subject_name, grading_type, max_marks FROM v_assessments_overview WHERE 1=1");
            var parameters = new List<NpgsqlParameter>();

            if (!string.IsNullOrWhiteSpace(academicYear))
            {
                sqlBuilder.Append(" AND academic_year = @academicYear");
                parameters.Add(new NpgsqlParameter("@academicYear", NpgsqlDbType.Text) { Value = academicYear });
            }
            if (!string.IsNullOrWhiteSpace(grade))
            {
                sqlBuilder.Append(" AND grade = @grade");
                parameters.Add(new NpgsqlParameter("@grade", NpgsqlDbType.Text) { Value = grade });
            }
            if (!string.IsNullOrWhiteSpace(section))
            {
                sqlBuilder.Append(" AND section = @section");
                parameters.Add(new NpgsqlParameter("@section", NpgsqlDbType.Text) { Value = section });
            }
            if (!string.IsNullOrWhiteSpace(subjectName))
            {
                sqlBuilder.Append(" AND subject_name ILIKE @subjectName");
                parameters.Add(new NpgsqlParameter("@subjectName", NpgsqlDbType.Text) { Value = $"%{subjectName}%" });
            }
            if (!string.IsNullOrWhiteSpace(assessmentName))
            {
                sqlBuilder.Append(" AND assessment_name ILIKE @assessmentName");
                parameters.Add(new NpgsqlParameter("@assessmentName", NpgsqlDbType.Text) { Value = $"%{assessmentName}%" });
            }

            sqlBuilder.Append(" ORDER BY grade::int, section, academic_year, subject_name, assessment_name");

            await using var cmd = new NpgsqlCommand(sqlBuilder.ToString(), conn);
            cmd.Parameters.AddRange(parameters.ToArray());

            await using var reader = await cmd.ExecuteReaderAsync();
            var assessments = new List<Assessment>();
            while (await reader.ReadAsync())
            {
                assessments.Add(new Assessment
                {
                    Grade = reader.GetString(reader.GetOrdinal("grade")),
                    Section = reader.GetString(reader.GetOrdinal("section")),
                    AcademicYear = reader.GetString(reader.GetOrdinal("academic_year")),
                    Id = reader.GetGuid(reader.GetOrdinal("assessment_id")),
                    Name = reader.GetString(reader.GetOrdinal("assessment_name")),
                    SubjectName = reader.GetString(reader.GetOrdinal("subject_name")),
                    GradingType = reader.GetString(reader.GetOrdinal("grading_type")),
                    MaxMarks = reader.GetDecimal(reader.GetOrdinal("max_marks"))
                });
            }
            return assessments;
        }

        public async Task AddAssessmentAsync(Assessment newAssessment)
        {
           
            await using var conn = await _dataSource.OpenConnectionAsync();
            var classSubjectId = await GetClassSubjectIdAsync(conn, newAssessment.ClassId, newAssessment.SubjectId);

            const string sql = @"INSERT INTO assessments (
                id, class_subject_id, name, assessment_date, grading_type, max_marks, created_at, updated_at, academic_year
            ) VALUES (
                @id, @class_subject_id, @name, @assessment_date, @grading_type, @max_marks, @created_at, @updated_at, @academic_year
            )";
            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("id", newAssessment.Id == Guid.Empty ? Guid.NewGuid() : newAssessment.Id);
            cmd.Parameters.AddWithValue("class_subject_id", classSubjectId);
            cmd.Parameters.AddWithValue("name", newAssessment.Name);
            cmd.Parameters.AddWithValue("assessment_date", newAssessment.AssessmentDate);
            cmd.Parameters.AddWithValue("grading_type", newAssessment.GradingType);
            cmd.Parameters.AddWithValue("max_marks", newAssessment.MaxMarks);
            cmd.Parameters.AddWithValue("created_at", newAssessment.CreatedAt);
            cmd.Parameters.AddWithValue("updated_at", newAssessment.UpdatedAt);
            cmd.Parameters.AddWithValue("academic_year", newAssessment.AcademicYear);
            await cmd.ExecuteNonQueryAsync();
        }

        private async Task<Guid> GetClassSubjectIdAsync(NpgsqlConnection conn, Guid classId, Guid subjectId
        )
        {
            const string sql = @"SELECT id FROM class_subjects WHERE class_id = @classId AND subject_id = @subjectId LIMIT 1";
            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("classId", classId);
            cmd.Parameters.AddWithValue("subjectId", subjectId);
            var result = await cmd.ExecuteScalarAsync();
            if (result != null && Guid.TryParse(result.ToString(), out var id))
            {
                return id;
            }
            throw new InvalidOperationException("ClassSubject not found for the given class and subject.");
        }

        public async Task<List<Subject>> GetSubjectsAsync()
        {
            await using var conn = await _dataSource.OpenConnectionAsync();
            const string sql = @"SELECT * FROM subjects";
            await using var cmd = new NpgsqlCommand(sql, conn);
            await using var reader = await cmd.ExecuteReaderAsync();
            var subjects = new List<Subject>();
            while (await reader.ReadAsync())
            {
                subjects.Add(new Subject
                {
                    Id = reader.GetGuid(reader.GetOrdinal("id")),
                    Name = reader.GetString(reader.GetOrdinal("name")),
                    Code = reader.GetString(reader.GetOrdinal("code")),
                    Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
                    GradingType = reader.IsDBNull(reader.GetOrdinal("grading_type")) ? null : reader.GetString(reader.GetOrdinal("grading_type")),
                    MaxMarks = reader.IsDBNull(reader.GetOrdinal("max_marks")) ? null : reader.GetInt32(reader.GetOrdinal("max_marks")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
                });
            }
            return subjects;
        }
    }
}