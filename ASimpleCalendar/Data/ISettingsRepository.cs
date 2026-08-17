namespace ASimpleCalendar.Data;

public interface ISettingsRepository
{
    string? Get(string key);
    void Set(string key, string value);
}
