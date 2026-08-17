using ASimpleCalendar.Models;

namespace ASimpleCalendar.Data;

public interface INoteRepository
{
    List<Note> GetAll();
    Note? GetById(int id);
    int Add(Note item);
    void Update(Note item);
    void Delete(int id);
}
