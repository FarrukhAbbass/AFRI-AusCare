using AFRI_AusCare.DataModels;
using AFRI_AusCare.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AFRI_AusCare.Controllers
{
    public class ServiceController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ServiceController(DatabaseContext dbContext, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _mapper = mapper;
            _context = dbContext;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Media
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.Get("UserId") != null)
            {
                return _context.Services != null ?
                        View(await _context.Services.Where(x => !x.IsDeleted).ToListAsync()) :
                        Problem("Entity set 'DatabaseContext.Services'  is null.");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        // GET: Media/Create
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

        // POST: Media/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceModel media)
        {
            if (HttpContext.Session.Get("UserId") != null)
            {
                if (ModelState.IsValid)
                {
                    if (media.ImageFile != null && media.ImageFile.Length > 0)
                    {
                        var fileName = Path.GetFileName(media.ImageFile.FileName);
                        string[] fileDetails = fileName.Split(".");
                        fileName = Guid.NewGuid().ToString() + "." + fileDetails[1];
                        var path = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);
                        media.ImageUrl = "/images/" + fileName;
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await media.ImageFile.CopyToAsync(stream);
                        }
                    }

                    media.CreatedDate = DateTime.Now;
                    media.ModifiedDate = DateTime.Now;
                    media.IsDeleted = false;
                    var service = _mapper.Map<Service>(media);
                    _context.Add(service);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                return View(media);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        // GET: Media/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (HttpContext.Session.Get("UserId") != null)
            {
                if (id == null || _context.Services == null)
                {
                    return NotFound();
                }

                var service = await _context.Services.FindAsync(id);
                if (service == null)
                {
                    return NotFound();
                }

                var mediaModel = _mapper.Map<ServiceModel>(service);
                return View(mediaModel);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        // POST: Media/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ServiceModel media)
        {
            if (ModelState.IsValid)
            {
                var service = _context.Services.SingleOrDefault(x => x.Id == media.Id);
                if (service != null)
                {
                    var currentCoverImage = service.ImageUrl;
                    if (media.ImageFile != null && media.ImageFile.Length > 0)
                    {
                        var fileName = Path.GetFileName(media.ImageFile.FileName);
                        string[] fileDetails = fileName.Split(".");
                        fileName = Guid.NewGuid().ToString() + "." + fileDetails[1];
                        var path = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);
                        service.ImageUrl = "/images/" + fileName;
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await media.ImageFile.CopyToAsync(stream);
                        }
                    }

                    service.Description = media.Description;
                    service.Title = media.Title;
                    service.CreatedDate = DateTime.Now;
                    service.ModifiedDate = DateTime.Now;
                    service.IsDeleted = false;
                    _context.Update(service);
                    await _context.SaveChangesAsync();
                    var deleteImagePath = $"{_webHostEnvironment.WebRootPath}//{currentCoverImage}";
                    DeleteImage(deleteImagePath);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return NotFound();
                }
            }
            return View(media);
        }

        // GET: Media/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (_context.Services == null)
            {
                return Problem("Entity set 'DatabaseContext.Services'  is null.");
            }

            var service = _context.Services.SingleOrDefault(x => x.Id == id);

            if (service != null)
            {
                _context.Services.Remove(service);
                await _context.SaveChangesAsync();
                if (service.ImageUrl != null)
                {
                    var deleteImagePath = $"{_webHostEnvironment.WebRootPath}//{service.ImageUrl}";
                    DeleteImage(deleteImagePath);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool MediaExists(int id)
        {
            return (_context.Services?.Any(e => e.Id == id)).GetValueOrDefault();
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
