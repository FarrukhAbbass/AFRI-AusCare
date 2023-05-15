using AutoMapper;
using AFRI_AusCare.DataModels;
using AFRI_AusCare.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AFRI_AusCare.Controllers
{
    public class TeamController : Controller
    {
        private DatabaseContext _dbContext;
        private IMapper _mapper;
        private IWebHostEnvironment _webHostEnvironment;

        public TeamController(DatabaseContext dbContext, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public ActionResult Index()
        {
            if (HttpContext.Session.Get("UserId") != null)
            {
                var galleries = _dbContext.Teams.Where(x => !x.IsDeleted).ToList();
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
        public async Task<ActionResult> CreateAsync(TeamModel teamModel)
        {
            if (ModelState.IsValid)
            {
                if (teamModel.ImageFile != null && teamModel.ImageFile.Length > 0)
                {
                    var fileName = Path.GetFileName(teamModel.ImageFile.FileName);
                    string[] fileDetails = fileName.Split(".");
                    fileName = Guid.NewGuid().ToString() + "." + fileDetails[1];
                    var path = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);
                    teamModel.ImageUrl = "/images/" + fileName;
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await teamModel.ImageFile.CopyToAsync(stream);
                    }
                }
                teamModel.CreatedDate = DateTime.Now;
                teamModel.ModifiedDate = DateTime.Now;
                teamModel.IsDeleted = false;
                var team = _mapper.Map<Team>(teamModel);
                _dbContext.Teams.Add(team);
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(teamModel);
        }

        public ActionResult Edit(int id)
        {
            if (HttpContext.Session.Get("UserId") != null)
            {
                var gallery = _dbContext.Teams.SingleOrDefault(g => g.Id == id);
                var TeamModel = _mapper.Map<TeamModel>(gallery);
                return View(TeamModel);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync(TeamModel teamModel)
        {
            if (ModelState.IsValid)
            {
                if (teamModel.ImageFile != null && teamModel.ImageFile.Length > 0)
                {
                    var fileName = Path.GetFileName(teamModel.ImageFile.FileName);
                    string[] fileDetails = fileName.Split(".");
                    fileName = Guid.NewGuid().ToString() + "." + fileDetails[1];
                    var path = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);
                    teamModel.ImageUrl = "/images/" + fileName;
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await teamModel.ImageFile.CopyToAsync(stream);
                    }
                }
                teamModel.ModifiedDate = DateTime.Now;
                var gallery = _mapper.Map<Team>(teamModel);
                _dbContext.Entry(gallery).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(teamModel);
        }

        public ActionResult Delete(int id)
        {
            var team = _dbContext.Teams.SingleOrDefault(g => g.Id == id);
            if (team != null)
            {
                team.ModifiedDate = DateTime.Now;
                team.IsDeleted = true;
                _dbContext.Update(team);
                _dbContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
