using Microsoft.EntityFrameworkCore;
using PplTracker.Core.Models;
using PplTracker.Data.Context;
using PplTracker.Data.Repositories;
using Xunit;

namespace PplTracker.Tests;

public class ScheduleRepositoryTests
{
    private PplTrackerDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<PplTrackerDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        return new PplTrackerDbContext(options);
    }

    private async Task<Person> CreatePersonAsync(PplTrackerDbContext context)
    {
        var repo = new PersonRepository(context);
        return await repo.CreateAsync(new Person { FirstName = "Test", LastName = "User" });
    }

    [Fact]
    public async Task CreateAsync_ShouldAddSchedule()
    {
        using var context = CreateContext(nameof(CreateAsync_ShouldAddSchedule));
        var person = await CreatePersonAsync(context);
        var repo = new ScheduleRepository(context);

        var schedule = new Schedule
        {
            Title = "Doctor Appointment",
            StartTime = DateTime.UtcNow.AddDays(1),
            PersonId = person.Id
        };

        var result = await repo.CreateAsync(schedule);

        Assert.NotEqual(0, result.Id);
        Assert.Equal("Doctor Appointment", result.Title);
    }

    [Fact]
    public async Task GetByPersonAsync_ShouldReturnPersonSchedules()
    {
        using var context = CreateContext(nameof(GetByPersonAsync_ShouldReturnPersonSchedules));
        var person1 = await CreatePersonAsync(context);
        var repo2 = new PersonRepository(context);
        var person2 = await repo2.CreateAsync(new Person { FirstName = "Other", LastName = "Person" });

        var scheduleRepo = new ScheduleRepository(context);
        await scheduleRepo.CreateAsync(new Schedule { Title = "Event 1", StartTime = DateTime.UtcNow, PersonId = person1.Id });
        await scheduleRepo.CreateAsync(new Schedule { Title = "Event 2", StartTime = DateTime.UtcNow, PersonId = person1.Id });
        await scheduleRepo.CreateAsync(new Schedule { Title = "Event 3", StartTime = DateTime.UtcNow, PersonId = person2.Id });

        var schedules = await scheduleRepo.GetByPersonAsync(person1.Id);

        Assert.Equal(2, schedules.Count());
        Assert.All(schedules, s => Assert.Equal(person1.Id, s.PersonId));
    }

    [Fact]
    public async Task GetByDateRangeAsync_ShouldReturnSchedulesInRange()
    {
        using var context = CreateContext(nameof(GetByDateRangeAsync_ShouldReturnSchedulesInRange));
        var person = await CreatePersonAsync(context);
        var repo = new ScheduleRepository(context);

        var now = DateTime.UtcNow;
        await repo.CreateAsync(new Schedule { Title = "Past", StartTime = now.AddDays(-10), PersonId = person.Id });
        await repo.CreateAsync(new Schedule { Title = "Today", StartTime = now, PersonId = person.Id });
        await repo.CreateAsync(new Schedule { Title = "Future", StartTime = now.AddDays(10), PersonId = person.Id });

        var results = await repo.GetByDateRangeAsync(now.AddDays(-1), now.AddDays(1));

        Assert.Single(results);
        Assert.Equal("Today", results.First().Title);
    }
}
