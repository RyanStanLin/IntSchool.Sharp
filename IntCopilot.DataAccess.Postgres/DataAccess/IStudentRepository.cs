using IntCopilot.DataAccess.Postgres.Models;
using IntCopilot.Shared;

namespace IntCopilot.DataAccess.Postgres.DataAccess;

/// <summary>
/// Defines the contract for student data operations.
/// This interface is disposable and should be managed by a DI container or an 'await using' block.
/// </summary>
public interface IStudentRepository : IAsyncDisposable
{

    /// <summary>
    /// Adds a new student to the database asynchronously.
    /// </summary>
    Task AddStudentAsync(Student student, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a student by their unique ID asynchronously.
    /// </summary>
    Task<Student?> GetStudentByIdAsync(long studentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing student's information asynchronously.
    /// </summary>
    Task<bool> UpdateStudentAsync(Student student, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a student by their unique ID asynchronously.
    /// </summary>
    Task<bool> DeleteStudentAsync(long studentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a fuzzy search for students by name using trigram similarity.
    /// </summary>
    Task<IEnumerable<FuzzySearchResult>> FuzzySearchByNameAsync(string name, float similarityThreshold = 0.3f, CancellationToken cancellationToken = default);
}