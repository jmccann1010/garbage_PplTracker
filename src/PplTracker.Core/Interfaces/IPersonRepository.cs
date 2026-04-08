using PplTracker.Core.Models;

namespace PplTracker.Core.Interfaces;

public interface IPersonRepository : IRepository<Person>
{
    Task<IEnumerable<Person>> SearchAsync(string searchTerm);
    Task<IEnumerable<Person>> GetPeopleAtLocationAsync(int locationId);
}
