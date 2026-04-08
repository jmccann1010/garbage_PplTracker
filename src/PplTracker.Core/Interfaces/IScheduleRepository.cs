using PplTracker.Core.Models;

namespace PplTracker.Core.Interfaces;

public interface IScheduleRepository : IRepository<Schedule>
{
    Task<IEnumerable<Schedule>> GetByPersonAsync(int personId);
    Task<IEnumerable<Schedule>> GetByLocationAsync(int locationId);
    Task<IEnumerable<Schedule>> GetByDateRangeAsync(DateTime start, DateTime end);
}
