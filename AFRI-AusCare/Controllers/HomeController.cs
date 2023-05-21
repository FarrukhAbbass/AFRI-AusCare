using AFRI_AusCare.DataModels;
using AFRI_AusCare.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;

namespace AFRI_AusCare.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DatabaseContext _databaseContext;

        public HomeController(ILogger<HomeController> logger, DatabaseContext databaseContext)
        {
            _logger = logger;
            _databaseContext = databaseContext;
        }

        public IActionResult Index()
        {
            var adminSetting = _databaseContext.AdminSettings.FirstOrDefault();
            return View(adminSetting);
        }

        public IActionResult Events()
        {
            var events = _databaseContext.Events.Where(x => !x.IsDeleted).ToList();
            return View(events);
        }

        public IActionResult Gallery()
        {
            var gallery = _databaseContext.Albums.Include(x => x.Galleries).Where(x => !x.IsDeleted && x.Galleries.Count > 0).ToList();
            return View(gallery);
        }

        public IActionResult Team()
        {
            var teams = _databaseContext.Teams.Where(x => !x.IsDeleted).ToList();
            return View(teams);
        }

        public IActionResult Board()
        {
            var boardMembers = _databaseContext.BoardMembers.Where(x => !x.IsDeleted).ToList();
            return View(boardMembers);
        }

        public IActionResult Partner()
        {
            var keyPartners = _databaseContext.KeyPartners.Where(x => !x.IsDeleted).ToList();
            return View(keyPartners);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Media()
        {
            var medias = _databaseContext.Medias.Where(x => !x.IsDeleted).ToList();
            return View(medias);
        }

        public IActionResult NDIS()
        {
            var medias = _databaseContext.Services.Where(x => !x.IsDeleted).ToList();
            return View(medias);
        }

        [HttpPost]
        public ActionResult Contact(ContactFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Send email
                var message = new MailMessage();
                message.To.Add(new MailAddress(model.Email));
                message.From = new MailAddress("umair.hassan.013@gmail.com");
                message.Subject = model.Subject;
                message.Body = model.Message;

                using (var smtp = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("umair.hassan.013@gmail.com", "Momi523@@"),
                    EnableSsl = true,
                })
                {
                    smtp.Send(message);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error sending email: " + ex.Message);
                return View(model);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}