namespace IntCopilot.DataAccess.Postgres.Exceptions;

/// <summary>
/// Exception thrown when the data access layer fails to initialize,
/// for instance, when it cannot create the database or table.
/// </summary>
[Serializable]
public class DataAccessInitializationException : Exception
{
    public DataAccessInitializationException() { }
    public DataAccessInitializationException(string message) : base(message) { }
    public DataAccessInitializationException(string message, Exception inner) : base(message, inner) { }
}