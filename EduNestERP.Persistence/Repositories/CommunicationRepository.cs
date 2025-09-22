using EduNestERP.Persistence.Entities;
using Npgsql;
using NpgsqlTypes;

namespace EduNestERP.Persistence.Repositories
{
    public class CommunicationRepository : ICommunicationRepository
    {
        private readonly ITenantDataSourceProvider _dataSource;

        public CommunicationRepository(ITenantDataSourceProvider dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<bool> AddAsync(Communication communication)
        {
             await using var conn = await _dataSource.OpenConnectionAsync();
          
            // Now insert into communications
            const string sql = @"
            INSERT INTO communications (id, title, type, content, status, created_by, created_at, modified_at)
            VALUES (@id, @title, @type, @content, @status, @created_by, now(), now())";
                communication.Id = Guid.NewGuid();

            await using (var cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Uuid, communication.Id);
                cmd.Parameters.AddWithValue("@title", NpgsqlDbType.Varchar, communication.Title);
                cmd.Parameters.AddWithValue("@type", NpgsqlDbType.Varchar, communication.Type);
                cmd.Parameters.AddWithValue("@content", NpgsqlDbType.Text, communication.Content);
                cmd.Parameters.AddWithValue("@status", NpgsqlDbType.Varchar, communication.Status ?? "Active");
                cmd.Parameters.AddWithValue("@created_by", NpgsqlDbType.Uuid, communication.CreatedBy);

                var result = await cmd.ExecuteNonQueryAsync();
                return result > 0;
            }
        }

       public async Task<bool> UpdateAsync(Communication communication)
        {
         
                await using var conn = await _dataSource.OpenConnectionAsync();
            
                // Now update communications with the found modified_by id
                const string sql = @"
                    UPDATE communications
                    SET title = @title,
                        type = @type,
                        content = @content,
                        status = @status,
                        modified_by = @modified_by,
                        modified_at = now()
                    WHERE id = @id";
                await using var conn2 = await _dataSource.OpenConnectionAsync();
                await using var cmd = new NpgsqlCommand(sql, conn2);
                cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Uuid, communication.Id);
                cmd.Parameters.AddWithValue("@title", NpgsqlDbType.Varchar, communication.Title);
                cmd.Parameters.AddWithValue("@type", NpgsqlDbType.Varchar, communication.Type);
                cmd.Parameters.AddWithValue("@content", NpgsqlDbType.Text, communication.Content);
                cmd.Parameters.AddWithValue("@status", NpgsqlDbType.Varchar, communication.Status ?? "Active");
                cmd.Parameters.AddWithValue("@modified_by", NpgsqlDbType.Uuid, communication.ModifiedBy ?? (object)DBNull.Value);

                var result = await cmd.ExecuteNonQueryAsync();
                return result > 0;
           
        }
        
        public async Task<Communication?> GetByIdAsync(Guid id)
        {
            const string sql = @"SELECT * FROM communications WHERE id = @id";
            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Uuid, id);

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return Map(reader);
            }
            return null;
        }

        public async Task<List<Communication>> GetAllAsync(DateTime? modifiedAfter = null, string? status = null)
        {
            var sql = @"
                SELECT c.id, c.title, c.type, c.content, c.status, u.user_id AS created_by, 
                    c.modified_by, c.created_at, c.modified_at
                FROM communications c
                INNER JOIN users u ON c.created_by = u.id";
            if (modifiedAfter.HasValue)
            {
                sql += " WHERE c.modified_at > @modifiedAfter";
            }
            if (!string.IsNullOrEmpty(status))
            {
                sql += modifiedAfter.HasValue ? " AND" : " WHERE";
                sql += " c.status = @status";
            }
            sql += " ORDER BY c.created_at DESC";

            var list = new List<Communication>();
            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(sql, conn);
            if (modifiedAfter.HasValue)
            {
                cmd.Parameters.AddWithValue("@modifiedAfter", NpgsqlDbType.TimestampTz, modifiedAfter.Value);
            }
            if (!string.IsNullOrEmpty(status))
            {
                cmd.Parameters.AddWithValue("@status", NpgsqlDbType.Varchar, status);
            }
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new Communication
                {
                    Id = reader.GetGuid(reader.GetOrdinal("id")),
                    Title = reader.GetString(reader.GetOrdinal("title")),
                    Type = reader.GetString(reader.GetOrdinal("type")),
                    Content = reader.GetString(reader.GetOrdinal("content")),
                    Status = reader.GetString(reader.GetOrdinal("status")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                    ModifiedAt = reader.GetDateTime(reader.GetOrdinal("modified_at")),
                    });
            }
            return list;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            const string sql = @"DELETE FROM communications WHERE id = @id";
            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Uuid, id);

            var result = await cmd.ExecuteNonQueryAsync();
            return result > 0;
        }

        private Communication Map(NpgsqlDataReader reader)
        {
            return new Communication
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                Title = reader.GetString(reader.GetOrdinal("title")),
                Type = reader.GetString(reader.GetOrdinal("type")),
                Content = reader.GetString(reader.GetOrdinal("content")),
                Status = reader.GetString(reader.GetOrdinal("status")),

            };
        }
    }
}