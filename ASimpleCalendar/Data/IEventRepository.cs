using ASimpleCalendar.Models;

namespace ASimpleCalendar.Data;

public interface IEventRepository
{
    List<Event> GetAll();
    List<Event> GetRepeating();
    List<Event> GetPendingReminders();
    List<Event> GetByRange(DateTime start, DateTime end);
    Event? GetById(int id);
    int Add(Event item);
    void Update(Event item);
    void Delete(int id);
}
