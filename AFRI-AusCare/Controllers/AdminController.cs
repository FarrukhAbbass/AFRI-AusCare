using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AFRI_AusCare.Models;

namespace AFRI_AusCare.Controllers
{
    public class AdminController : Controller
    {
        private readonly DatabaseContext _context;

        public AdminController(DatabaseContext context)
        {
            _context = context;
        }


        // GET: Admin/Edit/5
        public IActionResult Credentials()
        {
            var adminSetting = _context.AdminSettings.SingleOrDefault(x => x.Id == 1);
            return View(adminSetting);
        }

        public IActionResult BankAccount()
        {
            var adminSetting = _context.AdminSettings.SingleOrDefault(x => x.Id == 1);
            return View(adminSetting);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("Id,AccountBSB,AccountNumber,BankName,BankABN,UserEmail,Password")] AdminSetting adminSetting)
        {
            if (id != adminSetting.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(adminSetting);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    
                }
                return RedirectToAction(nameof(Index));
            }
            return View(adminSetting);
        }
    }
}
