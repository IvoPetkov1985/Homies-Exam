using Homies.Models;

namespace Homies.Contracts
{
    public interface IEventService
    {
        Task<EventDetailsViewModel> CreateDetailsModelAsync(int id);

        Task<EventFormModel> CreateEditModelAsync(int id);

        Task CreateEventAsync(EventFormModel model, string userId);

        Task<IEnumerable<EventViewModel>> GetAllEventsAsync();

        Task<IEnumerable<EventViewModel>> GetJoinedEventsAsync(string userId);

        Task<IEnumerable<TypeViewModel>> GetTypesAsync();

        Task<bool> IsEventOnListAsync(int id);

        Task<bool> IsUserAuthorisedAsync(string userId, int id);

        Task JoinAnEventAsync(string userId, int id);

        Task LeaveAnEventAsync(string userId, int id);

        Task UpdateExistingEventAsync(EventFormModel model, int id);
    }
}
