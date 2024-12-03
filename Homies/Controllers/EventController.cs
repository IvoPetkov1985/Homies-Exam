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

        public EventController(IEventService _service)
        {
            service = _service;
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            IEnumerable<EventViewModel> model = await service.GetAllEventsAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            IEnumerable<TypeViewModel> types = await service.GetTypesAsync();

            EventFormModel model = new()
            {
                Types = types
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(EventFormModel model)
        {
            if (DateTime.TryParseExact(model.Start, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime start) == false)
            {
                ModelState.AddModelError(nameof(model.Start), DateTimeInvalid);
            }

            if (DateTime.TryParseExact(model.End, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime end) == false)
            {
                ModelState.AddModelError(nameof(model.End), DateTimeInvalid);
            }

            if (start > end)
            {
                ModelState.AddModelError(nameof(model.End), DateStartAfterTheEnd);
            }

            IEnumerable<TypeViewModel> types = await service.GetTypesAsync();

            if (types.Any(t => t.Id == model.TypeId) == false)
            {
                ModelState.AddModelError(nameof(model.TypeId), TypeInvalidErrorMsg);
            }

            if (ModelState.IsValid == false)
            {
                model.Types = types;

                return View(model);
            }

            string userId = GetUserId();

            await service.CreateEventAsync(model, userId);

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (await service.IsEventOnListAsync(id) == false)
            {
                return BadRequest();
            }

            string userId = GetUserId();

            if (await service.IsUserAuthorisedAsync(userId, id) == false)
            {
                return Unauthorized();
            }

            EventFormModel model = await service.CreateEditModelAsync(id);

            IEnumerable<TypeViewModel> types = await service.GetTypesAsync();

            model.Types = types;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EventFormModel model, int id)
        {
            if (await service.IsEventOnListAsync(id) == false)
            {
                return BadRequest();
            }

            string userId = GetUserId();

            if (await service.IsUserAuthorisedAsync(userId, id) == false)
            {
                return Unauthorized();
            }

            if (DateTime.TryParseExact(model.Start, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime start) == false)
            {
                ModelState.AddModelError(nameof(model.Start), DateTimeInvalid);
            }

            if (DateTime.TryParseExact(model.End, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime end) == false)
            {
                ModelState.AddModelError(nameof(model.End), DateTimeInvalid);
            }

            if (start > end)
            {
                ModelState.AddModelError(nameof(model.End), DateStartAfterTheEnd);
            }

            IEnumerable<TypeViewModel> types = await service.GetTypesAsync();

            if (ModelState.IsValid == false)
            {
                model.Types = types;

                return View(model);
            }

            await service.UpdateExistingEventAsync(model, id);

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Joined()
        {
            string userId = GetUserId();

            IEnumerable<EventViewModel> model = await service.GetJoinedEventsAsync(userId);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Join(int id)
        {
            if (await service.IsEventOnListAsync(id) == false)
            {
                return BadRequest();
            }

            string userId = GetUserId();

            if (await service.IsUserAuthorisedAsync(userId, id))
            {
                return Forbid();
            }

            await service.JoinAnEventAsync(userId, id);

            return RedirectToAction(nameof(Joined));
        }

        [HttpPost]
        public async Task<IActionResult> Leave(int id)
        {
            if (await service.IsEventOnListAsync(id) == false)
            {
                return BadRequest();
            }

            string userId = GetUserId();

            await service.LeaveAnEventAsync(userId, id);

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (await service.IsEventOnListAsync(id) == false)
            {
                return BadRequest();
            }

            EventDetailsViewModel model = await service.CreateDetailsModelAsync(id);

            return View(model);
        }
    }
}
