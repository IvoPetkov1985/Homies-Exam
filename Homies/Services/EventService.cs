using Homies.Contracts;
using Homies.Data;
using Homies.Data.DataModels;
using Homies.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using static Homies.Data.Common.DataConstants;

namespace Homies.Services
{
    public class EventService : IEventService
    {
        private readonly HomiesDbContext context;

        public EventService(HomiesDbContext dbContext)
        {
            context = dbContext;
        }

        public async Task CreateNewEventAsync(EventFormViewModel model, string userId)
        {
            var newEntity = new Event()
            {
                Name = model.Name,
                Description = model.Description,
                CreatedOn = DateTime.Now,
                Start = DateTime.ParseExact(model.Start, DateTimeFormat, CultureInfo.InvariantCulture),
                End = DateTime.ParseExact(model.End, DateTimeFormat, CultureInfo.InvariantCulture),
                OrganiserId = userId,
                TypeId = model.TypeId
            };

            await context.Events.AddAsync(newEntity);
            await context.SaveChangesAsync();
        }

        public async Task EditEventAsync(EventFormViewModel model, int id)
        {
            var eventToEdit = await context.Events
                .Where(e => e.Id == id)
                .FirstOrDefaultAsync();

            eventToEdit.Name = model.Name;
            eventToEdit.Description = model.Description;
            eventToEdit.Start = DateTime.ParseExact(model.Start, DateTimeFormat, CultureInfo.InvariantCulture);
            eventToEdit.End = DateTime.ParseExact(model.End, DateTimeFormat, CultureInfo.InvariantCulture);
            eventToEdit.TypeId = model.TypeId;

            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AllEventViewModel>> GetAllEventsAsync()
        {
            var allEvents = await context.Events
                .AsNoTracking()
                .Select(e => new AllEventViewModel()
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

        public async Task<IEnumerable<AllEventViewModel>> GetAllJoinedEventsAsync(string userId)
        {
            var joinedEvents = await context.EventsParticipants
                .AsNoTracking()
                .Where(ep => ep.HelperId == userId)
                .Select(ep => new AllEventViewModel()
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

        public async Task<DetailsEventViewModel> GetDetailsViewModelAsync(int id)
        {
            return await context.Events
                .AsNoTracking()
                .Where(e => e.Id == id)
                .Select(e => new DetailsEventViewModel()
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
                .FirstOrDefaultAsync();
        }

        public async Task<Event> GetEntityByIdAsync(int id)
        {
            Event entityToEdit = await context.Events
                .AsNoTracking()
                .Where(e => e.Id == id)
                .FirstOrDefaultAsync();

            return entityToEdit;
        }

        public async Task<EventFormViewModel> GetEventModelToEditAsync(Event entity)
        {
            EventFormViewModel model = new EventFormViewModel()
            {
                Name = entity.Name,
                Description = entity.Description,
                Start = entity.Start.ToString(DateTimeFormat),
                End = entity.End.ToString(DateTimeFormat),
                TypeId = entity.TypeId
            };

            return model;
        }

        public async Task<Event> GetEventToJoinOrLeaveAsync(int id)
        {
            var eventToJoin = await context.Events
                .Where(e => e.Id == id)
                .Include(e => e.EventsParticipants)
                .FirstOrDefaultAsync();

            return eventToJoin;
        }

        public async Task<IEnumerable<TypeViewModel>> GetTypesAsync()
        {
            var types = await context.Types
                .AsNoTracking()
                .Select(t => new TypeViewModel()
                {
                    Id = t.Id,
                    Name = t.Name,
                })
                .ToListAsync();

            return types;
        }

        public async Task JoinEventAsync(string userId, Event eventToJoin)
        {
            if (!eventToJoin.EventsParticipants.Any(e => e.HelperId == userId))
            {
                await context.EventsParticipants.AddAsync(new EventParticipant()
                {
                    HelperId = userId,
                    EventId = eventToJoin.Id
                });
            }

            await context.SaveChangesAsync();
        }

        public async Task LeaveEventAsync(string userId, int id)
        {
            var eventToLeave = await context.EventsParticipants
                .Where(ep => ep.HelperId == userId && ep.EventId == id)
                .FirstOrDefaultAsync();

            if (eventToLeave != null)
            {
                context.EventsParticipants.Remove(eventToLeave);
                await context.SaveChangesAsync();
            }
        }
    }
}
