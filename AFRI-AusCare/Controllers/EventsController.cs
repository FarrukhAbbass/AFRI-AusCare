using AFRI_AusCare.DataModels;
using AFRI_AusCare.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AFRI_AusCare.Controllers
{
    public class EventsController : Controller
    {
        private DatabaseContext _databaseContext;
        private IMapper _mapper;
        private IWebHostEnvironment _webHostEnvironment;

        public EventsController(DatabaseContext dbContext, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _mapper = mapper;
            _databaseContext = dbContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.Get("UserId") != null)
            {
                var eventsList = _databaseContext.Events.Where(x => !x.IsDeleted).ToList();
                return View(eventsList);
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
        public async Task<IActionResult> CreateAsync(EventModel eventModel)
        {
            if (eventModel.ImageFile != null && eventModel.ImageFile.Length > 0)
            {
                var fileName = Path.GetFileName(eventModel.ImageFile.FileName);
                string[] fileDetails = fileName.Split(".");
                fileName = Guid.NewGuid().ToString() + "." + fileDetails[1];
                var path = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);
                eventModel.ImageUrl = "/images/" + fileName;
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await eventModel.ImageFile.CopyToAsync(stream);
                }
            }

            eventModel.ModifiedDate = DateTime.Now;
            eventModel.IsDeleted = false;
            var events = _mapper.Map<Event>(eventModel);
            _databaseContext.Add(events);
            _databaseContext.SaveChanges();
            return Redirect("Index");
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync(EventModel eventModel)
        {
            if (eventModel.ImageFile != null && eventModel.ImageFile.Length > 0)
            {
                var fileName = Path.GetFileName(eventModel.ImageFile.FileName);
                string[] fileDetails = fileName.Split(".");
                fileName = Guid.NewGuid().ToString() + "." + fileDetails[1];
                var path = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);
                eventModel.ImageUrl = "/images/" + fileName;
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await eventModel.ImageFile.CopyToAsync(stream);
                }
            }
            eventModel.ModifiedDate = DateTime.Now;


            var events = _mapper.Map<Event>(eventModel);
            _databaseContext.Entry(events).State = EntityState.Modified;
            _databaseContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.Get("UserId") != null)
            {
                var events = _databaseContext.Events.SingleOrDefault(x => x.Id == id);
                var eventModel = _mapper.Map<EventModel>(events);
                return View(eventModel);
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
