using ASimpleCalendar.Models;

namespace ASimpleCalendar.Services;

public static class EventOccurrenceService
{
    public static IEnumerable<Event> Expand(Event ev, DateTime start, DateTime end)
    {
        var limit = ev.RepeatUntil ?? end;
        if (limit > end)
        {
            limit = end;
        }

        var current = ev.StartDate;
        while (current < start)
        {
            var next = NextOccurrence(current, ev.Repeat);
            if (next <= current)
            {
                yield break;
            }

            current = next;
        }

        while (current <= limit)
        {
            yield return CloneForDate(ev, current);

            var next = NextOccurrence(current, ev.Repeat);
            if (next <= current)
            {
                yield break;
            }

            current = next;
        }
    }

    public static DateTime NextOccurrence(DateTime current, RepeatRule repeat) => repeat switch
    {
        RepeatRule.Daily => current.AddDays(1),
        RepeatRule.Weekly => current.AddDays(7),
        RepeatRule.Monthly => current.AddMonths(1),
        RepeatRule.Yearly => current.AddYears(1),
        _ => current
    };

    public static Event CloneForDate(Event ev, DateTime date)
    {
        var duration = ev.EndDate.HasValue ? ev.EndDate.Value - ev.StartDate : TimeSpan.Zero;

        return new Event
        {
            Id = ev.Id,
            Title = ev.Title,
            Description = ev.Description,
            StartDate = date,
            EndDate = ev.EndDate.HasValue ? date + duration : null,
            AllDay = ev.AllDay,
            Category = ev.Category,
            Color = ev.Color,
            Repeat = ev.Repeat,
            RepeatUntil = ev.RepeatUntil,
            CreatedAt = ev.CreatedAt
        };
    }
}
