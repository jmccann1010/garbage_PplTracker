using PplTracker.Core.Models;

namespace PplTracker.Core.Interfaces;

public interface ILocationRepository : IRepository<Location>
{
    Task<IEnumerable<Location>> SearchAsync(string searchTerm);
}
