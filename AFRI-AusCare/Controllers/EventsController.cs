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
        public async Task<IActionResult> CreateAsync(EventModel eventModel, List<IFormFile> images)
        {
            eventModel.ModifiedDate = DateTime.Now;
            eventModel.IsDeleted = false;
            var events = _mapper.Map<Event>(eventModel);

            Album album = new()
            {
                Title = eventModel.Title,
                CreatedDate = eventModel.CreatedDate,
                ModifiedDate = eventModel.ModifiedDate,
                IsAlbum = true,
                IsDeleted = false
            };

            _databaseContext.Add(album);
            _databaseContext.SaveChanges();
            events.AlbumId = album.Id;

            if (eventModel.ImageFile != null && eventModel.ImageFile.Length > 0)
            {
                var fileName = Path.GetFileName(eventModel.ImageFile.FileName);
                string[] fileDetails = fileName.Split(".");
                fileName = Guid.NewGuid().ToString() + "." + fileDetails[1];
                var path = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);
                events.ImageUrl = "/images/" + fileName;
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await eventModel.ImageFile.CopyToAsync(stream);
                }
            }

            _databaseContext.Add(events);
            _databaseContext.SaveChanges();

            foreach (var item in images)
            {
                Gallery galleryItem = new Gallery()
                {
                    AlbumId = album.Id,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    IsDeleted = false
                };

                var fileName = Path.GetFileName(item.FileName);
                string[] fileDetails = fileName.Split(".");
                fileName = Guid.NewGuid().ToString() + "." + fileDetails[1];
                var path = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);
                galleryItem.ImageUrl = "/images/" + fileName;
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await item.CopyToAsync(stream);
                }

                _databaseContext.Galleries.Add(galleryItem);
                await _databaseContext.SaveChangesAsync();
            }

            return Redirect("Index");
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync(EventModel eventModel, List<IFormFile> images)
        {
            if (eventModel != null)
            {
                Event? eventEntity = await _databaseContext.Events.AsNoTracking()
                    .Include(x => x.Album).ThenInclude(x => x.Galleries)
                    .FirstOrDefaultAsync(a => a.Id == eventModel.Id);

                if (eventEntity == null)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        string currentCoverImage = eventEntity.ImageUrl ?? "";
                        if (eventModel.ImageFile != null && eventModel.ImageFile.Length > 0)
                        {
                            var fileName = Path.GetFileName(eventModel.ImageFile.FileName);
                            string[] fileDetails = fileName.Split(".");
                            fileName = Guid.NewGuid().ToString() + "." + fileDetails[1];
                            var path = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);
                            eventEntity.ImageUrl = "/images/" + fileName;
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await eventModel.ImageFile.CopyToAsync(stream);
                            }
                        }
                        eventEntity.Description = eventModel.Description;
                        eventModel.Title = eventModel.Title;
                        eventModel.CreatedDate = eventModel.CreatedDate;
                        eventModel.Author = eventModel.Author;
                        eventEntity.ModifiedDate = DateTime.Now;
                        _databaseContext.Update(eventEntity);
                        await _databaseContext.SaveChangesAsync();

                        var deleteImagePath = $"{_webHostEnvironment.WebRootPath}//{currentCoverImage}";
                        DeleteImage(deleteImagePath);

                        foreach (var item in images)
                        {
                            Gallery galleryItem = new Gallery()
                            {
                                AlbumId = eventEntity.AlbumId,
                                CreatedDate = DateTime.Now,
                                ModifiedDate = DateTime.Now,
                                IsDeleted = false,
                            };

                            var fileName = Path.GetFileName(item.FileName);
                            string[] fileDetails = fileName.Split(".");
                            fileName = Guid.NewGuid().ToString() + "." + fileDetails[1];
                            var path = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);
                            galleryItem.ImageUrl = "/images/" + fileName;
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await item.CopyToAsync(stream);
                            }

                            _databaseContext.Galleries.Add(galleryItem);
                            await _databaseContext.SaveChangesAsync();

                        }
                    }
                    catch (DbUpdateConcurrencyException)
                    {

                    }
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    eventModel.Album = _mapper.Map<AlbumModel>(eventEntity.Album);
                    return View(eventModel);
                }

            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.Get("UserId") != null)
            {
                var events = _databaseContext.Events.Include(x => x.Album).ThenInclude(x => x.Galleries).SingleOrDefault(x => x.Id == id);
                var eventModel = _mapper.Map<EventModel>(events);
                return View(eventModel);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            Event? eventModel = await _databaseContext.Events.Include(x => x.Album).ThenInclude(x => x.Galleries)
               .FirstOrDefaultAsync(a => a.Id == id);

            if (eventModel == null)
            {
                return NotFound();
            }
            else
            {
                if (eventModel.Album.Galleries != null)
                {
                    foreach (var item in eventModel.Album.Galleries)
                    {
                        if (item.ImageUrl != null)
                        {
                            var path = $"{_webHostEnvironment.WebRootPath}//{item.ImageUrl}";
                            if (System.IO.File.Exists(path))
                            {
                                DeleteImage(path);
                                _databaseContext.Galleries.Remove(item);
                            }
                        }
                    }
                    _databaseContext.Albums.Remove(eventModel.Album);

                    if (eventModel.ImageUrl != null)
                    {
                        var path = $"{_webHostEnvironment.WebRootPath}//{eventModel.ImageUrl}";
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                    }

                    _databaseContext.Events.Remove(eventModel);
                    _databaseContext.SaveChanges();
                }

                return RedirectToAction(nameof(Index));
            }
        }

        public void DeleteImage(string path)
        {
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }
    }
}
