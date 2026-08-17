using ASimpleCalendar.Models;
using Microsoft.Data.Sqlite;

namespace ASimpleCalendar.Data;

public class ReminderRepository : IReminderRepository
{
    private readonly DatabaseService _db;

    public ReminderRepository(DatabaseService db)
    {
        _db = db;
    }

    public List<Reminder> GetAll()
    {
        using var connection = _db.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM Reminders ORDER BY RemindAt";
        return ReadAll(command);
    }

    public List<Reminder> GetActive()
    {
        using var connection = _db.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM Reminders WHERE IsActive = 1 ORDER BY RemindAt";
        return ReadAll(command);
    }

    public Reminder? GetById(int id)
    {
        using var connection = _db.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM Reminders WHERE Id = @id";
        command.Parameters.AddWithValue("@id", id);
        using var reader = command.ExecuteReader();
        return reader.Read() ? Map(reader) : null;
    }

    public int Add(Reminder item)
    {
        using var connection = _db.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Reminders (Title, Message, RemindAt, Repeat, IsActive, EventId, LastNotifiedAt)
            VALUES (@title, @message, @remindAt, @repeat, @isActive, @eventId, @lastNotifiedAt);
            SELECT last_insert_rowid();
            """;
        command.Parameters.AddWithValue("@title", item.Title);
        command.Parameters.AddWithValue("@message", (object?)item.Message ?? DBNull.Value);
        command.Parameters.AddWithValue("@remindAt", item.RemindAt.ToString("O"));
        command.Parameters.AddWithValue("@repeat", (int)item.Repeat);
        command.Parameters.AddWithValue("@isActive", item.IsActive ? 1 : 0);
        command.Parameters.AddWithValue("@eventId", item.EventId.HasValue ? item.EventId.Value : DBNull.Value);
        command.Parameters.AddWithValue("@lastNotifiedAt", item.LastNotifiedAt.HasValue ? item.LastNotifiedAt.Value.ToString("O") : DBNull.Value);

        item.Id = Convert.ToInt32(command.ExecuteScalar());
        return item.Id;
    }

    public void Update(Reminder item)
    {
        using var connection = _db.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE Reminders SET
                Title = @title,
                Message = @message,
                RemindAt = @remindAt,
                Repeat = @repeat,
                IsActive = @isActive,
                EventId = @eventId,
                LastNotifiedAt = @lastNotifiedAt
            WHERE Id = @id
            """;
        command.Parameters.AddWithValue("@title", item.Title);
        command.Parameters.AddWithValue("@message", (object?)item.Message ?? DBNull.Value);
        command.Parameters.AddWithValue("@remindAt", item.RemindAt.ToString("O"));
        command.Parameters.AddWithValue("@repeat", (int)item.Repeat);
        command.Parameters.AddWithValue("@isActive", item.IsActive ? 1 : 0);
        command.Parameters.AddWithValue("@eventId", item.EventId.HasValue ? item.EventId.Value : DBNull.Value);
        command.Parameters.AddWithValue("@lastNotifiedAt", item.LastNotifiedAt.HasValue ? item.LastNotifiedAt.Value.ToString("O") : DBNull.Value);
        command.Parameters.AddWithValue("@id", item.Id);
        command.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var connection = _db.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Reminders WHERE Id = @id";
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();
    }

    private static List<Reminder> ReadAll(SqliteCommand command)
    {
        var result = new List<Reminder>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            result.Add(Map(reader));
        }
        return result;
    }

    private static Reminder Map(SqliteDataReader reader)
    {
        return new Reminder
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            Title = reader.GetString(reader.GetOrdinal("Title")),
            Message = reader.IsDBNull(reader.GetOrdinal("Message")) ? null : reader.GetString(reader.GetOrdinal("Message")),
            RemindAt = EventRepository.ParseDate(reader.GetString(reader.GetOrdinal("RemindAt"))),
            Repeat = (RepeatRule)reader.GetInt32(reader.GetOrdinal("Repeat")),
            IsActive = reader.GetInt32(reader.GetOrdinal("IsActive")) == 1,
            EventId = reader.IsDBNull(reader.GetOrdinal("EventId")) ? null : reader.GetInt32(reader.GetOrdinal("EventId")),
            LastNotifiedAt = reader.IsDBNull(reader.GetOrdinal("LastNotifiedAt")) ? null : EventRepository.ParseDate(reader.GetString(reader.GetOrdinal("LastNotifiedAt")))
        };
    }
}
