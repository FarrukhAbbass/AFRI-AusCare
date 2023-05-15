using AFRI_AusCare.Models;
using Microsoft.AspNetCore.Mvc;

namespace AFRI_AusCare.Controllers
{
    public class EventsController : Controller
    {
        private readonly DatabaseContext _databaseContext;

        public EventsController(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.Get("UserId") != null)
            {
                var events = _databaseContext.Events.Where(x => !x.IsDeleted).ToList();
                return View(events);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public IActionResult Create()
        {
            if (HttpContext.Session.Get("UserId") != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public IActionResult Create(Event @event)
        {
            @event.CreatedDate = DateTime.Now;
            @event.ModifiedDate = DateTime.Now;
            @event.IsDeleted = false;
            _databaseContext.Add(@event);
            _databaseContext.SaveChanges();
            return Redirect("Index");
        }

        [HttpPost]
        public IActionResult Edit(Event @event)
        {

            @event.ModifiedDate = DateTime.Now;
            _databaseContext.Update(@event);
            _databaseContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.Get("UserId") != null)
            {
                var events = _databaseContext.Events.SingleOrDefault(x => x.Id == id);
                return View(events);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public IActionResult Delete(int id)
        {
            Event? @event = _databaseContext.Events.SingleOrDefault(x => x.Id == id);
            if (@event != null)
            {
                @event.ModifiedDate = DateTime.Now;
                @event.IsDeleted = true;
                _databaseContext.Update(@event);
                _databaseContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
