using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookListMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookListMVC.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public Book Book { get; set; }

        public BooksController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            Book = new Book();
            if(id == null)
            {
                //create
                return View(Book);
            }
            else
            {
                //update
                Book = _db.Books.FirstOrDefault(b => b.Id == id);
                if(Book == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(Book);
                }
            }
            return View();
        }


        [HttpPost]
        //ValidateAntiForgeryToken to use the inbuilt security to prevent some attacks.
        [ValidateAntiForgeryToken]
        public IActionResult Upsert()
        {
            if (ModelState.IsValid)
            {
                if(Book.Id == 0)
                {
                    //create
                    _db.Books.Add(Book);
                }
                else
                {
                    //update
                    _db.Books.Update(Book);
                }
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(Book);
        }

        #region API Calls
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var hej = Json(new { data = await _db.Books.ToListAsync() });
            return Json(new { data = await _db.Books.ToListAsync() });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var bookFromId = await _db.Books.FirstOrDefaultAsync(b => b.Id == id);
            if (bookFromId == null)
            {
                return Json(new { success = false, message = "Error while deleting." });
            }
            else
            {
                _db.Books.Remove(bookFromId);
                await _db.SaveChangesAsync();
                return Json(new { success = true, message = "Delete successful" });
            }
        }
        #endregion
    }
}