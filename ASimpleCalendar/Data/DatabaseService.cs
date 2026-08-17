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
                CreatedAt TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Notes (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT NOT NULL,
                Content TEXT NOT NULL DEFAULT '',
                Color TEXT NULL,
                Tags TEXT NULL,
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
    }
}
