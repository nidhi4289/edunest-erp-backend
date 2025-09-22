using EduNestERP.Persistence.Entities;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Data;

namespace EduNestERP.Persistence.Repositories
{
    public class FeeAdminRepository : Repository<FeeAdmin>, IFeeAdminRepository
    {
        private readonly ILogger<FeeAdminRepository> _logger;

        public FeeAdminRepository(ITenantDataSourceProvider dataSource, ILogger<FeeAdminRepository> logger)
            : base(dataSource)
        {
            _logger = logger;
        }

        public override async Task<bool?> AddAsync(FeeAdmin feeAdmin)
        {
            try
            {
                const string sql = @"
                    INSERT INTO fee_admin (id, class_id, academic_year, monthly_fee, annual_fee, admission_fee, 
                                         transport_fee, library_fee, sports_fee, miscellaneous_fee, is_active, created_at, updated_at)
                    VALUES (@Id, @ClassId, @AcademicYear, @MonthlyFee, @AnnualFee, @AdmissionFee, 
                           @TransportFee, @LibraryFee, @SportsFee, @MiscellaneousFee, @IsActive, @CreatedAt, @UpdatedAt)";

                await using var connection = await _dataSource.OpenConnectionAsync();

                using var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", feeAdmin.Id);
                command.Parameters.AddWithValue("@ClassId", feeAdmin.ClassId);
                command.Parameters.AddWithValue("@AcademicYear", (object?)feeAdmin.AcademicYear ?? DBNull.Value);
                command.Parameters.AddWithValue("@MonthlyFee", (object?)feeAdmin.MonthlyFee ?? DBNull.Value);
                command.Parameters.AddWithValue("@AnnualFee", (object?)feeAdmin.AnnualFee ?? DBNull.Value);
                command.Parameters.AddWithValue("@AdmissionFee", (object?)feeAdmin.AdmissionFee ?? DBNull.Value);
                command.Parameters.AddWithValue("@TransportFee", (object?)feeAdmin.TransportFee ?? DBNull.Value);
                command.Parameters.AddWithValue("@LibraryFee", (object?)feeAdmin.LibraryFee ?? DBNull.Value);
                command.Parameters.AddWithValue("@SportsFee", (object?)feeAdmin.SportsFee ?? DBNull.Value);
                command.Parameters.AddWithValue("@MiscellaneousFee", (object?)feeAdmin.MiscellaneousFee ?? DBNull.Value);
                command.Parameters.AddWithValue("@IsActive", (object?)feeAdmin.IsActive ?? DBNull.Value);
                command.Parameters.AddWithValue("@CreatedAt", (object?)feeAdmin.CreatedAt ?? DBNull.Value);
                command.Parameters.AddWithValue("@UpdatedAt", (object?)feeAdmin.UpdatedAt ?? DBNull.Value);

                var result = await command.ExecuteNonQueryAsync();
                var success = result > 0;
                
                _logger.LogInformation("AddAsync - Fee admin added. Success: {Success}, ID: {Id}", success, feeAdmin.Id);
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddAsync - Error adding fee admin with ID: {Id}", feeAdmin.Id);
                return null;
            }
        }

        public override async Task<bool?> UpdateAsync(FeeAdmin feeAdmin)
        {
            try
            {
                const string sql = @"
                    UPDATE fee_admin 
                    SET class_id = @ClassId, academic_year = @AcademicYear, monthly_fee = @MonthlyFee, 
                        annual_fee = @AnnualFee, admission_fee = @AdmissionFee, transport_fee = @TransportFee, 
                        library_fee = @LibraryFee, sports_fee = @SportsFee, miscellaneous_fee = @MiscellaneousFee, 
                        is_active = @IsActive, updated_at = @UpdatedAt
                    WHERE id = @Id";

                await using var connection = await _dataSource.OpenConnectionAsync();

                using var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", feeAdmin.Id);
                command.Parameters.AddWithValue("@ClassId", feeAdmin.ClassId);
                command.Parameters.AddWithValue("@AcademicYear", (object?)feeAdmin.AcademicYear ?? DBNull.Value);
                command.Parameters.AddWithValue("@MonthlyFee", (object?)feeAdmin.MonthlyFee ?? DBNull.Value);
                command.Parameters.AddWithValue("@AnnualFee", (object?)feeAdmin.AnnualFee ?? DBNull.Value);
                command.Parameters.AddWithValue("@AdmissionFee", (object?)feeAdmin.AdmissionFee ?? DBNull.Value);
                command.Parameters.AddWithValue("@TransportFee", (object?)feeAdmin.TransportFee ?? DBNull.Value);
                command.Parameters.AddWithValue("@LibraryFee", (object?)feeAdmin.LibraryFee ?? DBNull.Value);
                command.Parameters.AddWithValue("@SportsFee", (object?)feeAdmin.SportsFee ?? DBNull.Value);
                command.Parameters.AddWithValue("@MiscellaneousFee", (object?)feeAdmin.MiscellaneousFee ?? DBNull.Value);
                command.Parameters.AddWithValue("@IsActive", (object?)feeAdmin.IsActive ?? DBNull.Value);
                command.Parameters.AddWithValue("@UpdatedAt", (object?)feeAdmin.UpdatedAt ?? DBNull.Value);

                var result = await command.ExecuteNonQueryAsync();
                var success = result > 0;
                
                _logger.LogInformation("UpdateAsync - Fee admin updated. Success: {Success}, ID: {Id}", success, feeAdmin.Id);
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync - Error updating fee admin with ID: {Id}", feeAdmin.Id);
                return null;
            }
        }

        public override async Task<FeeAdmin?> GetByIdAsync(Guid id)
        {
            try
            {
                const string sql = @"
                    SELECT id, class_id, academic_year, monthly_fee, annual_fee, admission_fee, 
                           transport_fee, library_fee, sports_fee, miscellaneous_fee, is_active, created_at, updated_at
                    FROM fee_admin 
                    WHERE id = @Id";

                await using var connection = await _dataSource.OpenConnectionAsync();

                using var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", id);

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var feeAdmin = MapReaderToFeeAdmin(reader);
                    _logger.LogInformation("GetByIdAsync - Fee admin found. ID: {Id}", id);
                    return feeAdmin;
                }

                _logger.LogInformation("GetByIdAsync - Fee admin not found. ID: {Id}", id);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByIdAsync - Error getting fee admin with ID: {Id}", id);
                return null;
            }
        }

        public async Task<List<FeeAdmin>> GetAllAsync(string? academicYear = null, bool? isActive = null)
        {
            try
            {
                var sql = @"
                    SELECT id, class_id, academic_year, monthly_fee, annual_fee, admission_fee, 
                           transport_fee, library_fee, sports_fee, miscellaneous_fee, is_active, created_at, updated_at
                    FROM fee_admin 
                    WHERE 1=1";

                var conditions = new List<string>();
                if (!string.IsNullOrEmpty(academicYear))
                    conditions.Add("academic_year = @AcademicYear");
                if (isActive.HasValue)
                    conditions.Add("is_active = @IsActive");

                if (conditions.Any())
                    sql += " AND " + string.Join(" AND ", conditions);

                sql += " ORDER BY academic_year DESC, created_at DESC";

                await using var connection = await _dataSource.OpenConnectionAsync();

                using var command = new NpgsqlCommand(sql, connection);
                if (!string.IsNullOrEmpty(academicYear))
                    command.Parameters.AddWithValue("@AcademicYear", academicYear);
                if (isActive.HasValue)
                    command.Parameters.AddWithValue("@IsActive", isActive.Value);

                var feeAdmins = new List<FeeAdmin>();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    feeAdmins.Add(MapReaderToFeeAdmin(reader));
                }

                _logger.LogInformation("GetAllAsync - Retrieved {Count} fee admin records", feeAdmins.Count);
                return feeAdmins;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllAsync - Error getting fee admin records");
                return new List<FeeAdmin>();
            }
        }

        public async Task<FeeAdmin?> GetByClassAndYearAsync(Guid classId, string academicYear)
        {
            try
            {
                const string sql = @"
                    SELECT id, class_id, academic_year, monthly_fee, annual_fee, admission_fee, 
                           transport_fee, library_fee, sports_fee, miscellaneous_fee, is_active, created_at, updated_at
                    FROM fee_admin 
                    WHERE class_id = @ClassId AND academic_year = @AcademicYear";

                await using var connection = await _dataSource.OpenConnectionAsync();
                
                // Log connection details for debugging
                _logger.LogInformation("GetByClassAndYearAsync - Database: {Database}, ClassId: {ClassId}, AcademicYear: {AcademicYear}", 
                    connection.Database, classId, academicYear);

                using var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@ClassId", classId);
                command.Parameters.AddWithValue("@AcademicYear", academicYear);

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var feeAdmin = MapReaderToFeeAdmin(reader);
                    _logger.LogInformation("GetByClassAndYearAsync - Fee admin found. ClassId: {ClassId}, Year: {AcademicYear}", classId, academicYear);
                    return feeAdmin;
                }

                _logger.LogInformation("GetByClassAndYearAsync - Fee admin not found. ClassId: {ClassId}, Year: {AcademicYear}", classId, academicYear);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByClassAndYearAsync - Error getting fee admin. ClassId: {ClassId}, Year: {AcademicYear}", classId, academicYear);
                return null;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                const string sql = "DELETE FROM fee_admin WHERE id = @Id";

                await using var connection = await _dataSource.OpenConnectionAsync();

                using var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", id);

                var result = await command.ExecuteNonQueryAsync();
                var success = result > 0;
                
                _logger.LogInformation("DeleteAsync - Fee admin deleted. Success: {Success}, ID: {Id}", success, id);
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteAsync - Error deleting fee admin with ID: {Id}", id);
                return false;
            }
        }

        public async Task<List<FeeAdmin>> GetByAcademicYearAsync(string academicYear)
        {
            try
            {
                const string sql = @"
                    SELECT id, class_id, academic_year, monthly_fee, annual_fee, admission_fee, 
                           transport_fee, library_fee, sports_fee, miscellaneous_fee, is_active, created_at, updated_at
                    FROM fee_admin 
                    WHERE academic_year = @AcademicYear
                    ORDER BY created_at DESC";

                await using var connection = await _dataSource.OpenConnectionAsync();

                using var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@AcademicYear", academicYear);

                var feeAdmins = new List<FeeAdmin>();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    feeAdmins.Add(MapReaderToFeeAdmin(reader));
                }

                _logger.LogInformation("GetByAcademicYearAsync - Retrieved {Count} fee admin records for year {AcademicYear}", feeAdmins.Count, academicYear);
                return feeAdmins;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByAcademicYearAsync - Error getting fee admin records for year {AcademicYear}", academicYear);
                return new List<FeeAdmin>();
            }
        }

        // Temporary debugging method - remove after testing
        public async Task<string> GetConnectionInfoAsync()
        {
            try
            {
                await using var connection = await _dataSource.OpenConnectionAsync();
                return $"Database: {connection.Database}, Host: {connection.Host}, Port: {connection.Port}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetConnectionInfoAsync - Error getting connection info");
                return $"Error: {ex.Message}";
            }
        }

        private static FeeAdmin MapReaderToFeeAdmin(IDataReader reader)
        {
            return new FeeAdmin
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                ClassId = reader.GetGuid(reader.GetOrdinal("class_id")),
                AcademicYear = reader.IsDBNull(reader.GetOrdinal("academic_year")) ? null : reader.GetString(reader.GetOrdinal("academic_year")),
                MonthlyFee = reader.IsDBNull(reader.GetOrdinal("monthly_fee")) ? null : reader.GetDecimal(reader.GetOrdinal("monthly_fee")),
                AnnualFee = reader.IsDBNull(reader.GetOrdinal("annual_fee")) ? null : reader.GetDecimal(reader.GetOrdinal("annual_fee")),
                AdmissionFee = reader.IsDBNull(reader.GetOrdinal("admission_fee")) ? null : reader.GetDecimal(reader.GetOrdinal("admission_fee")),
                TransportFee = reader.IsDBNull(reader.GetOrdinal("transport_fee")) ? null : reader.GetDecimal(reader.GetOrdinal("transport_fee")),
                LibraryFee = reader.IsDBNull(reader.GetOrdinal("library_fee")) ? null : reader.GetDecimal(reader.GetOrdinal("library_fee")),
                SportsFee = reader.IsDBNull(reader.GetOrdinal("sports_fee")) ? null : reader.GetDecimal(reader.GetOrdinal("sports_fee")),
                MiscellaneousFee = reader.IsDBNull(reader.GetOrdinal("miscellaneous_fee")) ? null : reader.GetDecimal(reader.GetOrdinal("miscellaneous_fee")),
                IsActive = reader.IsDBNull(reader.GetOrdinal("is_active")) ? null : reader.GetBoolean(reader.GetOrdinal("is_active")),
                CreatedAt = reader.IsDBNull(reader.GetOrdinal("created_at")) ? null : reader.GetDateTime(reader.GetOrdinal("created_at")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("updated_at")) ? null : reader.GetDateTime(reader.GetOrdinal("updated_at"))
            };
        }
    }
}