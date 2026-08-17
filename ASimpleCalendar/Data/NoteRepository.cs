using ASimpleCalendar.Models;
using Microsoft.Data.Sqlite;

namespace ASimpleCalendar.Data;

public class NoteRepository : INoteRepository
{
    private readonly DatabaseService _db;

    public NoteRepository(DatabaseService db)
    {
        _db = db;
    }

    public List<Note> GetAll()
    {
        using var connection = _db.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM Notes ORDER BY IsPinned DESC, UpdatedAt DESC";
        return ReadAll(command);
    }

    public Note? GetById(int id)
    {
        using var connection = _db.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM Notes WHERE Id = @id";
        command.Parameters.AddWithValue("@id", id);
        using var reader = command.ExecuteReader();
        return reader.Read() ? Map(reader) : null;
    }

    public int Add(Note item)
    {
        using var connection = _db.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Notes (Title, Content, Color, Tags, Category, IsPinned, CreatedAt, UpdatedAt)
            VALUES (@title, @content, @color, @tags, @category, @isPinned, @createdAt, @updatedAt);
            SELECT last_insert_rowid();
            """;
        command.Parameters.AddWithValue("@title", item.Title);
        command.Parameters.AddWithValue("@content", item.Content);
        command.Parameters.AddWithValue("@color", (object?)item.Color ?? DBNull.Value);
        command.Parameters.AddWithValue("@tags", (object?)item.Tags ?? DBNull.Value);
        command.Parameters.AddWithValue("@category", (object?)item.Category ?? DBNull.Value);
        command.Parameters.AddWithValue("@isPinned", item.IsPinned ? 1 : 0);
        command.Parameters.AddWithValue("@createdAt", item.CreatedAt.ToString("O"));
        command.Parameters.AddWithValue("@updatedAt", item.UpdatedAt.ToString("O"));

        item.Id = Convert.ToInt32(command.ExecuteScalar());
        return item.Id;
    }

    public void Update(Note item)
    {
        using var connection = _db.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE Notes SET
                Title = @title,
                Content = @content,
                Color = @color,
                Tags = @tags,
                Category = @category,
                IsPinned = @isPinned,
                UpdatedAt = @updatedAt
            WHERE Id = @id
            """;
        command.Parameters.AddWithValue("@title", item.Title);
        command.Parameters.AddWithValue("@content", item.Content);
        command.Parameters.AddWithValue("@color", (object?)item.Color ?? DBNull.Value);
        command.Parameters.AddWithValue("@tags", (object?)item.Tags ?? DBNull.Value);
        command.Parameters.AddWithValue("@category", (object?)item.Category ?? DBNull.Value);
        command.Parameters.AddWithValue("@isPinned", item.IsPinned ? 1 : 0);
        command.Parameters.AddWithValue("@updatedAt", item.UpdatedAt.ToString("O"));
        command.Parameters.AddWithValue("@id", item.Id);
        command.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var connection = _db.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Notes WHERE Id = @id";
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();
    }

    private static List<Note> ReadAll(SqliteCommand command)
    {
        var result = new List<Note>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            result.Add(Map(reader));
        }
        return result;
    }

    private static Note Map(SqliteDataReader reader)
    {
        return new Note
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            Title = reader.GetString(reader.GetOrdinal("Title")),
            Content = reader.GetString(reader.GetOrdinal("Content")),
            Color = reader.IsDBNull(reader.GetOrdinal("Color")) ? null : reader.GetString(reader.GetOrdinal("Color")),
            Tags = reader.IsDBNull(reader.GetOrdinal("Tags")) ? null : reader.GetString(reader.GetOrdinal("Tags")),
            Category = reader.IsDBNull(reader.GetOrdinal("Category")) ? null : reader.GetString(reader.GetOrdinal("Category")),
            IsPinned = reader.GetInt32(reader.GetOrdinal("IsPinned")) == 1,
            CreatedAt = EventRepository.ParseDate(reader.GetString(reader.GetOrdinal("CreatedAt"))),
            UpdatedAt = EventRepository.ParseDate(reader.GetString(reader.GetOrdinal("UpdatedAt")))
        };
    }
}
