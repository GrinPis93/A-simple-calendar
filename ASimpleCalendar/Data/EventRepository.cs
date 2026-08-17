using ASimpleCalendar.Models;
using Microsoft.Data.Sqlite;

namespace ASimpleCalendar.Data;

public class EventRepository : IEventRepository
{
    private readonly DatabaseService _db;

    public EventRepository(DatabaseService db)
    {
        _db = db;
    }

    public List<Event> GetAll()
    {
        using var connection = _db.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM Events ORDER BY StartDate";
        return ReadAll(command);
    }

    public List<Event> GetRepeating()
    {
        using var connection = _db.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM Events WHERE Repeat != 0 ORDER BY StartDate";
        return ReadAll(command);
    }

    public List<Event> GetByRange(DateTime start, DateTime end)
    {
        using var connection = _db.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT * FROM Events
            WHERE StartDate < @end AND COALESCE(EndDate, StartDate) >= @start
            ORDER BY StartDate
            """;
        command.Parameters.AddWithValue("@start", start.ToString("O"));
        command.Parameters.AddWithValue("@end", end.ToString("O"));
        return ReadAll(command);
    }

    public Event? GetById(int id)
    {
        using var connection = _db.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM Events WHERE Id = @id";
        command.Parameters.AddWithValue("@id", id);
        using var reader = command.ExecuteReader();
        return reader.Read() ? Map(reader) : null;
    }

    public int Add(Event item)
    {
        using var connection = _db.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Events (Title, Description, StartDate, EndDate, AllDay, Category, Color, Repeat, RepeatUntil, CreatedAt)
            VALUES (@title, @description, @startDate, @endDate, @allDay, @category, @color, @repeat, @repeatUntil, @createdAt);
            SELECT last_insert_rowid();
            """;
        command.Parameters.AddWithValue("@title", item.Title);
        command.Parameters.AddWithValue("@description", (object?)item.Description ?? DBNull.Value);
        command.Parameters.AddWithValue("@startDate", item.StartDate.ToString("O"));
        command.Parameters.AddWithValue("@endDate", item.EndDate.HasValue ? item.EndDate.Value.ToString("O") : DBNull.Value);
        command.Parameters.AddWithValue("@allDay", item.AllDay ? 1 : 0);
        command.Parameters.AddWithValue("@category", (object?)item.Category ?? DBNull.Value);
        command.Parameters.AddWithValue("@color", (object?)item.Color ?? DBNull.Value);
        command.Parameters.AddWithValue("@repeat", (int)item.Repeat);
        command.Parameters.AddWithValue("@repeatUntil", item.RepeatUntil.HasValue ? item.RepeatUntil.Value.ToString("O") : DBNull.Value);
        command.Parameters.AddWithValue("@createdAt", item.CreatedAt.ToString("O"));

        item.Id = Convert.ToInt32(command.ExecuteScalar());
        return item.Id;
    }

    public void Update(Event item)
    {
        using var connection = _db.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE Events SET
                Title = @title,
                Description = @description,
                StartDate = @startDate,
                EndDate = @endDate,
                AllDay = @allDay,
                Category = @category,
                Color = @color,
                Repeat = @repeat,
                RepeatUntil = @repeatUntil
            WHERE Id = @id
            """;
        command.Parameters.AddWithValue("@title", item.Title);
        command.Parameters.AddWithValue("@description", (object?)item.Description ?? DBNull.Value);
        command.Parameters.AddWithValue("@startDate", item.StartDate.ToString("O"));
        command.Parameters.AddWithValue("@endDate", item.EndDate.HasValue ? item.EndDate.Value.ToString("O") : DBNull.Value);
        command.Parameters.AddWithValue("@allDay", item.AllDay ? 1 : 0);
        command.Parameters.AddWithValue("@category", (object?)item.Category ?? DBNull.Value);
        command.Parameters.AddWithValue("@color", (object?)item.Color ?? DBNull.Value);
        command.Parameters.AddWithValue("@repeat", (int)item.Repeat);
        command.Parameters.AddWithValue("@repeatUntil", item.RepeatUntil.HasValue ? item.RepeatUntil.Value.ToString("O") : DBNull.Value);
        command.Parameters.AddWithValue("@id", item.Id);
        command.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var connection = _db.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Events WHERE Id = @id";
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();
    }

    private static List<Event> ReadAll(SqliteCommand command)
    {
        var result = new List<Event>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            result.Add(Map(reader));
        }
        return result;
    }

    private static Event Map(SqliteDataReader reader)
    {
        return new Event
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            Title = reader.GetString(reader.GetOrdinal("Title")),
            Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
            StartDate = ParseDate(reader.GetString(reader.GetOrdinal("StartDate"))),
            EndDate = reader.IsDBNull(reader.GetOrdinal("EndDate")) ? null : ParseDate(reader.GetString(reader.GetOrdinal("EndDate"))),
            AllDay = reader.GetInt32(reader.GetOrdinal("AllDay")) == 1,
            Category = reader.IsDBNull(reader.GetOrdinal("Category")) ? null : reader.GetString(reader.GetOrdinal("Category")),
            Color = reader.IsDBNull(reader.GetOrdinal("Color")) ? null : reader.GetString(reader.GetOrdinal("Color")),
            Repeat = (RepeatRule)reader.GetInt32(reader.GetOrdinal("Repeat")),
            RepeatUntil = reader.IsDBNull(reader.GetOrdinal("RepeatUntil")) ? null : ParseDate(reader.GetString(reader.GetOrdinal("RepeatUntil"))),
            CreatedAt = ParseDate(reader.GetString(reader.GetOrdinal("CreatedAt")))
        };
    }

    internal static DateTime ParseDate(string value) =>
        DateTime.Parse(value, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.RoundtripKind);
}
