using EduNestERP.Persistence.Entities;
using Npgsql;
using NpgsqlTypes;
using Microsoft.Extensions.Logging;

namespace EduNestERP.Persistence.Repositories
{
    public class HomeworkRepository : IHomeworkRepository
    {
        private readonly ITenantDataSourceProvider _dataSource;
        private readonly ILogger<HomeworkRepository> _logger;

        public HomeworkRepository(ITenantDataSourceProvider dataSource, ILogger<HomeworkRepository> logger)
        {
            _dataSource = dataSource;
            _logger = logger;
        }

        public async Task<bool?> AddAsync(Homework homework)
        {
            try
            {
                await using var conn = await _dataSource.OpenConnectionAsync();
                const string sql = @"INSERT INTO homework (id, class_subject_id, created_by_id, assigned_date, due_date, details, created_at, updated_at)
                                     VALUES (@id, @classSubjectId, @createdById, @assignedDate, @dueDate, @details, @createdAt, @updatedAt)";
                await using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.Add("@id", NpgsqlDbType.Uuid).Value = homework.Id == Guid.Empty ? Guid.NewGuid() : homework.Id;
                cmd.Parameters.Add("@classSubjectId", NpgsqlDbType.Uuid).Value = homework.ClassSubjectId;
                cmd.Parameters.Add("@createdById", NpgsqlDbType.Uuid).Value = homework.CreatedById;
                cmd.Parameters.Add("@assignedDate", NpgsqlDbType.Date).Value = homework.AssignedDate;
                cmd.Parameters.Add("@dueDate", NpgsqlDbType.Date).Value = homework.DueDate;
                cmd.Parameters.Add("@details", NpgsqlDbType.Text).Value = homework.Details;
                cmd.Parameters.Add("@createdAt", NpgsqlDbType.TimestampTz).Value = homework.CreatedAt == default ? DateTime.UtcNow : homework.CreatedAt;
                cmd.Parameters.Add("@updatedAt", NpgsqlDbType.TimestampTz).Value = homework.UpdatedAt == default ? DateTime.UtcNow : homework.UpdatedAt;
                var result = await cmd.ExecuteNonQueryAsync();
                _logger.LogInformation("Homework added successfully: {@Homework}", homework);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding homework: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Homework homework)
        {
            try
            {
                await using var conn = await _dataSource.OpenConnectionAsync();
                const string sql = @"UPDATE homework SET class_subject_id = @classSubjectId, created_by_id = @createdById, assigned_date = @assignedDate, due_date = @dueDate, details = @details, updated_at = @updatedAt WHERE id = @id";
                await using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.Add("@id", NpgsqlDbType.Uuid).Value = homework.Id;
                cmd.Parameters.Add("@classSubjectId", NpgsqlDbType.Uuid).Value = homework.ClassSubjectId;
                cmd.Parameters.Add("@createdById", NpgsqlDbType.Uuid).Value = homework.CreatedById;
                cmd.Parameters.Add("@assignedDate", NpgsqlDbType.Date).Value = homework.AssignedDate;
                cmd.Parameters.Add("@dueDate", NpgsqlDbType.Date).Value = homework.DueDate;
                cmd.Parameters.Add("@details", NpgsqlDbType.Text).Value = homework.Details;
                cmd.Parameters.Add("@updatedAt", NpgsqlDbType.TimestampTz).Value = DateTime.UtcNow;
                var result = await cmd.ExecuteNonQueryAsync();
                _logger.LogInformation("Homework updated successfully: {@Homework}", homework);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating homework: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<Homework?> GetByIdAsync(Guid id)
        {
            try
            {
                await using var conn = await _dataSource.OpenConnectionAsync();
                const string sql = @"SELECT id, class_subject_id, created_by_id, assigned_date, due_date, details, created_at, updated_at FROM homework WHERE id = @id";
                await using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.Add("@id", NpgsqlDbType.Uuid).Value = id;
                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var hw = new Homework
                    {
                        Id = reader.GetGuid(reader.GetOrdinal("id")),
                        ClassSubjectId = reader.GetGuid(reader.GetOrdinal("class_subject_id")),
                        CreatedById = reader.GetGuid(reader.GetOrdinal("created_by_id")),
                        AssignedDate = reader.GetDateTime(reader.GetOrdinal("assigned_date")),
                        DueDate = reader.GetDateTime(reader.GetOrdinal("due_date")),
                        Details = reader.GetString(reader.GetOrdinal("details")),
                        CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                        UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                    };
                    _logger.LogInformation("Fetched homework by id: {Id}", id);
                    return hw;
                }
                _logger.LogInformation("No homework found for id: {Id}", id);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching homework by id: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<List<Homework>> GetAllAsync(string? grade = null, string? section = null)
        {
            try
            {
                await using var conn = await _dataSource.OpenConnectionAsync();
                var sql = @"SELECT h.id, h.class_subject_id, h.created_by_id, h.assigned_date, h.due_date, h.details, h.created_at, h.updated_at
                            FROM homework h
                            JOIN class_subject_view v ON h.class_subject_id = v.class_subject_id
                            WHERE (COALESCE(@grade, '') = '' OR v.grade = @grade)
                              AND (COALESCE(@section, '') = '' OR v.section = @section)
                            ORDER BY h.created_at DESC";
                await using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@grade", (object?)grade ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@section", (object?)section ?? DBNull.Value);
                await using var reader = await cmd.ExecuteReaderAsync();
                var homeworks = new List<Homework>();
                while (await reader.ReadAsync())
                {
                    homeworks.Add(new Homework
                    {
                        Id = reader.GetGuid(reader.GetOrdinal("id")),
                        ClassSubjectId = reader.GetGuid(reader.GetOrdinal("class_subject_id")),
                        CreatedById = reader.GetGuid(reader.GetOrdinal("created_by_id")),
                        AssignedDate = reader.GetDateTime(reader.GetOrdinal("assigned_date")),
                        DueDate = reader.GetDateTime(reader.GetOrdinal("due_date")),
                        Details = reader.GetString(reader.GetOrdinal("details")),
                        CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                        UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                    });
                }
                _logger.LogInformation("Fetched {Count} homework records for grade={Grade}, section={Section}", homeworks.Count, grade, section);
                return homeworks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching homework list: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<List<Homework>> GetByClassSubjectIdAsync(Guid classSubjectId)
        {
            await using var conn = await _dataSource.OpenConnectionAsync();
            const string sql = @"SELECT id, class_subject_id, created_by_id, assigned_date, due_date, details, created_at, updated_at FROM homework WHERE class_subject_id = @classSubjectId ORDER BY created_at DESC";
            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add("@classSubjectId", NpgsqlDbType.Uuid).Value = classSubjectId;
            await using var reader = await cmd.ExecuteReaderAsync();
            var homeworks = new List<Homework>();
            while (await reader.ReadAsync())
            {
                homeworks.Add(new Homework
                {
                    Id = reader.GetGuid(reader.GetOrdinal("id")),
                    ClassSubjectId = reader.GetGuid(reader.GetOrdinal("class_subject_id")),
                    CreatedById = reader.GetGuid(reader.GetOrdinal("created_by_id")),
                    AssignedDate = reader.GetDateTime(reader.GetOrdinal("assigned_date")),
                    DueDate = reader.GetDateTime(reader.GetOrdinal("due_date")),
                    Details = reader.GetString(reader.GetOrdinal("details")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                });
            }
            return homeworks;
        }
    }
}
