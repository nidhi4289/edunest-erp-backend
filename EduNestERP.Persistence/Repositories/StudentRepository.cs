using EduNestERP.Persistence.Entities;
using Npgsql;

namespace EduNestERP.Persistence.Repositories
{
    public class StudentRepository : Repository<Student>, IStudentRepository
    {
        public StudentRepository(ITenantDataSourceProvider dataSource)
            : base(dataSource)
        {
        }

       public async Task<IEnumerable<Student>> SearchStudentsAsync(string? firstName, string? lastName, string? grade)
        {
            var students = new List<Student>();

            var sql = @"SELECT edunest_id, first_name, last_name, date_of_birth, grade, section, status, created_at, created_by, father_name, father_email, mother_name, mother_email, admission_number, address_line1, address_line2, city, state, zip, country, phone_number, secondary_phone_number, email, modified_at, modified_by
                        FROM students WHERE 1=1";
            var parameters = new List<NpgsqlParameter>();

            if (!string.IsNullOrWhiteSpace(firstName))
            {
                sql += " AND LOWER(first_name) LIKE @FirstName";
                parameters.Add(new NpgsqlParameter("FirstName", $"%{firstName.ToLower()}%"));
            }
            if (!string.IsNullOrWhiteSpace(lastName))
            {
                sql += " AND LOWER(last_name) LIKE @LastName";
                parameters.Add(new NpgsqlParameter("LastName", $"%{lastName.ToLower()}%"));
            }
            if (!string.IsNullOrWhiteSpace(grade))
            {
                sql += " AND LOWER(grade) = @Grade";
                parameters.Add(new NpgsqlParameter("Grade", grade.ToLower()));
            }

            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(sql, conn);
            foreach (var param in parameters)
                cmd.Parameters.Add(param);

            await using var rdr = await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync())
            {
                students.Add(new Student
                {
                    EduNestId = rdr.GetString(0),
                    FirstName = rdr.GetString(1),
                    LastName = rdr.GetString(2),
                    DateOfBirth = rdr.GetDateTime(3),
                    Grade = rdr.GetString(4),
                    Section = rdr.IsDBNull(5) ? null : rdr.GetString(5),
                    Status = rdr.GetString(6),
                    CreatedAt = rdr.GetDateTime(7),
                    CreatedBy = rdr.IsDBNull(8) ? null : rdr.GetString(8),
                    FatherName = rdr.IsDBNull(9) ? null : rdr.GetString(9),
                    FatherEmail = rdr.IsDBNull(10) ? null : rdr.GetString(10),
                    MotherName = rdr.IsDBNull(11) ? null : rdr.GetString(11),
                    MotherEmail = rdr.IsDBNull(12) ? null : rdr.GetString(12),
                    AdmissionNumber = rdr.IsDBNull(13) ? null : rdr.GetString(13),
                    AddressLine1 = rdr.IsDBNull(14) ? null : rdr.GetString(14),
                    AddressLine2 = rdr.IsDBNull(15) ? null : rdr.GetString(15),
                    City = rdr.IsDBNull(16) ? null : rdr.GetString(16),
                    State = rdr.IsDBNull(17) ? null : rdr.GetString(17),
                    ZipCode = rdr.IsDBNull(18) ? null : rdr.GetString(18),
                    Country = rdr.IsDBNull(19) ? null : rdr.GetString(19),
                    PhoneNumber = rdr.IsDBNull(20) ? null : rdr.GetString(20),
                    SecondaryPhoneNumber = rdr.IsDBNull(21) ? null : rdr.GetString(21),
                    Email = rdr.IsDBNull(22) ? null : rdr.GetString(22),
                    ModifiedAt = rdr.IsDBNull(23) ? (DateTime?)null : rdr.GetDateTime(23),
                    ModifiedBy = rdr.IsDBNull(24) ? null : rdr.GetString(24)
                });
            }
            return students;
        }

        public override async Task<Student?> GetByIdAsync(Guid id)
        {
            await using var conn = await _dataSource.OpenConnectionAsync();
            const string sql = "SELECT edunest_id, first_name, last_name, grade, status, created_at FROM students WHERE id = @id";
            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("id", id);
            await using var rdr = await cmd.ExecuteReaderAsync();
            if (await rdr.ReadAsync())
            {
                return new Student
                {
                    EduNestId = rdr.GetString(0),
                    FirstName = rdr.GetString(1),
                    LastName = rdr.GetString(2),
                    Grade = rdr.GetString(3),
                    Status = rdr.GetString(4),
                    CreatedAt = rdr.GetDateTime(5)
                };
            }
            return null;
        }

        public async Task<Student?> GetByEduNestIdAsync(string eduNestId)
        {
            await using var conn = await _dataSource.OpenConnectionAsync();
            const string sql = "SELECT edunest_id, first_name, last_name, date_of_birth, grade, section, status, created_at, created_by, father_name, father_email, mother_name, mother_email, admission_number, address_line1, address_line2, city, state, zip, country, phone_number, secondary_phone_number, email, modified_at, modified_by FROM students WHERE edunest_id = @EduNestId";
            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("EduNestId", eduNestId);
            await using var rdr = await cmd.ExecuteReaderAsync();
            if (await rdr.ReadAsync())
            {
                return new Student
                {
                    EduNestId = rdr.GetString(0),
                    FirstName = rdr.GetString(1),
                    LastName = rdr.GetString(2),
                    DateOfBirth = rdr.GetDateTime(3),
                    Grade = rdr.GetString(4),
                    Section = rdr.IsDBNull(5) ? null : rdr.GetString(5),
                    Status = rdr.GetString(6),
                    CreatedAt = rdr.GetDateTime(7),
                    CreatedBy = rdr.IsDBNull(8) ? null : rdr.GetString(8),
                    FatherName = rdr.IsDBNull(9) ? null : rdr.GetString(9),
                    FatherEmail = rdr.IsDBNull(10) ? null : rdr.GetString(10),
                    MotherName = rdr.IsDBNull(11) ? null : rdr.GetString(11),
                    MotherEmail = rdr.IsDBNull(12) ? null : rdr.GetString(12),
                    AdmissionNumber = rdr.IsDBNull(13) ? null : rdr.GetString(13),
                    AddressLine1 = rdr.IsDBNull(14) ? null : rdr.GetString(14),
                    AddressLine2 = rdr.IsDBNull(15) ? null : rdr.GetString(15),
                    City = rdr.IsDBNull(16) ? null : rdr.GetString(16),
                    State = rdr.IsDBNull(17) ? null : rdr.GetString(17),
                    ZipCode = rdr.IsDBNull(18) ? null : rdr.GetString(18),
                    Country = rdr.IsDBNull(19) ? null : rdr.GetString(19),
                    PhoneNumber = rdr.IsDBNull(20) ? null : rdr.GetString(20),
                    SecondaryPhoneNumber = rdr.IsDBNull(21) ? null : rdr.GetString(21),
                    Email = rdr.IsDBNull(22) ? null : rdr.GetString(22),
                    ModifiedAt = rdr.IsDBNull(23) ? (DateTime?)null : rdr.GetDateTime(23),
                    ModifiedBy = rdr.IsDBNull(24) ? null : rdr.GetString(24)
                };
            }
            return null;
        }

        public override async Task<bool?> AddAsync(Student student)
        {
            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var transaction = await conn.BeginTransactionAsync();
            try
            {
                var rowsAffected = await InsertStudentAsync(student, conn, transaction);
                await transaction.CommitAsync();
                return rowsAffected > 0;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        
        public override async Task<bool?> UpdateAsync(Student student)
        {
            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var transaction = await conn.BeginTransactionAsync();
            try
            {
                var rowsAffected = await UpdateStudentAsync(student, conn, transaction);
                await transaction.CommitAsync();
                return rowsAffected > 0;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> AddStudentsBulkAsync(List<Student> students)
        {
            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var transaction = await conn.BeginTransactionAsync();
            try
            {
                foreach (var student in students)
                {
                    // Generate eduNestId as 'ST-' + FirstName + LastName + DateOfBirth (yyyymmdd)
                    var dobStr = student.DateOfBirth.ToString("yyyyMMdd");
                    var generatedEduNestId = $"ST-{student.FirstName}{student.LastName}{dobStr}";

                    // Search for student by edunest_id
                    const string checkSql = @"SELECT edunest_id FROM students WHERE edunest_id = @EduNestId";
                    await using var checkCmd = new NpgsqlCommand(checkSql, conn, transaction);
                    checkCmd.Parameters.AddWithValue("EduNestId", generatedEduNestId);
                    var existingEduNestId = await checkCmd.ExecuteScalarAsync() as string;

                    // Set eduNestId for insert/update
                    student.EduNestId = generatedEduNestId;

                    if (!string.IsNullOrWhiteSpace(existingEduNestId))
                    {
                        await UpdateStudentAsync(student, conn, transaction);
                    }
                    else
                    {
                        await InsertStudentAsync(student, conn, transaction);
                    }
                }
                
                await transaction.CommitAsync();
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<int> InsertStudentAsync(Student student, NpgsqlConnection conn, NpgsqlTransaction transaction)
        {
        
            Console.WriteLine($"Inserting student: {student}");

            const string sql = @"INSERT INTO students 
                (edunest_id, first_name, last_name, date_of_birth, grade, section, status, created_at, created_by, father_name, father_email, mother_name, mother_email, admission_number, address_line1, address_line2, city, state, zip, country, phone_number, secondary_phone_number, email)
                VALUES (@EduNestId, @FirstName, @LastName, @DateOfBirth, @Grade, @Section, @Status, @CreatedAt, @CreatedBy, @FatherName, @FatherEmail, @MotherName, @MotherEmail, @AdmissionNumber, @AddressLine1, @AddressLine2, @City, @State, @Zip, @Country, @PhoneNumber, @SecondaryPhoneNumber, @Email)";
            await using var cmd = new NpgsqlCommand(sql, conn, transaction);
            cmd.Parameters.AddWithValue("EduNestId", student.EduNestId);
            cmd.Parameters.AddWithValue("FirstName", student.FirstName);
            cmd.Parameters.AddWithValue("LastName", student.LastName);
            cmd.Parameters.AddWithValue("DateOfBirth", student.DateOfBirth);
            cmd.Parameters.AddWithValue("Grade", student.Grade);
            cmd.Parameters.AddWithValue("Section", student.Section);
            cmd.Parameters.AddWithValue("Status", student.Status);
            cmd.Parameters.AddWithValue("CreatedAt", DateTime.Now);
            cmd.Parameters.AddWithValue("CreatedBy", "BulkUpload");
            cmd.Parameters.AddWithValue("FatherName", student.FatherName);
            cmd.Parameters.AddWithValue("FatherEmail", student.FatherEmail ?? "");
            cmd.Parameters.AddWithValue("MotherName", student.MotherName);
            cmd.Parameters.AddWithValue("MotherEmail", student.MotherEmail ?? "");
            cmd.Parameters.AddWithValue("AdmissionNumber", student.AdmissionNumber ?? "");
            cmd.Parameters.AddWithValue("AddressLine1", student.AddressLine1 ?? "");
            cmd.Parameters.AddWithValue("AddressLine2", student.AddressLine2 ?? "");
            cmd.Parameters.AddWithValue("City", student.City ?? "");
            cmd.Parameters.AddWithValue("State", student.State ?? "");
            cmd.Parameters.AddWithValue("Zip", student.ZipCode ?? "");
            cmd.Parameters.AddWithValue("Country", student.Country ?? "");
            cmd.Parameters.AddWithValue("PhoneNumber", student.PhoneNumber ?? "");
            cmd.Parameters.AddWithValue("SecondaryPhoneNumber", student.SecondaryPhoneNumber ?? "");
            cmd.Parameters.AddWithValue("Email", student.Email ?? "");
            return await cmd.ExecuteNonQueryAsync();
        }
       
        public async Task<int> UpdateStudentAsync(Student student, NpgsqlConnection conn, NpgsqlTransaction transaction)
        {
            Console.WriteLine($"Updating student: {student}");
            const string sql = @"UPDATE students SET
            grade = @Grade,
            section = @Section,
            status = @Status,
            modified_at = @ModifiedAt,
            modified_by = @ModifiedBy,
            father_name = @FatherName,
            father_email = @FatherEmail,
            mother_name = @MotherName,
            mother_email = @MotherEmail,
            admission_number = @AdmissionNumber,
            address_line1 = @AddressLine1,
            address_line2 = @AddressLine2,
            city = @City,
            state = @State,
            zip = @Zip,
            country = @Country,
            phone_number = @PhoneNumber,
            secondary_phone_number = @SecondaryPhoneNumber,
            email = @Email
            WHERE edunest_id = @EduNestId";

            await using var cmd = new NpgsqlCommand(sql, conn, transaction);
            cmd.Parameters.AddWithValue("Grade", student.Grade);
            cmd.Parameters.AddWithValue("Section", student.Section);
            cmd.Parameters.AddWithValue("Status", student.Status);
            cmd.Parameters.AddWithValue("ModifiedAt", student.ModifiedAt ?? DateTime.UtcNow);
            cmd.Parameters.AddWithValue("ModifiedBy", student.ModifiedBy );
            cmd.Parameters.AddWithValue("FatherName", student.FatherName);
            cmd.Parameters.AddWithValue("FatherEmail", student.FatherEmail ?? "");
            cmd.Parameters.AddWithValue("MotherName", student.MotherName);
            cmd.Parameters.AddWithValue("MotherEmail", student.MotherEmail ?? "");
            cmd.Parameters.AddWithValue("AdmissionNumber", student.AdmissionNumber ?? "");
            cmd.Parameters.AddWithValue("AddressLine1", student.AddressLine1 ?? "");
            cmd.Parameters.AddWithValue("AddressLine2", student.AddressLine2 ?? "");
            cmd.Parameters.AddWithValue("City", student.City ?? "");
            cmd.Parameters.AddWithValue("State", student.State ?? "");
            cmd.Parameters.AddWithValue("Zip", student.ZipCode ?? "");
            cmd.Parameters.AddWithValue("Country", student.Country ?? "");
            cmd.Parameters.AddWithValue("PhoneNumber", student.PhoneNumber ?? "");
            cmd.Parameters.AddWithValue("SecondaryPhoneNumber", student.SecondaryPhoneNumber ?? "");
            cmd.Parameters.AddWithValue("Email", student.Email ?? "");
            cmd.Parameters.AddWithValue("EduNestId", student.EduNestId);
            return await cmd.ExecuteNonQueryAsync();

        }

    }
}