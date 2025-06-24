using IntCopilot.DataAccess.Postgres.Configuration;
using IntCopilot.DataAccess.Postgres.Models;
using IntCopilot.Shared;

namespace IntCopilot.DataAccess.Postgres.Models;

/// <summary>
/// Represents the result of a fuzzy search, including the matched student and a similarity score.
/// </summary>
/// <param name="Student">The matched student object.</param>
/// <param name="Similarity">The similarity score (from 0.0 to 1.0) between the search term and the student's name.</param>
public record FuzzySearchResult(StudentProfile Student, float Similarity);