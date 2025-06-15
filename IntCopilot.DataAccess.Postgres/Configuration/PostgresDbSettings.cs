namespace IntCopilot.DataAccess.Postgres.Configuration;

/// <summary>
/// Represents the configuration settings for connecting to a PostgreSQL database.
/// </summary>
public sealed class PostgresDbSettings
{
    /// <summary>
    /// The hostname or IP address of the PostgreSQL server.
    /// </summary>
    public required string Host { get; set; }

    /// <summary>
    /// The port number for the PostgreSQL server. Defaults to 5432.
    /// </summary>
    public int Port { get; set; } = 5432;

    /// <summary>
    /// The username for authentication.
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    /// The password for authentication.
    /// </summary>
    public required string Password { get; set; }

    /// <summary>
    /// The name of the database to connect to.
    /// This database will be created if it does not exist.
    /// </summary>
    public required string Database { get; set; }

    /// <summary>
    /// Gets the full connection string for the target database.
    /// </summary>
    public string ConnectionString =>
        $"Host={Host};Port={Port};Username={Username};Password={Password};Database={Database}";

    /// <summary>
    /// Gets the connection string for the maintenance database (e.g., "postgres").
    /// Used for administrative tasks like creating the target database.
    /// </summary>
    public string MaintenanceDbConnectionString =>
        $"Host={Host};Port={Port};Username={Username};Password={Password};Database=postgres";
}