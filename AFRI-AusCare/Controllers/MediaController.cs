using AFRI_AusCare.DataModels;
using AFRI_AusCare.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AFRI_AusCare.Controllers
{
    public class MediaController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MediaController(DatabaseContext dbContext, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _mapper = mapper;
            _context = dbContext;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Media
        public async Task<IActionResult> Index()
        {
            return _context.Medias != null ?
                        View(await _context.Medias.Where(x => !x.IsDeleted).ToListAsync()) :
                        Problem("Entity set 'DatabaseContext.Medias'  is null.");
        }

        // GET: Media/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Media/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MediaModel media)
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
                var mediaEntity = _mapper.Map<Media>(media);
                _context.Add(mediaEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(media);
        }

        // GET: Media/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Medias == null)
            {
                return NotFound();
            }

            var media = await _context.Medias.FindAsync(id);
            if (media == null)
            {
                return NotFound();
            }

            var mediaModel = _mapper.Map<MediaModel>(media);
            return View(mediaModel);
        }

        // POST: Media/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MediaModel media)
        {
            if (ModelState.IsValid)
            {
                var mediaEntity = _context.Medias.SingleOrDefault(x => x.Id == media.Id);
                if (mediaEntity != null)
                {
                    var currentCoverImage = mediaEntity.ImageUrl;
                    if (media.ImageFile != null && media.ImageFile.Length > 0)
                    {
                        var fileName = Path.GetFileName(media.ImageFile.FileName);
                        string[] fileDetails = fileName.Split(".");
                        fileName = Guid.NewGuid().ToString() + "." + fileDetails[1];
                        var path = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);
                        mediaEntity.ImageUrl = "/images/" + fileName;
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await media.ImageFile.CopyToAsync(stream);
                        }
                    }

                    mediaEntity.Description = media.Description;
                    mediaEntity.Title = media.Title;
                    mediaEntity.MediaURL = media.MediaURL;
                    mediaEntity.CreatedDate = DateTime.Now;
                    mediaEntity.ModifiedDate = DateTime.Now;
                    mediaEntity.IsDeleted = false;
                    _context.Update(mediaEntity);
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
            if (_context.Medias == null)
            {
                return Problem("Entity set 'DatabaseContext.Medias'  is null.");
            }

            var media = _context.Medias.SingleOrDefault(x => x.Id == id);

            if (media != null)
            {
                _context.Medias.Remove(media);
                await _context.SaveChangesAsync();
                if (media.ImageUrl != null)
                {
                    var deleteImagePath = $"{_webHostEnvironment.WebRootPath}//{media.ImageUrl}";
                    DeleteImage(deleteImagePath);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool MediaExists(int id)
        {
            return (_context.Medias?.Any(e => e.Id == id)).GetValueOrDefault();
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
