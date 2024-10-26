using Homies.Contracts;
using Homies.Models;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using static Homies.Data.Common.DataConstants;

namespace Homies.Controllers
{
    public class EventController : BaseController
    {
        private readonly IEventService service;

        public EventController(IEventService eventService)
        {
            service = eventService;
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var model = await service.GetAllEventsAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new EventFormViewModel
            {
                Types = await service.GetTypesAsync()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(EventFormViewModel model)
        {
            DateTime start;
            DateTime end;

            if (!DateTime.TryParseExact(model.Start, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out start))
            {
                ModelState.AddModelError(nameof(model.Start), DateTimeErrorMsg);
            }

            if (!DateTime.TryParseExact(model.End, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out end))
            {
                ModelState.AddModelError(nameof(model.End), DateTimeErrorMsg);
            }

            if (!ModelState.IsValid)
            {
                model.Types = await service.GetTypesAsync();
                return View(model);
            }

            string userId = GetUserId();
            await service.CreateNewEventAsync(model, userId);

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var entity = await service.GetEntityByIdAsync(id);

            if (entity == null)
            {
                return BadRequest();
            }

            string userId = GetUserId();

            if (entity.OrganiserId != userId)
            {
                return Unauthorized();
            }

            var model = await service.GetEventModelToEditAsync(entity);

            model.Types = await service.GetTypesAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EventFormViewModel model, int id)
        {
            var eventToEdit = await service.GetEntityByIdAsync(id);

            if (eventToEdit == null)
            {
                return BadRequest();
            }

            string userId = GetUserId();

            if (eventToEdit.OrganiserId != userId)
            {
                return Unauthorized();
            }

            DateTime start;
            DateTime end;

            if (!DateTime.TryParseExact(model.Start, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out start))
            {
                ModelState.AddModelError(nameof(model.Start), DateTimeErrorMsg);
            }

            if (!DateTime.TryParseExact(model.End, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out end))
            {
                ModelState.AddModelError(nameof(model.End), DateTimeErrorMsg);
            }

            if (!ModelState.IsValid)
            {
                model.Types = await service.GetTypesAsync();
                return View(model);
            }

            await service.EditEventAsync(model, id);

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var model = await service.GetDetailsViewModelAsync(id);

            if (model == null)
            {
                return BadRequest();
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Join(int id)
        {
            var eventToJoin = await service.GetEventToJoinOrLeaveAsync(id);

            if (eventToJoin == null)
            {
                return BadRequest();
            }

            string userId = GetUserId();

            await service.JoinEventAsync(userId, eventToJoin);

            return RedirectToAction(nameof(Joined));
        }

        [HttpGet]
        public async Task<IActionResult> Joined()
        {
            string userId = GetUserId();

            var model = await service.GetAllJoinedEventsAsync(userId);

            return View(model);
        }

        public async Task<IActionResult> Leave(int id)
        {
            var existingEvent = await service.GetEventToJoinOrLeaveAsync(id);

            if (existingEvent == null)
            {
                return BadRequest();
            }

            string userId = GetUserId();

            await service.LeaveEventAsync(userId, id);

            return RedirectToAction(nameof(All));
        }
    }
}
