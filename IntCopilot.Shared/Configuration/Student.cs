namespace IntCopilot.Shared;

/// <summary>
/// Represents a student with a unique ID and a name.
/// </summary>
/// <param name="StudentId">The unique identifier for the student.</param>
/// <param name="StudentName">The name of the student.</param>
public record Student(long StudentId, string StudentName);