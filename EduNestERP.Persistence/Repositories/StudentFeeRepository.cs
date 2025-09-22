using EduNestERP.Persistence.Entities;
using Npgsql;

namespace EduNestERP.Persistence.Repositories
{
    public class StudentFeeRepository : Repository<StudentFee>, IStudentFeeRepository
    {
        public StudentFeeRepository(ITenantDataSourceProvider dataSource)
          : base(dataSource)
        {
        }

        public async Task<bool?> AddBulkStudentFeesAsync(IEnumerable<StudentFee> studentFees)
        {
            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var transaction = await conn.BeginTransactionAsync();

            try
            {
                var tasks = studentFees.Select(studentFee => InsertStudentFeeAsync(studentFee, conn, transaction));
                var results = await Task.WhenAll(tasks);
                await transaction.CommitAsync();
                return results.All(r => r > 0);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public override async Task<bool?> AddAsync(StudentFee studentFee)
        {
            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var transaction = await conn.BeginTransactionAsync();
            try
            {
                var rowsAffected = await InsertStudentFeeAsync(studentFee, conn, transaction);
                await transaction.CommitAsync();
                return rowsAffected > 0;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public override async Task<bool?> UpdateAsync(StudentFee studentFee)
        {
            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var transaction = await conn.BeginTransactionAsync();
            try
            {
                var rowsAffected = await UpdateStudentFeeAsync(studentFee.Id, studentFee, conn, transaction);
                await transaction.CommitAsync();
                return rowsAffected > 0;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<int> InsertStudentFeeAsync(StudentFee studentFee, NpgsqlConnection conn, NpgsqlTransaction transaction)
        {
            Console.WriteLine($"Inserting student fee: {studentFee}");
            const string classSql = @"SELECT id FROM classes WHERE grade = @Grade and section = @Section";
            await using var classCmd = new NpgsqlCommand(classSql, conn, transaction);
            classCmd.Parameters.AddWithValue("Grade", studentFee.Grade);
            classCmd.Parameters.AddWithValue("Section", studentFee.Section);
            var classId = await classCmd.ExecuteScalarAsync() as Guid?;
            if (!classId.HasValue)
            {
                throw new Exception("Class not found");
            }
            studentFee.ClassId = classId.Value;

            const string sql = @"
                INSERT INTO fees_collection (
                    student_edunest_id,
                    date_of_collection,
                    fee_collected,
                    fee_waived,
                    waiver_reason,
                    class_id
                ) VALUES (
                    @student_edunest_id,
                    @date_of_collection,
                    @fee_collected,
                    @fee_waived,
                    @waiver_reason,
                    @class_id
                )";

            await using var cmd = new NpgsqlCommand(sql, conn, transaction);

            cmd.Parameters.AddWithValue("@student_edunest_id", studentFee.StudentEduNestId);
            cmd.Parameters.AddWithValue("@date_of_collection", studentFee.DateOfCollection);
            cmd.Parameters.AddWithValue("@fee_collected", studentFee.FeeCollected);
            cmd.Parameters.AddWithValue("@fee_waived", studentFee.FeeWaived);
            cmd.Parameters.AddWithValue("@waiver_reason", studentFee.WaiverReason ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@class_id", studentFee.ClassId);

            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<int> UpdateStudentFeeAsync(Guid id, StudentFee studentFee, NpgsqlConnection conn, NpgsqlTransaction transaction)
        {
            Console.WriteLine($"Updating student fee with ID: {id}");

            const string sql = @"
                UPDATE fees_collection 
                SET 
                    fee_collected = @fee_collected,
                    fee_waived = @fee_waived,
                    waiver_reason = @waiver_reason,
                    modified_date = @modified_date
                WHERE id = @id";

            await using var cmd = new NpgsqlCommand(sql, conn, transaction);

            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@fee_collected", studentFee.FeeCollected);
            cmd.Parameters.AddWithValue("@fee_waived", studentFee.FeeWaived);
            cmd.Parameters.AddWithValue("@waiver_reason", studentFee.WaiverReason ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@modified_date", DateTime.UtcNow);

            return await cmd.ExecuteNonQueryAsync();
        }
        public override async Task<IEnumerable<StudentFee>> GetAllAsync()
        {
            await using var conn = await _dataSource.OpenConnectionAsync();
            await conn.OpenAsync();

            const string sql = "SELECT * FROM fees_collection";
            await using var cmd = new NpgsqlCommand(sql, conn);

            await using var reader = await cmd.ExecuteReaderAsync();
            var fees = new List<StudentFee>();
            while (await reader.ReadAsync())
            {
                fees.Add(new StudentFee
                {
                    Id = reader.GetGuid(reader.GetOrdinal("id")),
                    StudentEduNestId = reader.GetString(reader.GetOrdinal("student_edunest_id")),
                    DateOfCollection = reader.GetDateTime(reader.GetOrdinal("date_of_collection")),
                    FeeCollected = reader.GetDecimal(reader.GetOrdinal("fee_collected")),
                    FeeWaived = reader.GetDecimal(reader.GetOrdinal("fee_waived")),
                    WaiverReason = reader.IsDBNull(reader.GetOrdinal("waiver_reason")) ? null : reader.GetString(reader.GetOrdinal("waiver_reason")),
                    Grade = reader.GetString(reader.GetOrdinal("grade")),
                    Section = reader.GetString(reader.GetOrdinal("section")),
                    TotalFees = reader.GetDecimal(reader.GetOrdinal("total_fees")),
                    FeeRemaining = reader.GetDecimal(reader.GetOrdinal("fee_remaining"))
                });
            }

            return fees;
        }

        public async Task<List<StudentFee?>> GetByEduNestIdAsync(string eduNestId)
        {
            await using var conn = await _dataSource.OpenConnectionAsync();
            const string sql = @"
                SELECT 
                    d.id, 
                    d.student_edunest_id, 
                    d.date_of_collection, 
                    d.fee_collected, 
                    d.fee_waived, 
                    d.waiver_reason,
                    d.created_at,
                    d.modified_date,
                    c.grade, 
                    c.section, 
                    f.total_annual_fee 
                FROM fees_collection d 
                INNER JOIN classes c ON d.class_id = c.id 
                INNER JOIN fee_admin f ON d.class_id = f.class_id 
                WHERE d.student_edunest_id = @EduNestId
                ORDER BY d.date_of_collection DESC";
                
            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("EduNestId", eduNestId);
            
            await using var reader = await cmd.ExecuteReaderAsync();
            var fees = new List<StudentFee?>();
            
            while (await reader.ReadAsync())
            {
                fees.Add(new StudentFee
                {
                    Id = reader.GetGuid(reader.GetOrdinal("id")),
                    StudentEduNestId = reader.GetString(reader.GetOrdinal("student_edunest_id")),
                    DateOfCollection = reader.GetDateTime(reader.GetOrdinal("date_of_collection")),
                    FeeCollected = reader.GetDecimal(reader.GetOrdinal("fee_collected")),
                    FeeWaived = reader.GetDecimal(reader.GetOrdinal("fee_waived")),
                    WaiverReason = reader.IsDBNull(reader.GetOrdinal("waiver_reason")) ? null : reader.GetString(reader.GetOrdinal("waiver_reason")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                    ModifiedAt = reader.GetDateTime(reader.GetOrdinal("modified_date")),
                    TotalFees = reader.GetDecimal(reader.GetOrdinal("total_annual_fee")),
                    Grade = reader.GetString(reader.GetOrdinal("grade")),
                    Section = reader.GetString(reader.GetOrdinal("section"))
                });
            }

            return fees;
        }
    }
}
