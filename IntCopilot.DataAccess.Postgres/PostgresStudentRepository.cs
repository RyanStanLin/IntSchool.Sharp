using System.Data.Common;
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
                
                var createTableSql = $@"
                    CREATE TABLE IF NOT EXISTS public.{TableName} (
                        student_id      BIGINT PRIMARY KEY,
                        student_name    TEXT NOT NULL,
                        first_name      TEXT,
                        last_name       TEXT,
                        student_num     TEXT UNIQUE,
                        default_name    TEXT,
                        english_name    TEXT,
                        email           TEXT UNIQUE,
                        nationality     TEXT,
                        enter_year      TEXT,
                        address         TEXT,
                        house_name      TEXT,
                        stage           TEXT,
                        id_number       TEXT,
                        image_url       TEXT,
                        is_male         BOOLEAN NOT NULL,
                        birthday        DATE,
                        section_name    TEXT,
                        class_name      TEXT
                    );
                    CREATE INDEX IF NOT EXISTS idx_students_name_trgm ON public.{TableName} USING gin (student_name gin_trgm_ops);";

                await using (var createTableCmd = new NpgsqlCommand(createTableSql, targetConnection))
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
    public async Task AddStudentAsync(StudentProfile student, CancellationToken cancellationToken = default)
    {
        const string sql = $@"
            INSERT INTO public.{TableName} (
                student_id, student_name, first_name, last_name, student_num, default_name, english_name, email,
                nationality, enter_year, address, house_name, stage, id_number, image_url,
                is_male, birthday, section_name, class_name
            ) VALUES (
                @StudentId, @StudentName, @FirstName, @LastName, @StudentNum, @DefaultName, @EnglishName, @Email,
                @Nationality, @EnterYear, @Address, @HouseName, @Stage, @IdNumber, @ImageUrl,
                @IsMale, @Birthday, @SectionName, @ClassName
            );";
        
        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        AddStudentProfileParameters(command, student);
        command.Parameters.Add(new NpgsqlParameter("StudentId", student.StudentId));
        await command.ExecuteNonQueryAsync(cancellationToken);
        _logger.LogInformation("Added student with ID {StudentId}.", student.StudentId);
    }

    /// <inheritdoc />
    public async Task<StudentProfile?> GetStudentByIdAsync(long studentId, CancellationToken cancellationToken = default)
    {
        var sql = $"SELECT {AllColumns} FROM public.{TableName} WHERE student_id = @Id";
        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters = { new NpgsqlParameter("Id", studentId) }
        };

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            return MapReaderToProfile(reader);
        }
        return null;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateStudentAsync(StudentProfile student, CancellationToken cancellationToken = default)
    {
        const string sql = $@"
            UPDATE public.{TableName} SET
                student_name = @StudentName, first_name = @FirstName, last_name = @LastName,
                student_num = @StudentNum, default_name = @DefaultName, english_name = @EnglishName,
                email = @Email, nationality = @Nationality, enter_year = @EnterYear, address = @Address,
                house_name = @HouseName, stage = @Stage, id_number = @IdNumber, image_url = @ImageUrl,
                is_male = @IsMale, birthday = @Birthday, section_name = @SectionName, class_name = @ClassName
            WHERE student_id = @StudentId;";
            
        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        AddStudentProfileParameters(command, student);
        command.Parameters.Add(new NpgsqlParameter("StudentId", student.StudentId));

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
            SELECT {AllColumns}, similarity(student_name, @Name) AS score
            FROM public.{TableName}
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
            var profile = MapReaderToProfile(reader);
            var similarity = reader.GetFloat(reader.GetOrdinal("score"));
            results.Add(new FuzzySearchResult(profile, similarity));
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
    
    // --- Private Helper Methods ---

    private const string AllColumns = @"
        student_id, student_name, first_name, last_name, student_num, default_name, english_name, email,
        nationality, enter_year, address, house_name, stage, id_number, image_url,
        is_male, birthday, section_name, class_name";

    private static StudentProfile MapReaderToProfile(DbDataReader reader) =>
        new StudentProfile(
            reader.GetInt64(reader.GetOrdinal("student_id")),
            reader.GetString(reader.GetOrdinal("student_name")),
            reader.IsDBNull(reader.GetOrdinal("student_num")) ? null : reader.GetString(reader.GetOrdinal("student_num")),
            reader.IsDBNull(reader.GetOrdinal("default_name")) ? null : reader.GetString(reader.GetOrdinal("default_name")),
            reader.IsDBNull(reader.GetOrdinal("english_name")) ? null : reader.GetString(reader.GetOrdinal("english_name")),
            reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString(reader.GetOrdinal("email")),
            reader.IsDBNull(reader.GetOrdinal("nationality")) ? null : reader.GetString(reader.GetOrdinal("nationality")),
            reader.IsDBNull(reader.GetOrdinal("enter_year")) ? null : reader.GetString(reader.GetOrdinal("enter_year")),
            reader.IsDBNull(reader.GetOrdinal("address")) ? null : reader.GetString(reader.GetOrdinal("address")),
            reader.IsDBNull(reader.GetOrdinal("house_name")) ? null : reader.GetString(reader.GetOrdinal("house_name")),
            reader.IsDBNull(reader.GetOrdinal("stage")) ? null : reader.GetString(reader.GetOrdinal("stage")),
            reader.IsDBNull(reader.GetOrdinal("id_number")) ? null : reader.GetString(reader.GetOrdinal("id_number")),
            reader.IsDBNull(reader.GetOrdinal("image_url")) ? null : reader.GetString(reader.GetOrdinal("image_url")),
            reader.GetBoolean(reader.GetOrdinal("is_male")),
            reader.IsDBNull(reader.GetOrdinal("birthday")) ? null : reader.GetFieldValue<DateOnly>(reader.GetOrdinal("birthday")),
            reader.IsDBNull(reader.GetOrdinal("section_name")) ? null : reader.GetString(reader.GetOrdinal("section_name")),
            reader.IsDBNull(reader.GetOrdinal("class_name")) ? null : reader.GetString(reader.GetOrdinal("class_name")),
            reader.IsDBNull(reader.GetOrdinal("first_name")) ? null : reader.GetString(reader.GetOrdinal("first_name")),
            reader.IsDBNull(reader.GetOrdinal("last_name")) ? null : reader.GetString(reader.GetOrdinal("last_name"))
        );

    private static void AddStudentProfileParameters(NpgsqlCommand command, StudentProfile student)
    {
        //command.Parameters.Add(new NpgsqlParameter("StudentId", student.StudentId));
        command.Parameters.Add(new NpgsqlParameter("StudentName", student.StudentName));
        command.Parameters.Add(new NpgsqlParameter("FirstName", (object?)student.FirstName ?? DBNull.Value));
        command.Parameters.Add(new NpgsqlParameter("LastName", (object?)student.LastName ?? DBNull.Value));
        command.Parameters.Add(new NpgsqlParameter("StudentNum", (object?)student.StudentNum ?? DBNull.Value));
        command.Parameters.Add(new NpgsqlParameter("DefaultName", (object?)student.DefaultName ?? DBNull.Value));
        command.Parameters.Add(new NpgsqlParameter("EnglishName", (object?)student.EnglishName ?? DBNull.Value));
        command.Parameters.Add(new NpgsqlParameter("Email", (object?)student.Email ?? DBNull.Value));
        command.Parameters.Add(new NpgsqlParameter("Nationality", (object?)student.Nationality ?? DBNull.Value));
        command.Parameters.Add(new NpgsqlParameter("EnterYear", (object?)student.EnterYear ?? DBNull.Value));
        command.Parameters.Add(new NpgsqlParameter("Address", (object?)student.Address ?? DBNull.Value));
        command.Parameters.Add(new NpgsqlParameter("HouseName", (object?)student.HouseName ?? DBNull.Value));
        command.Parameters.Add(new NpgsqlParameter("Stage", (object?)student.Stage ?? DBNull.Value));
        command.Parameters.Add(new NpgsqlParameter("IdNumber", (object?)student.IdNumber ?? DBNull.Value));
        command.Parameters.Add(new NpgsqlParameter("ImageUrl", (object?)student.ImageUrl ?? DBNull.Value));
        command.Parameters.Add(new NpgsqlParameter("IsMale", student.IsMale));
        command.Parameters.Add(new NpgsqlParameter("Birthday", (object?)student.Birthday ?? DBNull.Value));
        command.Parameters.Add(new NpgsqlParameter("SectionName", (object?)student.SectionName ?? DBNull.Value));
        command.Parameters.Add(new NpgsqlParameter("ClassName", (object?)student.ClassName ?? DBNull.Value));
    }
}