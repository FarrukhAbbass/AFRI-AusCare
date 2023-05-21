using AFRI_AusCare.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AFRI_AusCare.Controllers
{
    public class AlbumController : Controller
    {
        private DatabaseContext _context;
        private IMapper _mapper;
        private IWebHostEnvironment _webHostEnvironment;

        public AlbumController(DatabaseContext dbContext, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _mapper = mapper;
            _context = dbContext;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: /Album
        public async Task<IActionResult> Index()
        {
            List<Album> albums = await _context.Albums.Include(x => x.Galleries).Where(x => !x.IsDeleted).ToListAsync();
            return View(albums);
        }

        // GET: /Album/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Album? album = await _context.Albums.Include(x => x.Galleries).FirstOrDefaultAsync(a => a.Id == id);

            if (album == null)
            {
                return NotFound();
            }

            return View(album);
        }

        // GET: /Album/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Album/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Album album, List<IFormFile> images)
        {
            if (ModelState.IsValid)
            {
                album.CreatedDate = DateTime.Now;
                album.ModifiedDate = DateTime.Now;
                album.IsDeleted = false;
                album.IsAlbum = true;
                album.AlbumType = AlbumType.Album;
                _context.Add(album);
                await _context.SaveChangesAsync();

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

                    _context.Galleries.Add(galleryItem);
                    await _context.SaveChangesAsync();

                }

                return RedirectToAction(nameof(Index));
            }

            return View(album);
        }

        // GET: /Album/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Album? album = await _context.Albums.Include(x => x.Galleries).FirstOrDefaultAsync(a => a.Id == id && a.AlbumType == AlbumType.Album);

            if (album == null)
            {
                return NotFound();
            }

            return View(album);
        }

        // POST: /Album/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Album model, List<IFormFile> images)
        {
            if (model != null)
            {
                Album? album = await _context.Albums.AsNoTracking()
                    .Include(x => x.Galleries)
                    .FirstOrDefaultAsync(a => a.Id == model.Id && a.AlbumType == AlbumType.Album);

                if (album == null)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        model.IsAlbum = album.IsAlbum;
                        model.CreatedDate = album.CreatedDate;
                        model.IsDeleted = album.IsDeleted;
                        model.AlbumType = album.AlbumType;
                        model.ModifiedDate = DateTime.Now;
                        _context.Update(model);
                        await _context.SaveChangesAsync();

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

                            _context.Galleries.Add(galleryItem);
                            await _context.SaveChangesAsync();

                        }
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!AlbumExists(album.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }

                return View(album);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: /Album/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Album? album = await _context.Albums.Include(x => x.Galleries)
                .FirstOrDefaultAsync(a => a.Id == id && a.AlbumType == AlbumType.Album);

            if (album == null)
            {
                return NotFound();
            }
            else
            {
                _context.Albums.Remove(album);
                if (album.Galleries != null)
                {
                    foreach (var item in album.Galleries)
                    {
                        if (item.ImageUrl != null)
                        {
                            var path = $"{_webHostEnvironment.WebRootPath}//{item.ImageUrl}";
                            if (System.IO.File.Exists(path))
                            {
                                System.IO.File.Delete(path);
                                _context.Galleries.Remove(item);
                            }
                        }
                    }
                    _context.SaveChanges();
                }

                return RedirectToAction(nameof(Index));
            }
        }

        private bool AlbumExists(int id)
        {
            return _context.Albums.Any(a => a.Id == id && a.AlbumType == AlbumType.Album);
        }

    }
}
