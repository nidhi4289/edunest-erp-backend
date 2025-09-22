using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EduNestERP.Persistence.Entities;
using BCrypt.Net;


namespace EduNestERP.Persistence.Repositories
{
    public class StaffRepository : IStaffRepository
    {
        private readonly ITenantDataSourceProvider _dataSource;

        public StaffRepository(ITenantDataSourceProvider dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<IEnumerable<Staff>> GetAllAsync(string? firstName = null, string? lastName = null, string? staffId = null)
        {
            var staffList = new List<Staff>();
            await using var conn = await _dataSource.OpenConnectionAsync();
            // Connection is already open
            using (var cmd = conn.CreateCommand())
            {
                var sql = "SELECT * FROM staff WHERE 1=1";
                if (!string.IsNullOrWhiteSpace(firstName))
                {
                    sql += " AND first_name ILIKE @firstName";
                    var param = cmd.CreateParameter();
                    param.ParameterName = "@firstName";
                    param.Value = $"%{firstName}%";
                    cmd.Parameters.Add(param);
                }
                if (!string.IsNullOrWhiteSpace(lastName))
                {
                    sql += " AND last_name ILIKE @lastName";
                    var param = cmd.CreateParameter();
                    param.ParameterName = "@lastName";
                    param.Value = $"%{lastName}%";
                    cmd.Parameters.Add(param);
                }
                if (!string.IsNullOrWhiteSpace(staffId))
                {
                    sql += " AND staff_id = @staffId";
                    var param = cmd.CreateParameter();
                    param.ParameterName = "@staffId";
                    param.Value = staffId;
                    cmd.Parameters.Add(param);
                }
                cmd.CommandText = sql;
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        staffList.Add(MapStaff(reader));
                    }
                }
            }
            return staffList;
        }

        public async Task<Staff> GetByIdAsync(Guid id)
        {
            await using var conn = await _dataSource.OpenConnectionAsync();
            {
                // Connection is already open
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM staff WHERE id = @id";
                    var param = cmd.CreateParameter();
                    param.ParameterName = "@id";
                    param.Value = id;
                    cmd.Parameters.Add(param);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return MapStaff(reader);
                        }
                    }
                }
            }
            return default!;
        }

        public async Task AddAsync(Staff staff)
        {
            await using var conn = await _dataSource.OpenConnectionAsync();
            // Connection is already open
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO staff (
                    id, staff_id, first_name, last_name, middle_name, gender, dob, personal_email, official_email, phone, role, joining_date, exit_date, status, address_line1, address_line2, city, state, zip, country, created_at
                ) VALUES (
                    @id, @staff_id, @first_name, @last_name, @middle_name, @gender, @dob, @personal_email, @official_email, @phone, @role, @joining_date, @exit_date, @status, @address_line1, @address_line2, @city, @state, @zip, @country, @created_at
                )";
                AddParameters(cmd, staff);
                await cmd.ExecuteNonQueryAsync();
            }

            // Add user entry for staff
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserId = staff.StaffId,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Start123", workFactor: 11),
                Role = "Teacher",
                FirstLoginCompleted = false
            };
            const string userSql = @"INSERT INTO users (id, user_id, password_hash, role, first_login_completed) VALUES (@id, @user_id, @password_hash, @role, @first_login_completed)";
            await using var userCmd = new Npgsql.NpgsqlCommand(userSql, conn);
            userCmd.Parameters.AddWithValue("id", user.Id);
            userCmd.Parameters.AddWithValue("user_id", user.UserId);
            userCmd.Parameters.AddWithValue("password_hash", user.PasswordHash);
            userCmd.Parameters.AddWithValue("role", user.Role);
            userCmd.Parameters.AddWithValue("first_login_completed", user.FirstLoginCompleted);
            await userCmd.ExecuteNonQueryAsync();

        // No longer needed: HashPassword method, using BCrypt.Net.BCrypt instead
        }

        public async Task UpdateAsync(Staff staff)
        {
            await using var conn = await _dataSource.OpenConnectionAsync();
            {
                // Connection is already open
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE staff SET
                        staff_id = @staff_id,
                        first_name = @first_name,
                        last_name = @last_name,
                        middle_name = @middle_name,
                        gender = @gender,
                        dob = @dob,
                        personal_email = @personal_email,
                        official_email = @official_email,
                        phone = @phone,
                        role = @role,
                        joining_date = @joining_date,
                        exit_date = @exit_date,
                        status = @status,
                        address_line1 = @address_line1,
                        address_line2 = @address_line2,
                        city = @city,
                        state = @state,
                        zip = @zip,
                        country = @country,
                        created_at = @created_at
                    WHERE id = @id";
                    AddParameters(cmd, staff);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            await using var conn = await _dataSource.OpenConnectionAsync();
            {
                // Connection is already open
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM staff WHERE id = @id";
                    var param = cmd.CreateParameter();
                    param.ParameterName = "@id";
                    param.Value = id;
                    cmd.Parameters.Add(param);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        private Staff MapStaff(System.Data.IDataReader reader)
        {
            return new Staff
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                StaffId = reader["staff_id"] is DBNull ? string.Empty : reader["staff_id"].ToString()!,
                FirstName = reader["first_name"] is DBNull ? string.Empty : reader["first_name"].ToString()!,
                LastName = reader["last_name"] is DBNull ? string.Empty : reader["last_name"].ToString()!,
                MiddleName = reader["middle_name"] is DBNull ? null : reader["middle_name"].ToString()!,
                Gender = reader["gender"] is DBNull ? null : reader["gender"].ToString()!,
                Dob = reader["dob"] is DBNull ? null : (DateTime?)reader["dob"],
                PersonalEmail = reader["personal_email"] is DBNull ? null : reader["personal_email"].ToString()!,
                OfficialEmail = reader["official_email"] is DBNull ? null : reader["official_email"].ToString()!,
                Phone = reader["phone"] is DBNull ? null : reader["phone"].ToString()!,
                Role = reader["role"] is DBNull ? string.Empty : reader["role"].ToString()!,
                JoiningDate = reader.GetDateTime(reader.GetOrdinal("joining_date")),
                ExitDate = reader["exit_date"] is DBNull ? null : (DateTime?)reader["exit_date"],
                Status = reader["status"] is DBNull ? "Active" : reader["status"].ToString()!,
                AddressLine1 = reader["address_line1"] is DBNull ? null : reader["address_line1"].ToString()!,
                AddressLine2 = reader["address_line2"] is DBNull ? null : reader["address_line2"].ToString()!,
                City = reader["city"] is DBNull ? null : reader["city"].ToString()!,
                State = reader["state"] is DBNull ? null : reader["state"].ToString()!,
                Zip = reader["zip"] is DBNull ? null : reader["zip"].ToString()!,
                Country = reader["country"] is DBNull ? null : reader["country"].ToString()!,
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
            };
        }

        private void AddParameters(System.Data.IDbCommand cmd, Staff staff)
        {
            var parameters = new[]
            {
                ("@id", (object)staff.Id),
                ("@staff_id", (object)staff.StaffId),
                ("@first_name", (object)staff.FirstName),
                ("@last_name", (object)staff.LastName),
                ("@middle_name", staff.MiddleName != null ? (object)staff.MiddleName : DBNull.Value),
                ("@gender", staff.Gender != null ? (object)staff.Gender : DBNull.Value),
                ("@dob", staff.Dob.HasValue ? (object)staff.Dob.Value : DBNull.Value),
                ("@personal_email", staff.PersonalEmail != null ? (object)staff.PersonalEmail : DBNull.Value),
                ("@official_email", staff.OfficialEmail != null ? (object)staff.OfficialEmail : DBNull.Value),
                ("@phone", staff.Phone != null ? (object)staff.Phone : DBNull.Value),
                ("@role", (object)staff.Role),
                ("@joining_date", (object)staff.JoiningDate),
                ("@exit_date", staff.ExitDate.HasValue ? (object)staff.ExitDate.Value : DBNull.Value),
                ("@status", (object)staff.Status),
                ("@address_line1", staff.AddressLine1 != null ? (object)staff.AddressLine1 : DBNull.Value),
                ("@address_line2", staff.AddressLine2 != null ? (object)staff.AddressLine2 : DBNull.Value),
                ("@city", staff.City != null ? (object)staff.City : DBNull.Value),
                ("@state", staff.State != null ? (object)staff.State : DBNull.Value),
                ("@zip", staff.Zip != null ? (object)staff.Zip : DBNull.Value),
                ("@country", staff.Country != null ? (object)staff.Country : DBNull.Value),
                ("@created_at", (object)staff.CreatedAt)
            };
            foreach (var (name, value) in parameters)
            {
                var param = cmd.CreateParameter();
                param.ParameterName = name;
                param.Value = value;
                cmd.Parameters.Add(param);
            }
        }
    }
}