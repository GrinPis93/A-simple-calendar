using System.IO;
using Microsoft.Data.Sqlite;

namespace ASimpleCalendar.Data;

public class DatabaseService
{
    private readonly string _connectionString;

    public DatabaseService()
    {
        var directory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ASimpleCalendar");

        Directory.CreateDirectory(directory);

        _connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = Path.Combine(directory, "asimplecalendar.db")
        }.ToString();
    }

    public SqliteConnection CreateConnection()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        return connection;
    }

    public void Initialize()
    {
        using var connection = CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS Events (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT NOT NULL,
                Description TEXT NULL,
                StartDate TEXT NOT NULL,
                EndDate TEXT NULL,
                AllDay INTEGER NOT NULL DEFAULT 0,
                Category TEXT NULL,
                Color TEXT NULL,
                Repeat INTEGER NOT NULL DEFAULT 0,
                RepeatUntil TEXT NULL,
                CreatedAt TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Notes (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT NOT NULL,
                Content TEXT NOT NULL DEFAULT '',
                Color TEXT NULL,
                Tags TEXT NULL,
                Category TEXT NULL,
                IsPinned INTEGER NOT NULL DEFAULT 0,
                CreatedAt TEXT NOT NULL,
                UpdatedAt TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Reminders (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT NOT NULL,
                Message TEXT NULL,
                RemindAt TEXT NOT NULL,
                Repeat INTEGER NOT NULL DEFAULT 0,
                IsActive INTEGER NOT NULL DEFAULT 1,
                EventId INTEGER NULL,
                LastNotifiedAt TEXT NULL
            );

            CREATE TABLE IF NOT EXISTS Settings (
                Key TEXT PRIMARY KEY,
                Value TEXT NULL
            );
            """;
        command.ExecuteNonQuery();

        // Ускорение записи и конкурентного доступа (чтение/запись параллельно).
        using var pragma = connection.CreateCommand();
        pragma.CommandText = "PRAGMA journal_mode=WAL;";
        pragma.ExecuteNonQuery();

        // Миграции для баз, созданных в ранних версиях приложения.
        EnsureColumn(connection, "Events", "Repeat", "INTEGER NOT NULL DEFAULT 0");
        EnsureColumn(connection, "Events", "RepeatUntil", "TEXT NULL");
        EnsureColumn(connection, "Notes", "Category", "TEXT NULL");
    }

    private static void EnsureColumn(SqliteConnection connection, string table, string column, string definition)
    {
        using var check = connection.CreateCommand();
        check.CommandText = $"PRAGMA table_info({table})";
        using var reader = check.ExecuteReader();

        var exists = false;
        while (reader.Read())
        {
            if (reader.GetString(1) == column)
            {
                exists = true;
                break;
            }
        }

        if (exists)
        {
            return;
        }

        using var alter = connection.CreateCommand();
        alter.CommandText = $"ALTER TABLE {table} ADD COLUMN {column} {definition}";
        alter.ExecuteNonQuery();
    }
}
