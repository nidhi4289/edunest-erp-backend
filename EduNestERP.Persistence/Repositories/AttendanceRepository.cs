using EduNestERP.Persistence.Entities;
using Npgsql;
using NpgsqlTypes;
using Microsoft.Extensions.Logging;

namespace EduNestERP.Persistence.Repositories
{
    public class AttendanceRepository : Repository<Attendance>, IAttendanceRepository
    {
        private readonly IMasterDataRepository _masterDataRepository;
        private readonly ILogger<AttendanceRepository> _logger;

        public AttendanceRepository(ITenantDataSourceProvider dataSource, IMasterDataRepository masterDataRepository, ILogger<AttendanceRepository> logger)
            : base(dataSource)
        {
            _masterDataRepository = masterDataRepository;
            _logger = logger;
        }

        public async Task<bool> MarkAttendanceAsync(List<Attendance> attendance)
        {
            if (attendance == null || attendance.Count == 0)
                return false;

            var classes = await _masterDataRepository.GetAllClassesAsync();

            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var tx = await conn.BeginTransactionAsync();

            try
            {
                int successCount = 0;

                const string sql = @"
                WITH upd AS (
                UPDATE attendance
                    SET class_id    = @class_id,
                        is_present  = @is_present,
                        modified_at = now()
                WHERE edunest_id = @edunest_id
                    AND date       = @date
                RETURNING 1
                )
                INSERT INTO attendance (id, edunest_id, class_id, date, is_present, created_at, modified_at)
                SELECT uuid_generate_v4(), @edunest_id, @class_id, @date, @is_present, now(), now()
                WHERE NOT EXISTS (SELECT 1 FROM upd);";

                foreach (var att in attendance)
                {
                    var classEntity = classes.FirstOrDefault(c => c.Grade == att.Grade && c.Section == att.Section);
                    if (classEntity?.Id == null)
                    {
                        _logger.LogWarning("Skipping attendance - Class not found for Grade:{Grade}, Section:{Section}", att.Grade, att.Section);
                        continue;
                    }

                    await using var cmd = new Npgsql.NpgsqlCommand(sql, conn, tx);
                    cmd.Parameters.AddWithValue("@edunest_id", NpgsqlTypes.NpgsqlDbType.Text, att.StudentId);
                    cmd.Parameters.AddWithValue("@class_id", NpgsqlTypes.NpgsqlDbType.Uuid, classEntity.Id);
                    cmd.Parameters.AddWithValue("@date", NpgsqlTypes.NpgsqlDbType.Date, att.Date.Date);
                    cmd.Parameters.AddWithValue("@is_present", NpgsqlTypes.NpgsqlDbType.Boolean, att.IsPresent);

                    successCount += await cmd.ExecuteNonQueryAsync() > 0 ? 1 : 0;
                }

                await tx.CommitAsync();
                _logger.LogInformation("Attendance processed: {SuccessCount}/{Total} records successful", successCount, attendance.Count);
                return successCount > 0;
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Error in MarkAttendanceAsync: {Message}", ex.Message);
                return false;
            }
        }

        public async Task<List<Attendance>> GetAttendanceByStudentAsync(string eduNestId, string grade, string section)
        {
            if (string.IsNullOrWhiteSpace(eduNestId) || string.IsNullOrWhiteSpace(grade) || string.IsNullOrWhiteSpace(section))
                return null;

            try
            {
                await using var conn = await _dataSource.OpenConnectionAsync();

                const string sql = @"
                    SELECT a.*
                    FROM attendance a
                    INNER JOIN classes c ON a.class_id = c.id
                    WHERE edunest_id = @edunest_id
                      AND c.grade = @grade
                      AND c.section = @section";

                await using var cmd = new Npgsql.NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@edunest_id", NpgsqlTypes.NpgsqlDbType.Text, eduNestId);
                cmd.Parameters.AddWithValue("@grade", NpgsqlTypes.NpgsqlDbType.Text, grade);
                cmd.Parameters.AddWithValue("@section", NpgsqlTypes.NpgsqlDbType.Text, section);

                var attendance = new List<Attendance>();
                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    attendance.Add(new Attendance
                    {
                        Id = reader.GetGuid(reader.GetOrdinal("id")),
                        ClassId = reader.GetGuid(reader.GetOrdinal("class_id")),
                        Date = reader.GetDateTime(reader.GetOrdinal("date")),
                        IsPresent = reader.GetBoolean(reader.GetOrdinal("is_present"))
                    });
                }
                _logger.LogInformation("Fetched {Count} attendance records for student {EduNestId}, grade={Grade}, section={Section}", attendance.Count, eduNestId, grade, section);
                return attendance;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching attendance by student: {Message}", ex.Message);
                throw;
            }
        }

    }

}