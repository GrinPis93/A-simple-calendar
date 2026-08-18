using ASimpleCalendar.Models;
using ASimpleCalendar.Services;
using Xunit;

namespace ASimpleCalendar.Tests;

public class EventOccurrenceServiceTests
{
    private static Event CreateEvent(DateTime start, RepeatRule repeat, DateTime? until = null, DateTime? end = null)
    {
        return new Event
        {
            Title = "Событие",
            StartDate = start,
            EndDate = end,
            Repeat = repeat,
            RepeatUntil = until
        };
    }

    [Fact]
    public void Daily_ExpandsWithinRange()
    {
        var ev = CreateEvent(new DateTime(2026, 1, 1, 9, 0, 0), RepeatRule.Daily);

        var result = EventOccurrenceService
            .Expand(ev, new DateTime(2026, 1, 3), new DateTime(2026, 1, 5))
            .ToList();

        Assert.Equal(3, result.Count);
        Assert.Equal(new DateTime(2026, 1, 3, 9, 0, 0), result[0].StartDate);
        Assert.Equal(new DateTime(2026, 1, 5, 9, 0, 0), result[2].StartDate);
    }

    [Fact]
    public void Weekly_StepsBySevenDays()
    {
        var ev = CreateEvent(new DateTime(2026, 1, 5, 10, 0, 0), RepeatRule.Weekly);

        var result = EventOccurrenceService
            .Expand(ev, new DateTime(2026, 1, 12), new DateTime(2026, 1, 18))
            .ToList();

        Assert.Single(result);
        Assert.Equal(new DateTime(2026, 1, 12, 10, 0, 0), result[0].StartDate);
    }

    [Fact]
    public void Monthly_ClampsToEndOfMonth()
    {
        var ev = CreateEvent(new DateTime(2026, 1, 31, 8, 0, 0), RepeatRule.Monthly);

        var result = EventOccurrenceService
            .Expand(ev, new DateTime(2026, 2, 1), new DateTime(2026, 2, 28))
            .ToList();

        Assert.Single(result);
        Assert.Equal(new DateTime(2026, 2, 28, 8, 0, 0), result[0].StartDate);
    }

    [Fact]
    public void None_ProducesSingleOccurrence()
    {
        var ev = CreateEvent(new DateTime(2026, 1, 10, 9, 0, 0), RepeatRule.None);

        var result = EventOccurrenceService
            .Expand(ev, new DateTime(2026, 1, 1), new DateTime(2026, 1, 31))
            .ToList();

        Assert.Single(result);
        Assert.Equal(new DateTime(2026, 1, 10, 9, 0, 0), result[0].StartDate);
    }

    [Fact]
    public void RepeatUntil_LimitsOccurrences()
    {
        var ev = CreateEvent(new DateTime(2026, 1, 1, 9, 0, 0), RepeatRule.Daily, until: new DateTime(2026, 1, 3));

        var result = EventOccurrenceService
            .Expand(ev, new DateTime(2026, 1, 1), new DateTime(2026, 1, 10))
            .ToList();

        Assert.Equal(3, result.Count);
        Assert.Equal(new DateTime(2026, 1, 3, 9, 0, 0), result[2].StartDate);
    }

    [Fact]
    public void CloneForDate_PreservesDuration()
    {
        var ev = CreateEvent(new DateTime(2026, 1, 1, 9, 0, 0), RepeatRule.None, end: new DateTime(2026, 1, 1, 10, 30, 0));

        var clone = EventOccurrenceService.CloneForDate(ev, new DateTime(2026, 1, 5, 12, 0, 0));

        Assert.Equal(new DateTime(2026, 1, 5, 12, 0, 0), clone.StartDate);
        Assert.Equal(new DateTime(2026, 1, 5, 13, 30, 0), clone.EndDate);
    }
}

public class RepeatOptionListTests
{
    [Fact]
    public void ContainsAllRepeatRules()
    {
        var values = RepeatOptionList.All.Select(o => o.Value).ToList();

        Assert.Contains(RepeatRule.None, values);
        Assert.Contains(RepeatRule.Daily, values);
        Assert.Contains(RepeatRule.Weekly, values);
        Assert.Contains(RepeatRule.Monthly, values);
        Assert.Contains(RepeatRule.Yearly, values);
    }
}

public class ThemeHelperTests
{
    [Theory]
    [InlineData(null, ThemeMode.Dark)]
    [InlineData("dark", ThemeMode.Dark)]
    [InlineData("light", ThemeMode.Light)]
    [InlineData("auto", ThemeMode.Auto)]
    [InlineData("unknown", ThemeMode.Dark)]
    public void Parse_ReturnsExpectedMode(string? value, ThemeMode expected)
    {
        Assert.Equal(expected, ThemeHelper.Parse(value));
    }

    [Fact]
    public void ToString_RoundTrips()
    {
        Assert.Equal("dark", ThemeHelper.ToString(ThemeMode.Dark));
        Assert.Equal("light", ThemeHelper.ToString(ThemeMode.Light));
        Assert.Equal("auto", ThemeHelper.ToString(ThemeMode.Auto));
    }
}
