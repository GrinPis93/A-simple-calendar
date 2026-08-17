using System.Text.Json;
using ASimpleCalendar.Data;
using ASimpleCalendar.Models;

namespace ASimpleCalendar.Services;

public class CategoryService
{
    private readonly ISettingsRepository _settings;

    public CategoryService(ISettingsRepository settings)
    {
        _settings = settings;
    }

    public List<CategoryItem> GetCategories()
    {
        var json = _settings.Get("categories");
        if (string.IsNullOrWhiteSpace(json))
        {
            return CategoryPalette.Items.ToList();
        }

        try
        {
            var list = JsonSerializer.Deserialize<List<CategoryItem>>(json);
            if (list is { Count: > 0 })
            {
                return list;
            }
        }
        catch
        {
            // повреждённые данные — вернём категории по умолчанию
        }

        return CategoryPalette.Items.ToList();
    }

    public void SaveCategories(IEnumerable<CategoryItem> categories)
    {
        _settings.Set("categories", JsonSerializer.Serialize(categories.ToList()));
    }
}
