using Microsoft.EntityFrameworkCore;
using PplTracker.Core.Models;
using PplTracker.Data.Context;
using PplTracker.Data.Repositories;
using Xunit;

namespace PplTracker.Tests;

public class LocationRepositoryTests
{
    private PplTrackerDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<PplTrackerDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        return new PplTrackerDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddLocation()
    {
        using var context = CreateContext(nameof(CreateAsync_ShouldAddLocation));
        var repo = new LocationRepository(context);

        var location = new Location { Name = "Home", City = "Springfield", State = "IL" };
        var result = await repo.CreateAsync(location);

        Assert.NotEqual(0, result.Id);
        Assert.Equal("Home", result.Name);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllLocations()
    {
        using var context = CreateContext(nameof(GetAllAsync_ShouldReturnAllLocations));
        var repo = new LocationRepository(context);

        await repo.CreateAsync(new Location { Name = "Office" });
        await repo.CreateAsync(new Location { Name = "Gym" });

        var locations = await repo.GetAllAsync();

        Assert.Equal(2, locations.Count());
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnMatchingLocations()
    {
        using var context = CreateContext(nameof(SearchAsync_ShouldReturnMatchingLocations));
        var repo = new LocationRepository(context);

        await repo.CreateAsync(new Location { Name = "Coffee Shop", City = "Seattle" });
        await repo.CreateAsync(new Location { Name = "Library", City = "Portland" });

        var results = await repo.SearchAsync("seattle");

        Assert.Single(results);
        Assert.Equal("Coffee Shop", results.First().Name);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveLocation()
    {
        using var context = CreateContext(nameof(DeleteAsync_ShouldRemoveLocation));
        var repo = new LocationRepository(context);

        var created = await repo.CreateAsync(new Location { Name = "Work" });
        var result = await repo.DeleteAsync(created.Id);

        Assert.True(result);
        Assert.Null(await repo.GetByIdAsync(created.Id));
    }
}
