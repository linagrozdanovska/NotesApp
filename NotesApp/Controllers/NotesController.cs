using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NotesApp.Data;
using NotesApp.Models;

namespace NotesApp.Controllers
{
    [Authorize]
    public class NotesController : Controller
    {
        private readonly INoteRepository _noteRepository;
        private readonly UserManager<IdentityUser> _userManager;

        public NotesController(INoteRepository noteRepository, UserManager<IdentityUser> userManager)
        {
            _noteRepository = noteRepository;
            _userManager = userManager;
        }

        // GET: Notes
        public IActionResult Index(string searchString)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var notes = _noteRepository.GetAll(userId).ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
                notes = notes.Where(z => z.Title.ToLowerInvariant().Normalize().Contains(searchString.ToLowerInvariant().Normalize()) || z.Body.ToLowerInvariant().Normalize().Contains(searchString.ToLowerInvariant().Normalize())).ToList();
            }
            return View(notes);
        }

        // GET: Notes/Details/5
        public IActionResult Details(int? id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (id == null)
            {
                return NotFound();
            }

            var note = _noteRepository.Get(id, userId);

            if (note == null)
            {
                return NotFound();
            }

            return View("Details", note);
        }

        // GET: Notes/Create
        public IActionResult Create()
        {
            return View("Create");
        }

        // POST: Notes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,UserId,Title,Body,Date")] Note note)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ModelState.IsValid)
            {
                note.UserId = userId;
                note.Date = DateTime.Now;
                _noteRepository.Insert(note);
                return RedirectToAction(nameof(Index));
            }
            return View("Create", note);
        }

        // GET: Notes/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var note = _noteRepository.Get(id, userId);

            if (note == null)
            {
                return NotFound();
            }
            return View("Edit", note);
        }

        // POST: Notes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,UserId,Title,Body,Date")] Note note)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!id.Equals(note.Id))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    note.UserId = userId;
                    note.Date = DateTime.Now;
                    _noteRepository.Update(note);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NoteExists(note.Id))
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
            return View("Edit", note);
        }

        // GET: Notes/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var note = _noteRepository.Get(id, userId);
            
            if (note == null)
            {
                return NotFound();
            }

            return View("Delete", note);
        }

        // POST: Notes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var note = _noteRepository.Get(id, userId);
            _noteRepository.Delete(note);
            return RedirectToAction(nameof(Index));
        }
        
        [AllowAnonymous]
        public IActionResult Home()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Notes");
            }
            else
            {
                return View("Home");
            }
        }

        private bool NoteExists(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return _noteRepository.GetAll(userId).Any(e => e.Id == id);
        }
    }
}
