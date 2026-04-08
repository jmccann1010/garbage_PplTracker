using PplTracker.Core.Models;

namespace PplTracker.Core.Interfaces;

public interface IPersonLocationRepository : IRepository<PersonLocation>
{
    Task<IEnumerable<PersonLocation>> GetByPersonAsync(int personId);
    Task<IEnumerable<PersonLocation>> GetByLocationAsync(int locationId);
    Task<IEnumerable<PersonLocation>> GetCurrentlyCheckedInAsync();
    Task<PersonLocation?> CheckOutAsync(int personLocationId);
}
