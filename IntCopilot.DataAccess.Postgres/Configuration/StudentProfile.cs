using IntCopilot.Shared;

namespace IntCopilot.DataAccess.Postgres.Configuration;

/// <summary>
/// Represents the full profile of a student, inheriting basic information from the Student record.
/// </summary>
public record StudentProfile(
    // --- Inherited from Student ---
    long StudentId,
    string StudentName,

    // --- New Fields ---
    string? StudentNum,
    string? DefaultName,
    string? EnglishName,
    string? Email,
    string? Nationality,
    string? EnterYear,
    string? Address,
    string? HouseName,
    string? Stage,
    string? IdNumber,
    string? ImageUrl, // Renamed from 'image' for clarity
    bool IsMale,
    DateOnly? Birthday,
    string? SectionName,
    string? ClassName,
    string? FirstName,
    string? LastName
) : Student(StudentId, StudentName)
{
    // public StudentProfile(long StudentId, string StudentNum, string StudentName, string DefaultName, string EnglishName, string FirstName, string LastName, string Email, string Nationality, string EnterYear, string Address, string HouseName, string Stage, string IdNumber, string ImageUrl, bool IsMale, DateOnly Birthday, string SectionName, string ClassName) : base(BASE)
    // {
    //     throw new NotImplementedException();
    // }
}