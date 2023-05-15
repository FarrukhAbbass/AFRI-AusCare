using AutoMapper;
using AFRI_AusCare.DataModels;
using AFRI_AusCare.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AFRI_AusCare.Controllers
{
    public class BoardMemberController : Controller
    {
        private DatabaseContext _dbContext;
        private IMapper _mapper;
        private IWebHostEnvironment _webHostEnvironment;

        public BoardMemberController(DatabaseContext dbContext, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public ActionResult Index()
        {
            if (HttpContext.Session.Get("UserId") != null)
            {
                var galleries = _dbContext.BoardMembers.Where(x => !x.IsDeleted).ToList();
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
        public async Task<ActionResult> CreateAsync(BoardMemberModel boardMemberModel)
        {
            if (ModelState.IsValid)
            {
                if (boardMemberModel.ImageFile != null && boardMemberModel.ImageFile.Length > 0)
                {
                    var fileName = Path.GetFileName(boardMemberModel.ImageFile.FileName);
                    string[] fileDetails = fileName.Split(".");
                    fileName = Guid.NewGuid().ToString() + "." + fileDetails[1];
                    var path = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);
                    boardMemberModel.ImageUrl = "/images/" + fileName;
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await boardMemberModel.ImageFile.CopyToAsync(stream);
                    }
                }
                boardMemberModel.CreatedDate = DateTime.Now;
                boardMemberModel.ModifiedDate = DateTime.Now;
                boardMemberModel.IsDeleted = false;
                var boardMember = _mapper.Map<BoardMember>(boardMemberModel);
                _dbContext.BoardMembers.Add(boardMember);
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(boardMemberModel);
        }

        public ActionResult Edit(int id)
        {
            if (HttpContext.Session.Get("UserId") != null)
            {
                var boardMember = _dbContext.BoardMembers.SingleOrDefault(g => g.Id == id);
                var boardMemberModel = _mapper.Map<BoardMemberModel>(boardMember);
                return View(boardMemberModel);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync(BoardMemberModel boardMemberModel)
        {
            if (ModelState.IsValid)
            {
                if (boardMemberModel.ImageFile != null && boardMemberModel.ImageFile.Length > 0)
                {
                    var fileName = Path.GetFileName(boardMemberModel.ImageFile.FileName);
                    string[] fileDetails = fileName.Split(".");
                    fileName = Guid.NewGuid().ToString() + "." + fileDetails[1];
                    var path = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);
                    boardMemberModel.ImageUrl = "/images/" + fileName;
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await boardMemberModel.ImageFile.CopyToAsync(stream);
                    }
                }
                boardMemberModel.ModifiedDate = DateTime.Now;
                var boardMember = _mapper.Map<BoardMember>(boardMemberModel);
                _dbContext.Entry(boardMember).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(boardMemberModel);
        }

        public ActionResult Delete(int id)
        {
            var keyPartner = _dbContext.BoardMembers.SingleOrDefault(g => g.Id == id);
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
