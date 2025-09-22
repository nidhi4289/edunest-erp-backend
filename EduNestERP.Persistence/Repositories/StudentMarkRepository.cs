using EduNestERP.Persistence.Entities;
using Npgsql;
using NpgsqlTypes;
using System.Text;

namespace EduNestERP.Persistence.Repositories
{
    public class StudentMarkRepository : IStudentMarkRepository
    {
         private readonly ITenantDataSourceProvider _dataSource;

        public StudentMarkRepository(ITenantDataSourceProvider dataSource)
        {
            _dataSource = dataSource;
        }


        public async Task<bool?> AddAsync(StudentMark studentMark)
        {
            await using var conn = await _dataSource.OpenConnectionAsync();

            const string sql = @"
                INSERT INTO student_marks (assessment_id, edunest_id, marks_obtained, grade_awarded, remarks, created_at, updated_at)
                VALUES (@assessmentId, @eduNestId, @marksObtained, @gradeAwarded, @remarks, @createdAt, @updatedAt)";

            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add("@assessmentId", NpgsqlDbType.Uuid).Value = studentMark.AssessmentId;
            cmd.Parameters.Add("@eduNestId", NpgsqlDbType.Text).Value = studentMark.EduNestId;
            cmd.Parameters.Add("@marksObtained", NpgsqlDbType.Numeric).Value = studentMark.MarksObtained ?? (object)DBNull.Value;
            cmd.Parameters.Add("@gradeAwarded", NpgsqlDbType.Text).Value = studentMark.GradeAwarded ?? (object)DBNull.Value;
            cmd.Parameters.Add("@remarks", NpgsqlDbType.Text).Value = studentMark.Remarks ?? (object)DBNull.Value;
            cmd.Parameters.Add("@createdAt", NpgsqlDbType.TimestampTz).Value = DateTime.UtcNow;
            cmd.Parameters.Add("@updatedAt", NpgsqlDbType.TimestampTz).Value = DateTime.UtcNow;

            var result = await cmd.ExecuteNonQueryAsync();
            return result > 0;
        }

        public async Task<bool> UpdateAsync(StudentMark studentMark)
        {
            await using var conn = await _dataSource.OpenConnectionAsync();

            const string sql = @"
                UPDATE student_marks 
                SET marks_obtained = @marksObtained, 
                    grade_awarded = @gradeAwarded, 
                    remarks = @remarks, 
                    updated_at = @updatedAt
                WHERE id = @id";

            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add("@id", NpgsqlDbType.Uuid).Value = studentMark.Id;
            cmd.Parameters.Add("@marksObtained", NpgsqlDbType.Numeric).Value = studentMark.MarksObtained ?? (object)DBNull.Value;
            cmd.Parameters.Add("@gradeAwarded", NpgsqlDbType.Text).Value = studentMark.GradeAwarded ?? (object)DBNull.Value;
            cmd.Parameters.Add("@remarks", NpgsqlDbType.Text).Value = studentMark.Remarks ?? (object)DBNull.Value;
            cmd.Parameters.Add("@updatedAt", NpgsqlDbType.TimestampTz).Value = DateTime.UtcNow;

            var result = await cmd.ExecuteNonQueryAsync();
            return result > 0;
        }

        public async Task<bool?> BulkAddAsync(List<StudentMark> studentMarks)
        {
            if (!studentMarks.Any()) return false;

            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var transaction = await conn.BeginTransactionAsync();

               try
            {
                 const string sql = @"
                WITH upd AS (
                    UPDATE student_marks
                        SET marks_obtained = @marksObtained,
                            grade_awarded  = @gradeAwarded,
                            remarks        = @remarks,
                            updated_at     = now()
                    WHERE assessment_id = @assessmentId
                        AND edunest_id  = @eduNestId
                    RETURNING edunest_id, 'UPDATED' as action
                ),
                ins AS (
                    INSERT INTO student_marks (id, assessment_id, edunest_id, marks_obtained, grade_awarded, remarks, created_at, updated_at)
                    SELECT uuid_generate_v4(), @assessmentId, @eduNestId, @marksObtained, @gradeAwarded, @remarks, now(), now()
                    WHERE NOT EXISTS (SELECT 1 FROM upd)
                    RETURNING edunest_id, 'INSERTED' as action
                )
                SELECT * FROM upd UNION ALL SELECT * FROM ins";

                int processedCount = 0;
                foreach (var studentMark in studentMarks)
                {
                    await using var cmd = new NpgsqlCommand(sql, conn, transaction);
                    cmd.Parameters.Add("@assessmentId", NpgsqlDbType.Uuid).Value = studentMark.AssessmentId;
                    cmd.Parameters.Add("@eduNestId", NpgsqlDbType.Text).Value = studentMark.EduNestId;
                    cmd.Parameters.Add("@marksObtained", NpgsqlDbType.Numeric).Value = studentMark.MarksObtained ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@gradeAwarded", NpgsqlDbType.Text).Value = studentMark.GradeAwarded ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@remarks", NpgsqlDbType.Text).Value = studentMark.Remarks ?? (object)DBNull.Value;

                    await using var reader = await cmd.ExecuteReaderAsync();
                    bool hasResult = false;
                    while (await reader.ReadAsync())
                    {
                        hasResult = true;
                        var action = reader.GetString(reader.GetOrdinal("action"));
                        var eduNestId = reader.GetString(reader.GetOrdinal("edunest_id"));
                        Console.WriteLine($"Student {eduNestId}: {action}");
                    }
                    
                    if (hasResult) processedCount++;
                }

                await transaction.CommitAsync();
                Console.WriteLine($"Bulk upsert completed: {processedCount}/{studentMarks.Count} records processed");
                return processedCount > 0;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<StudentMark>> GetByAssessmentIdAsync(Guid assessmentId)
        {
            await using var conn = await _dataSource.OpenConnectionAsync();

            const string sql = @"
                SELECT id, assessment_id, edunest_id, marks_obtained, grade_awarded, remarks, created_at, updated_at
                FROM student_marks 
                WHERE assessment_id = @assessmentId
                ORDER BY edunest_id";

            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add("@assessmentId", NpgsqlDbType.Uuid).Value = assessmentId;

            await using var reader = await cmd.ExecuteReaderAsync();
            var marks = new List<StudentMark>();

            while (await reader.ReadAsync())
            {
                marks.Add(new StudentMark
                {
                    Id = reader.GetGuid(reader.GetOrdinal("id")),
                    AssessmentId = reader.GetGuid(reader.GetOrdinal("assessment_id")),
                    EduNestId = reader.GetString(reader.GetOrdinal("edunest_id")),
                    MarksObtained = reader.IsDBNull(reader.GetOrdinal("marks_obtained")) ? null : reader.GetDecimal(reader.GetOrdinal("marks_obtained")),
                    GradeAwarded = reader.IsDBNull(reader.GetOrdinal("grade_awarded")) ? null : reader.GetString(reader.GetOrdinal("grade_awarded")),
                    Remarks = reader.IsDBNull(reader.GetOrdinal("remarks")) ? null : reader.GetString(reader.GetOrdinal("remarks")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                });
            }

            return marks;
        }

        public async Task<List<StudentMark>> GetByStudentIdAsync(string eduNestId, string academicYear)
        {

              var sqlBuilder = new StringBuilder(@"
                SELECT 
                    edunest_id,
                    first_name,
                    last_name,
                    grade,
                    section,
                    academic_year,
                    subject_name,
                    assessment_name,
                    grading_type,
                    max_marks,
                    marks_obtained,
                    grade_awarded,
                    remarks,
                    assessment_date,
                    mark_created_at,
                    mark_updated_at,
                    assessment_id
                FROM v_student_marks_academic_year 
                WHERE 1=1");

            var parameters = new List<NpgsqlParameter>();
            

            // Optional filters
            if (!string.IsNullOrWhiteSpace(academicYear))
            {
                sqlBuilder.Append(" AND academic_year = @academicYear");
                parameters.Add(new NpgsqlParameter("@academicYear", NpgsqlDbType.Text) { Value = academicYear });
            }

            if (!string.IsNullOrWhiteSpace(eduNestId))
            {
                sqlBuilder.Append(" AND edunest_id = @eduNestId");
                parameters.Add(new NpgsqlParameter("@eduNestId", NpgsqlDbType.Text) { Value = eduNestId });
            }

            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(sqlBuilder.ToString(), conn);
            foreach (var param in parameters)
                cmd.Parameters.Add(param);

            await using var reader = await cmd.ExecuteReaderAsync();
            var marks = new List<StudentMark>();
            while (await reader.ReadAsync())
            {
                marks.Add(new StudentMark
                {
                    EduNestId = reader.GetString(reader.GetOrdinal("edunest_id")),
                    SubjectName = reader.GetString(reader.GetOrdinal("subject_name")),
                    AssessmentName = reader.GetString(reader.GetOrdinal("assessment_name")),
                    MaxMarks = reader.GetDecimal(reader.GetOrdinal("max_marks")),
                    MarksObtained = reader.IsDBNull(reader.GetOrdinal("marks_obtained")) ? null : reader.GetDecimal(reader.GetOrdinal("marks_obtained")),
                    GradeAwarded = reader.IsDBNull(reader.GetOrdinal("grade_awarded")) ? null : reader.GetString(reader.GetOrdinal("grade_awarded")),
                    Remarks = reader.IsDBNull(reader.GetOrdinal("remarks")) ? null : reader.GetString(reader.GetOrdinal("remarks")),
                });
            }

            return marks;
        }

        public async Task<StudentMark?> GetByAssessmentAndStudentAsync(Guid assessmentId, string eduNestId)
        {
            await using var conn = await _dataSource.OpenConnectionAsync();

            const string sql = @"
                SELECT id, assessment_id, edunest_id, marks_obtained, grade_awarded, remarks, created_at, updated_at
                FROM student_marks 
                WHERE assessment_id = @assessmentId AND edunest_id = @eduNestId";

            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add("@assessmentId", NpgsqlDbType.Uuid).Value = assessmentId;
            cmd.Parameters.Add("@eduNestId", NpgsqlDbType.Text).Value = eduNestId;

            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new StudentMark
                {
                    Id = reader.GetGuid(reader.GetOrdinal("id")),
                    AssessmentId = reader.GetGuid(reader.GetOrdinal("assessment_id")),
                    EduNestId = reader.GetString(reader.GetOrdinal("edunest_id")),
                    MarksObtained = reader.IsDBNull(reader.GetOrdinal("marks_obtained")) ? null : reader.GetDecimal(reader.GetOrdinal("marks_obtained")),
                    GradeAwarded = reader.IsDBNull(reader.GetOrdinal("grade_awarded")) ? null : reader.GetString(reader.GetOrdinal("grade_awarded")),
                    Remarks = reader.IsDBNull(reader.GetOrdinal("remarks")) ? null : reader.GetString(reader.GetOrdinal("remarks")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                };
            }

            return null;
        }

        public async Task<bool?> DeleteAsync(Guid id)
        {
            await using var conn = await _dataSource.OpenConnectionAsync();

            const string sql = "DELETE FROM student_marks WHERE id = @id";

            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add("@id", NpgsqlDbType.Uuid).Value = id;

            var result = await cmd.ExecuteNonQueryAsync();
            return result > 0;
        }

        public async Task<List<StudentMark>> GetAllAsync()
        {
            await using var conn = await _dataSource.OpenConnectionAsync();

            const string sql = @"
                SELECT id, assessment_id, edunest_id, marks_obtained, grade_awarded, remarks, created_at, updated_at
                FROM student_marks
                ORDER BY created_at DESC";

            await using var cmd = new NpgsqlCommand(sql, conn);

            await using var reader = await cmd.ExecuteReaderAsync();
            var marks = new List<StudentMark>();

            while (await reader.ReadAsync())
            {
                marks.Add(new StudentMark
                {
                    Id = reader.GetGuid(reader.GetOrdinal("id")),
                    AssessmentId = reader.GetGuid(reader.GetOrdinal("assessment_id")),
                    EduNestId = reader.GetString(reader.GetOrdinal("edunest_id")),
                    MarksObtained = reader.IsDBNull(reader.GetOrdinal("marks_obtained")) ? null : reader.GetDecimal(reader.GetOrdinal("marks_obtained")),
                    GradeAwarded = reader.IsDBNull(reader.GetOrdinal("grade_awarded")) ? null : reader.GetString(reader.GetOrdinal("grade_awarded")),
                    Remarks = reader.IsDBNull(reader.GetOrdinal("remarks")) ? null : reader.GetString(reader.GetOrdinal("remarks")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                });
            }

            return marks;
        }
    }
}