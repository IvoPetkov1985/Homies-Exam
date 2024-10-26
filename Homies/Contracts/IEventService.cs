using Homies.Data.DataModels;
using Homies.Models;

namespace Homies.Contracts
{
    public interface IEventService
    {
        Task<IEnumerable<AllEventViewModel>> GetAllEventsAsync();

        Task<IEnumerable<TypeViewModel>> GetTypesAsync();

        Task CreateNewEventAsync(EventFormViewModel model, string userId);

        Task<Event> GetEntityByIdAsync(int id);

        Task<EventFormViewModel> GetEventModelToEditAsync(Event entity);

        Task EditEventAsync(EventFormViewModel model, int id);

        Task<DetailsEventViewModel> GetDetailsViewModelAsync(int id);

        Task<Event> GetEventToJoinOrLeaveAsync(int id);

        Task JoinEventAsync(string userId, Event eventToJoin);

        Task<IEnumerable<AllEventViewModel>> GetAllJoinedEventsAsync(string userId);

        Task LeaveEventAsync(string userId, int id);
    }
}
