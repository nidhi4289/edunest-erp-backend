using EduNestERP.Persistence.Repositories;
using EduNestERP.Persistence.Entities;
using EduNestERP.Application.Interfaces;
using System.Linq;
namespace EduNestERP.Application.Services
{
    public class StudentFeeService : IStudentFeeService
    {
        private readonly IStudentFeeRepository _feeRepository;
        private readonly IStudentRepository _studentRepository;

        public StudentFeeService(IStudentFeeRepository feeRepository, IStudentRepository studentRepository)
        {
            _feeRepository = feeRepository;
            _studentRepository = studentRepository;     
        }

        public async Task<List<StudentFee>> GetFeesAsync()
        {
            return (await _feeRepository.GetAllAsync()).ToList();
        }

        public async Task<bool?> CreateFeeAsync(StudentFee fee)
        {
            //search for student Id from IStudentRepository
            //GetEduNestId if EduNestId is not present
            if (string.IsNullOrEmpty(fee.StudentEduNestId))
            {
                var student = await _studentRepository.GetByEduNestIdAsync(fee.StudentEduNestId);
                if (student == null)
                {
                    throw new ArgumentException("Invalid student ID");
                }

                fee.StudentEduNestId = student.EduNestId;
            }

            return await _feeRepository.AddAsync(fee);
        }

        public async Task<bool?> CreateBulkFeesAsync(List<StudentFee> fees)
        {
            foreach (var fee in fees)
            {
                if (string.IsNullOrEmpty(fee.StudentEduNestId))
                {
                    var student = await _studentRepository.GetByEduNestIdAsync(fee.StudentEduNestId);
                    if (student == null)
                    {
                        throw new ArgumentException("Invalid student ID");
                    }

                    fee.StudentEduNestId = student.EduNestId;
                }
            }

            return await _feeRepository.AddBulkStudentFeesAsync(fees);
        }

        public async Task<List<StudentFee>> GetFeeByEduNestIdAsync(string eduNestId)
        {
            var fees = await _feeRepository.GetByEduNestIdAsync(eduNestId);
            return fees.Where(f => f != null).Cast<StudentFee>().ToList();
        }

        public async Task<bool?> UpdateFeeAsync(StudentFee fee)
        {
            return await _feeRepository.UpdateAsync(fee);
        }

        public Task<bool> DeleteFeeAsync(int id)
        {
            return Task.FromResult(false);
        }
    }
}
    