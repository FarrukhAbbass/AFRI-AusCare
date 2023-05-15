using AutoMapper;
using AFRI_AusCare.DataModels;
using AFRI_AusCare.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AFRI_AusCare.Controllers
{
    public class KeyPartnerController : Controller
    {
        private DatabaseContext _dbContext;
        private IMapper _mapper;
        private IWebHostEnvironment _webHostEnvironment;

        public KeyPartnerController(DatabaseContext dbContext, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public ActionResult Index()
        {
            if (HttpContext.Session.Get("UserId") != null)
            {
                var galleries = _dbContext.KeyPartners.Where(x => !x.IsDeleted).ToList();
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
        public async Task<ActionResult> CreateAsync(KeyPartnerModel keyPartnerModel)
        {
            if (ModelState.IsValid)
            {
                if (keyPartnerModel.ImageFile != null && keyPartnerModel.ImageFile.Length > 0)
                {
                    var fileName = Path.GetFileName(keyPartnerModel.ImageFile.FileName);
                    string[] fileDetails = fileName.Split(".");
                    fileName = Guid.NewGuid().ToString() + "." + fileDetails[1];
                    var path = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);
                    keyPartnerModel.ImageUrl = "/images/" + fileName;
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await keyPartnerModel.ImageFile.CopyToAsync(stream);
                    }
                }
                keyPartnerModel.CreatedDate = DateTime.Now;
                keyPartnerModel.ModifiedDate = DateTime.Now;
                keyPartnerModel.IsDeleted = false;
                var keyPartner = _mapper.Map<KeyPartner>(keyPartnerModel);
                _dbContext.KeyPartners.Add(keyPartner);
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(keyPartnerModel);
        }

        public ActionResult Edit(int id)
        {
            if (HttpContext.Session.Get("UserId") != null)
            {
                var keyPartner = _dbContext.KeyPartners.SingleOrDefault(g => g.Id == id);
                var keyPartnerModel = _mapper.Map<KeyPartnerModel>(keyPartner);
                return View(keyPartnerModel);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync(KeyPartnerModel keyPartnerModel)
        {
            if (ModelState.IsValid)
            {
                if (keyPartnerModel.ImageFile != null && keyPartnerModel.ImageFile.Length > 0)
                {
                    var fileName = Path.GetFileName(keyPartnerModel.ImageFile.FileName);
                    string[] fileDetails = fileName.Split(".");
                    fileName = Guid.NewGuid().ToString() + "." + fileDetails[1];
                    var path = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);
                    keyPartnerModel.ImageUrl = "/images/" + fileName;
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await keyPartnerModel.ImageFile.CopyToAsync(stream);
                    }
                }
                keyPartnerModel.ModifiedDate = DateTime.Now;
                var gallery = _mapper.Map<KeyPartner>(keyPartnerModel);
                _dbContext.Entry(gallery).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(keyPartnerModel);
        }

        public ActionResult Delete(int id)
        {
            var keyPartner = _dbContext.KeyPartners.SingleOrDefault(g => g.Id == id);
            if (keyPartner != null)
            {
                keyPartner.ModifiedDate = DateTime.Now;
                keyPartner.IsDeleted = true;
                _dbContext.Update(keyPartner);
                _dbContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
