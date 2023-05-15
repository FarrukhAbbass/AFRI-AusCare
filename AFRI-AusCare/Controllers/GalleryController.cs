using AutoMapper;
using AFRI_AusCare.DataModels;
using AFRI_AusCare.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AFRI_AusCare.Controllers
{
    public class GalleryController : Controller
    {
        private DatabaseContext _dbContext;
        private IMapper _mapper;
        private IWebHostEnvironment _webHostEnvironment;

        public GalleryController(DatabaseContext dbContext, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public ActionResult Index()
        {
            if (HttpContext.Session.Get("UserId") != null)
            {
                var galleries = _dbContext.Galleries.Where(x => !x.IsDeleted).ToList();
                return View(galleries);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult Create()
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
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync(GalleryModel galleryModel)
        {
            if (ModelState.IsValid)
            {
                if (galleryModel.ImageFile != null && galleryModel.ImageFile.Length > 0)
                {
                    var fileName = Path.GetFileName(galleryModel.ImageFile.FileName);
                    string[] fileDetails = fileName.Split(".");
                    fileName = Guid.NewGuid().ToString() + "." + fileDetails[1];
                    var path = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);
                    galleryModel.ImageUrl = "/images/" + fileName;
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await galleryModel.ImageFile.CopyToAsync(stream);
                    }
                }
                galleryModel.CreatedDate = DateTime.Now;
                galleryModel.ModifiedDate = DateTime.Now;
                galleryModel.IsDeleted = false;
                var gallery = _mapper.Map<Gallery>(galleryModel);
                _dbContext.Galleries.Add(gallery);
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(galleryModel);
        }

        public ActionResult Edit(int id)
        {
            if (HttpContext.Session.Get("UserId") != null)
            {
                var gallery = _dbContext.Galleries.SingleOrDefault(g => g.Id == id);
                var galleryModel = _mapper.Map<GalleryModel>(gallery);
                return View(galleryModel);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync(GalleryModel galleryModel)
        {
            if (ModelState.IsValid)
            {
                if (galleryModel.ImageFile != null && galleryModel.ImageFile.Length > 0)
                {
                    var fileName = Path.GetFileName(galleryModel.ImageFile.FileName);
                    string[] fileDetails = fileName.Split(".");
                    fileName = Guid.NewGuid().ToString() + "." + fileDetails[1];
                    var path = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);
                    galleryModel.ImageUrl = "/images/" + fileName;
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await galleryModel.ImageFile.CopyToAsync(stream);
                    }
                }
                galleryModel.ModifiedDate = DateTime.Now;
                var gallery = _mapper.Map<Gallery>(galleryModel);
                _dbContext.Entry(gallery).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(galleryModel);
        }

        public ActionResult Delete(int id)
        {
            var gallery = _dbContext.Galleries.SingleOrDefault(g => g.Id == id);
            if (gallery != null)
            {
                gallery.ModifiedDate = DateTime.Now;
                gallery.IsDeleted = true;
                _dbContext.Update(gallery);
                _dbContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
