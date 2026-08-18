using System.IO;
using System.Text.Json;
using ASimpleCalendar.Data;
using ASimpleCalendar.Models;

namespace ASimpleCalendar.Services;

public class DataExportService
{
    private readonly IEventRepository _events;
    private readonly INoteRepository _notes;
    private readonly IReminderRepository _reminders;
    private readonly ISettingsRepository _settings;

    public DataExportService(
        IEventRepository events,
        INoteRepository notes,
        IReminderRepository reminders,
        ISettingsRepository settings)
    {
        _events = events;
        _notes = notes;
        _reminders = reminders;
        _settings = settings;
    }

    public void ExportToFile(string path)
    {
        var data = new ExportData
        {
            Version = 1,
            Events = _events.GetAll(),
            Notes = _notes.GetAll(),
            Reminders = _reminders.GetAll(),
            Settings = _settings.GetAll()
        };

        var options = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(path, JsonSerializer.Serialize(data, options));
    }

    public void ImportFromFile(string path)
    {
        ExportData? data;
        try
        {
            var json = File.ReadAllText(path);
            data = JsonSerializer.Deserialize<ExportData>(json);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Не удалось прочитать файл импорта: " + ex.Message, ex);
        }

        if (data is null)
        {
            throw new InvalidDataException("Файл импорта пуст или имеет неверный формат.");
        }

        foreach (var item in _events.GetAll())
        {
            _events.Delete(item.Id);
        }

        foreach (var item in _notes.GetAll())
        {
            _notes.Delete(item.Id);
        }

        foreach (var item in _reminders.GetAll())
        {
            _reminders.Delete(item.Id);
        }

        foreach (var ev in data.Events)
        {
            ev.Id = 0;
            _events.Add(ev);
        }

        foreach (var note in data.Notes)
        {
            note.Id = 0;
            _notes.Add(note);
        }

        foreach (var reminder in data.Reminders)
        {
            reminder.Id = 0;
            _reminders.Add(reminder);
        }

        foreach (var (key, value) in data.Settings)
        {
            _settings.Set(key, value);
        }
    }

    public string CreateBackup()
    {
        var directory = BackupDirectory;
        Directory.CreateDirectory(directory);

        var path = Path.Combine(directory, $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.json");
        ExportToFile(path);
        return path;
    }

    public void EnsureDailyBackup()
    {
        var today = DateTime.Today.ToString("yyyy-MM-dd");
        if (_settings.Get("lastBackup") == today)
        {
            return;
        }

        CreateBackup();
        _settings.Set("lastBackup", today);
        PruneOldBackups(keep: 10);
    }

    private static string BackupDirectory => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "ASimpleCalendar",
        "backups");

    private static void PruneOldBackups(int keep)
    {
        try
        {
            if (!Directory.Exists(BackupDirectory))
            {
                return;
            }

            var files = Directory.GetFiles(BackupDirectory, "backup_*.json")
                .OrderByDescending(f => f)
                .ToList();

            foreach (var file in files.Skip(keep))
            {
                File.Delete(file);
            }
        }
        catch
        {
            // очистка старых бэкапов не критична
        }
    }
}
