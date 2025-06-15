using IntCopilot.DataAccess.Postgres.Configuration;
using IntCopilot.DataAccess.Postgres.DataAccess;
using IntCopilot.DataAccess.Postgres.Exceptions;
using IntCopilot.DataAccess.Postgres.Models;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Text.RegularExpressions;
using System.Globalization;
using IntCopilot.Shared;

namespace IntCopilot.DataAccess.Postgres.DataAccess;

/// <summary>
/// Provides a production-grade PostgreSQL implementation for student data operations,
/// using NpgsqlDataSource for efficient connection pooling.
/// This class manages its own resources and must be created via the asynchronous factory method CreateAsync.
/// </summary>
public sealed class PostgresStudentRepository : IStudentRepository
{
    private const string TableName = "students";
    private readonly NpgsqlDataSource _dataSource;
    private readonly ILogger<PostgresStudentRepository> _logger;

    // Regex for basic validation of a PostgreSQL database name.
    // Allows letters, numbers, and underscores. Must not be empty.
    private static readonly Regex SafeDbNameRegex = new("^[a-zA-Z0-9_]+$");

    /// <summary>
    /// Private constructor to prevent direct instantiation.
    /// Use the CreateAsync factory method instead.
    /// </summary>
    /// <param name="dataSource">The NpgsqlDataSource to be used for all connections.</param>
    /// <param name="logger">The logger instance.</param>
    private PostgresStudentRepository(NpgsqlDataSource dataSource, ILogger<PostgresStudentRepository> logger)
    {
        _dataSource = dataSource;
        _logger = logger;
    }

    /// <summary>
    /// Asynchronously creates and initializes a new instance of the <see cref="PostgresStudentRepository"/>.
    /// This is the correct and only way to instantiate this class. It handles database/table creation and
    /// sets up an efficient connection pool via NpgsqlDataSource.
    /// </summary>
    /// <param name="settings">The database connection settings.</param>
    /// <param name="logger">The logger for recording operations and errors.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A fully initialized instance of <see cref="IStudentRepository"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if settings or logger are null.</exception>
    /// <exception cref="ArgumentException">Thrown if the database name in settings is invalid.</exception>
    /// <exception cref="DataAccessInitializationException">Thrown if database or table initialization fails.</exception>
    public static async Task<IStudentRepository> CreateAsync(
        PostgresDbSettings settings, 
        ILogger<PostgresStudentRepository> logger,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(logger);

        // This static method correctly performs the full, two-phase initialization before creating the repository.
        await InitializeDatabaseAndSchemaAsync(settings, logger, cancellationToken);

        // Once initialization is confirmed, create the long-lived data source for the repository to use.
        var dataSource = NpgsqlDataSource.Create(settings.ConnectionString);
        
        var repository = new PostgresStudentRepository(dataSource, logger);
        
        return repository;
    }

    /// <summary>
    /// A static helper method that ensures the database, its extensions, and tables are all correctly created
    /// in a robust, two-phase process. This is run once during the factory creation.
    /// </summary>
    private static async Task InitializeDatabaseAndSchemaAsync(PostgresDbSettings settings, ILogger logger, CancellationToken cancellationToken)
    {
        try
        {
            if (!SafeDbNameRegex.IsMatch(settings.Database))
            {
                throw new ArgumentException($"Database name '{settings.Database}' contains invalid characters. Only letters, numbers, and underscores are allowed.", nameof(settings.Database));
            }
            
            // --- PHASE 1: Ensure database exists (using maintenance connection) ---
            logger.LogInformation("Starting Phase 1: Ensuring database '{Database}' exists...", settings.Database);
            await using (var maintConnection = new NpgsqlConnection(settings.MaintenanceDbConnectionString))
            {
                await maintConnection.OpenAsync(cancellationToken);
                await using var checkDbCmd = new NpgsqlCommand("SELECT 1 FROM pg_database WHERE datname = @dbName", maintConnection)
                {
                    Parameters = { new NpgsqlParameter("dbName", settings.Database) }
                };
                
                var dbExists = await checkDbCmd.ExecuteScalarAsync(cancellationToken) != null;

                if (!dbExists)
                {
                    logger.LogInformation("Database '{Database}' not found. Creating...", settings.Database);
                    var createDbCmd = new NpgsqlCommand($"CREATE DATABASE \"{settings.Database}\"", maintConnection);
                    await createDbCmd.ExecuteNonQueryAsync(cancellationToken);
                    logger.LogInformation("Database '{Database}' created successfully.", settings.Database);
                }
                else
                {
                    logger.LogInformation("Database '{Database}' already exists.", settings.Database);
                }
            } // Maintenance connection is disposed here.
            logger.LogInformation("Phase 1 complete.");

            // --- PHASE 2: Ensure schema (extensions, tables, indexes) exists (using target connection) ---
            logger.LogInformation("Starting Phase 2: Ensuring schema in '{Database}' is up to date...", settings.Database);
            await using (var targetConnection = new NpgsqlConnection(settings.ConnectionString))
            {
                await targetConnection.OpenAsync(cancellationToken);

                // Ensure pg_trgm extension is enabled
                logger.LogInformation("Checking for 'pg_trgm' extension...");
                await using (var createExtensionCmd = new NpgsqlCommand("CREATE EXTENSION IF NOT EXISTS pg_trgm;", targetConnection))
                {
                    await createExtensionCmd.ExecuteNonQueryAsync(cancellationToken);
                }
                logger.LogInformation("'pg_trgm' extension is enabled.");

                // Ensure students table and its GIN index exist for fuzzy search performance
                logger.LogInformation("Checking for '{TableName}' table and index...", TableName);
                await using (var createTableCmd = new NpgsqlCommand($@"
                    CREATE TABLE IF NOT EXISTS public.{TableName} (
                        student_id BIGINT PRIMARY KEY,
                        student_name TEXT NOT NULL
                    );
                    CREATE INDEX IF NOT EXISTS idx_students_name_trgm ON public.{TableName} USING gin (student_name gin_trgm_ops);", targetConnection))
                {
                    await createTableCmd.ExecuteNonQueryAsync(cancellationToken);
                }
                logger.LogInformation("Table '{TableName}' and its indexes are ready.", TableName);
            } // Target connection is disposed here.
            logger.LogInformation("Phase 2 complete. Full initialization successful.");

        }
        catch (Exception ex) when (ex is not ArgumentException)
        {
            logger.LogError(ex, "A critical error occurred during database and schema initialization.");
            throw new DataAccessInitializationException("Failed to initialize the database and schema. See inner exception for details.", ex);
        }
    }
    
    /// <inheritdoc />
    public async Task AddStudentAsync(Student student, CancellationToken cancellationToken = default)
    {
        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        
        await using var command = new NpgsqlCommand($"INSERT INTO public.{TableName} (student_id, student_name) VALUES (@Id, @Name)", connection)
        {
            Parameters = { new("Id", student.StudentId), new("Name", student.StudentName) }
        };
        await command.ExecuteNonQueryAsync(cancellationToken);
        _logger.LogDebug("Added student with ID {StudentId}.", student.StudentId);
    }

    /// <inheritdoc />
    public async Task<Student?> GetStudentByIdAsync(long studentId, CancellationToken cancellationToken = default)
    {
        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand($"SELECT student_id, student_name FROM public.{TableName} WHERE student_id = @Id", connection)
        {
            Parameters = { new("Id", studentId) }
        };

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            return new Student(reader.GetInt64(0), reader.GetString(1));
        }
        return null;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateStudentAsync(Student student, CancellationToken cancellationToken = default)
    {
        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand($"UPDATE public.{TableName} SET student_name = @Name WHERE student_id = @Id", connection)
        {
            Parameters =
            {
                new NpgsqlParameter("Name", student.StudentName),
                new NpgsqlParameter("Id", student.StudentId)
            }
        };
        var rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
        if (rowsAffected > 0)
        {
            _logger.LogDebug("Updated student with ID {StudentId}.", student.StudentId);
            return true;
        }
        _logger.LogWarning("Attempted to update non-existent student with ID {StudentId}.", student.StudentId);
        return false;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteStudentAsync(long studentId, CancellationToken cancellationToken = default)
    {
        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand($"DELETE FROM public.{TableName} WHERE student_id = @Id", connection)
        {
            Parameters = { new NpgsqlParameter("Id", studentId) }
        };
        var rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
        if (rowsAffected > 0)
        {
            _logger.LogDebug("Deleted student with ID {StudentId}.", studentId);
            return true;
        }
        _logger.LogWarning("Attempted to delete non-existent student with ID {StudentId}.", studentId);
        return false;
    }
    
    /// <inheritdoc />
    public async Task<IEnumerable<FuzzySearchResult>> FuzzySearchByNameAsync(string name, float similarityThreshold = 0.3f, CancellationToken cancellationToken = default)
    {
        if (similarityThreshold < 0.0f || similarityThreshold > 1.0f)
        {
            throw new ArgumentOutOfRangeException(nameof(similarityThreshold), "Similarity threshold must be between 0.0 and 1.0.");
        }

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        var thresholdString = similarityThreshold.ToString(CultureInfo.InvariantCulture);
        
        var query = $@"
            SET LOCAL pg_trgm.similarity_threshold = {thresholdString};
            SELECT student_id, student_name, similarity(student_name, @Name) AS score
            FROM public.students
            WHERE student_name % @Name
            ORDER BY score DESC, student_name
            LIMIT 100;"; // Adding a sensible limit to prevent huge result sets + cover similarity_threshold
        
        await using var command = new NpgsqlCommand(query, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("Name", name)
                //new NpgsqlParameter("Threshold", similarityThreshold)//injected
            }
        };

        var results = new List<FuzzySearchResult>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(new FuzzySearchResult(
                new Student(reader.GetInt64(0), reader.GetString(1)), 
                reader.GetFloat(2))
            );
        }
        _logger.LogDebug("Fuzzy search for '{Name}' with threshold {Threshold} found {Count} results.", name, similarityThreshold, results.Count);
        return results;
    }

    /// <summary>
    /// Disposes the underlying NpgsqlDataSource, which in turn gracefully closes all connections in the pool.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        await _dataSource.DisposeAsync();
        _logger.LogInformation("PostgresStudentRepository and its connection pool have been disposed.");
    }
}