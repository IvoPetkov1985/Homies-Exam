using Homies.Contracts;
using Homies.Data;
using Homies.Data.DataModels;
using Homies.Models;
using Microsoft.EntityFrameworkCore;
using static Homies.Data.Common.DataConstants;

namespace Homies.Services
{
    public class EventService : IEventService
    {
        private readonly HomiesDbContext context;

        public EventService(HomiesDbContext _context)
        {
            context = _context;
        }

        public async Task<EventDetailsViewModel> CreateDetailsModelAsync(int id)
        {
            EventDetailsViewModel details = await context.Events
                .AsNoTracking()
                .Where(e => e.Id == id)
                .Select(e => new EventDetailsViewModel()
                {
                    Id = e.Id,
                    Name = e.Name,
                    Description = e.Description,
                    CreatedOn = e.CreatedOn.ToString(DateTimeFormat),
                    Start = e.Start.ToString(DateTimeFormat),
                    End = e.End.ToString(DateTimeFormat),
                    Type = e.Type.Name,
                    Organiser = e.Organiser.UserName
                })
                .SingleAsync();

            return details;
        }

        public async Task<EventFormModel> CreateEditModelAsync(int id)
        {
            EventFormModel eventToEdit = await context.Events
                .AsNoTracking()
                .Where(e => e.Id == id)
                .Select(e => new EventFormModel()
                {
                    Name = e.Name,
                    Description = e.Description,
                    Start = e.Start.ToString(DateTimeFormat),
                    End = e.Start.ToString(DateTimeFormat),
                    TypeId = e.TypeId
                })
                .SingleAsync();

            return eventToEdit;
        }

        public async Task CreateEventAsync(EventFormModel model, string userId)
        {
            Event eventToAdd = new()
            {
                Name = model.Name,
                Description = model.Description,
                CreatedOn = DateTime.Now,
                Start = DateTime.Parse(model.Start),
                End = DateTime.Parse(model.End),
                TypeId = model.TypeId,
                OrganiserId = userId
            };

            await context.Events.AddAsync(eventToAdd);

            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<EventViewModel>> GetAllEventsAsync()
        {
            IEnumerable<EventViewModel> allEvents = await context.Events
                .AsNoTracking()
                .Select(e => new EventViewModel()
                {
                    Id = e.Id,
                    Name = e.Name,
                    Start = e.Start.ToString(DateTimeFormat),
                    Type = e.Type.Name,
                    Organiser = e.Organiser.UserName
                })
                .ToListAsync();

            return allEvents;
        }

        public async Task<IEnumerable<EventViewModel>> GetJoinedEventsAsync(string userId)
        {
            IEnumerable<EventViewModel> joinedEvents = await context.EventsParticipants
                .AsNoTracking()
                .Where(ep => ep.HelperId == userId)
                .Select(ep => new EventViewModel()
                {
                    Id = ep.Event.Id,
                    Name = ep.Event.Name,
                    Start = ep.Event.Start.ToString(DateTimeFormat),
                    Type = ep.Event.Type.Name,
                    Organiser = ep.Event.Organiser.UserName
                })
                .ToListAsync();

            return joinedEvents;
        }

        public async Task<IEnumerable<TypeViewModel>> GetTypesAsync()
        {
            IEnumerable<TypeViewModel> types = await context.Types
                .AsNoTracking()
                .Select(t => new TypeViewModel()
                {
                    Id = t.Id,
                    Name = t.Name
                })
                .ToListAsync();

            return types;
        }

        public async Task<bool> IsEventOnListAsync(int id)
        {
            Event? eventToCheck = await context.Events
                .AsNoTracking()
                .SingleOrDefaultAsync(e => e.Id == id);

            return eventToCheck != null;
        }

        public async Task<bool> IsUserAuthorisedAsync(string userId, int id)
        {
            Event eventToCheck = await context.Events
                .AsNoTracking()
                .SingleAsync(e => e.Id == id);

            return eventToCheck.OrganiserId == userId;
        }

        public async Task JoinAnEventAsync(string userId, int id)
        {
            EventParticipant eventParticipant = new()
            {
                HelperId = userId,
                EventId = id
            };

            if (await context.EventsParticipants.ContainsAsync(eventParticipant) == false)
            {
                await context.EventsParticipants.AddAsync(eventParticipant);

                await context.SaveChangesAsync();
            }
        }

        public async Task LeaveAnEventAsync(string userId, int id)
        {
            EventParticipant eventParticipant = new()
            {
                HelperId = userId,
                EventId = id
            };

            if (await context.EventsParticipants.ContainsAsync(eventParticipant))
            {
                context.EventsParticipants.Remove(eventParticipant);

                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateExistingEventAsync(EventFormModel model, int id)
        {
            Event eventToEdit = await context.Events
                .SingleAsync(e => e.Id == id);

            eventToEdit.Name = model.Name;
            eventToEdit.Description = model.Description;
            eventToEdit.Start = DateTime.Parse(model.Start);
            eventToEdit.End = DateTime.Parse(model.End);
            eventToEdit.TypeId = model.TypeId;

            await context.SaveChangesAsync();
        }
    }
}
