    public interface IClassWithSubjects
    {
        List<Guid> SubjectIds { get; set; }
    }
namespace EduNestERP.Api.Model
{
    public class ClassDto
    {
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    }

    public class CreateClassDto
    {
    public string Name { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public List<Guid> SubjectIds { get; set; } = new();
    }

    public class CreateClassDtoWithSubjects : CreateClassDto, IClassWithSubjects
    {
    public new List<Guid> SubjectIds { get; set; } = new();
    }
    public class SubjectDto
    {
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? GradingType { get; set; }
    public int? MaxMarks { get; set; }
    public DateTime CreatedAt { get; set; }
    }
}
