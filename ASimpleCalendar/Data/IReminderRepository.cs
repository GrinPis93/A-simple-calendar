using ASimpleCalendar.Models;

namespace ASimpleCalendar.Data;

public interface IReminderRepository
{
    List<Reminder> GetAll();
    List<Reminder> GetActive();
    Reminder? GetById(int id);
    int Add(Reminder item);
    void Update(Reminder item);
    void Delete(int id);
}
