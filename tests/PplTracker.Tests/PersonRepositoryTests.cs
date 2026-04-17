using Microsoft.EntityFrameworkCore;
using PplTracker.Core.Models;
using PplTracker.Data.Context;
using PplTracker.Data.Repositories;
using Xunit;

namespace PplTracker.Tests;

public class PersonRepositoryTests
{
    private PplTrackerDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<PplTrackerDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        return new PplTrackerDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddPerson()
    {
        using var context = CreateContext(nameof(CreateAsync_ShouldAddPerson));
        var repo = new PersonRepository(context);

        var person = new Person { FirstName = "John", LastName = "Doe", Email = "john@example.com" };
        var result = await repo.CreateAsync(person);

        Assert.NotEqual(0, result.Id);
        Assert.Equal("John", result.FirstName);
        Assert.Equal("Doe", result.LastName);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllPeople()
    {
        using var context = CreateContext(nameof(GetAllAsync_ShouldReturnAllPeople));
        var repo = new PersonRepository(context);

        await repo.CreateAsync(new Person { FirstName = "Alice", LastName = "Smith" });
        await repo.CreateAsync(new Person { FirstName = "Bob", LastName = "Jones" });

        var people = await repo.GetAllAsync();

        Assert.Equal(2, people.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnPerson_WhenExists()
    {
        using var context = CreateContext(nameof(GetByIdAsync_ShouldReturnPerson_WhenExists));
        var repo = new PersonRepository(context);

        var created = await repo.CreateAsync(new Person { FirstName = "Jane", LastName = "Doe" });
        var person = await repo.GetByIdAsync(created.Id);

        Assert.NotNull(person);
        Assert.Equal("Jane", person.FirstName);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        using var context = CreateContext(nameof(GetByIdAsync_ShouldReturnNull_WhenNotExists));
        var repo = new PersonRepository(context);

        var person = await repo.GetByIdAsync(999);

        Assert.Null(person);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdatePerson()
    {
        using var context = CreateContext(nameof(UpdateAsync_ShouldUpdatePerson));
        var repo = new PersonRepository(context);

        var created = await repo.CreateAsync(new Person { FirstName = "John", LastName = "Doe" });
        created.FirstName = "Jonathan";
        await repo.UpdateAsync(created);

        var updated = await repo.GetByIdAsync(created.Id);
        Assert.Equal("Jonathan", updated?.FirstName);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemovePerson()
    {
        using var context = CreateContext(nameof(DeleteAsync_ShouldRemovePerson));
        var repo = new PersonRepository(context);

        var created = await repo.CreateAsync(new Person { FirstName = "John", LastName = "Doe" });
        var result = await repo.DeleteAsync(created.Id);

        Assert.True(result);
        Assert.Null(await repo.GetByIdAsync(created.Id));
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenNotExists()
    {
        using var context = CreateContext(nameof(DeleteAsync_ShouldReturnFalse_WhenNotExists));
        var repo = new PersonRepository(context);

        var result = await repo.DeleteAsync(999);

        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnMatchingPeople()
    {
        using var context = CreateContext(nameof(SearchAsync_ShouldReturnMatchingPeople));
        var repo = new PersonRepository(context);

        await repo.CreateAsync(new Person { FirstName = "Alice", LastName = "Smith", Email = "alice@example.com" });
        await repo.CreateAsync(new Person { FirstName = "Bob", LastName = "Jones", Email = "bob@example.com" });

        var results = await repo.SearchAsync("alice");

        Assert.Single(results);
        Assert.Equal("Alice", results.First().FirstName);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenPersonExists()
    {
        using var context = CreateContext(nameof(ExistsAsync_ShouldReturnTrue_WhenPersonExists));
        var repo = new PersonRepository(context);

        var created = await repo.CreateAsync(new Person { FirstName = "John", LastName = "Doe" });

        Assert.True(await repo.ExistsAsync(created.Id));
        Assert.False(await repo.ExistsAsync(999));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnPeopleOrderedByLastNameThenFirstName()
    {
        using var context = CreateContext(nameof(GetAllAsync_ShouldReturnPeopleOrderedByLastNameThenFirstName));
        var repo = new PersonRepository(context);

        await repo.CreateAsync(new Person { FirstName = "Bob", LastName = "Smith" });
        await repo.CreateAsync(new Person { FirstName = "Alice", LastName = "Jones" });
        await repo.CreateAsync(new Person { FirstName = "Charlie", LastName = "Jones" });

        var people = (await repo.GetAllAsync()).ToList();

        Assert.Equal("Jones", people[0].LastName);
        Assert.Equal("Alice", people[0].FirstName);
        Assert.Equal("Jones", people[1].LastName);
        Assert.Equal("Charlie", people[1].FirstName);
        Assert.Equal("Smith", people[2].LastName);
    }
}
