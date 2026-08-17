using Microsoft.Data.Sqlite;

namespace ASimpleCalendar.Data;

public class SettingsRepository : ISettingsRepository
{
    private readonly DatabaseService _db;

    public SettingsRepository(DatabaseService db)
    {
        _db = db;
    }

    public string? Get(string key)
    {
        using var connection = _db.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Value FROM Settings WHERE Key = @key";
        command.Parameters.AddWithValue("@key", key);
        var result = command.ExecuteScalar();
        return result is string value ? value : null;
    }

    public void Set(string key, string value)
    {
        using var connection = _db.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Settings (Key, Value) VALUES (@key, @value)
            ON CONFLICT(Key) DO UPDATE SET Value = @value
            """;
        command.Parameters.AddWithValue("@key", key);
        command.Parameters.AddWithValue("@value", value);
        command.ExecuteNonQuery();
    }
}
