using Microsoft.EntityFrameworkCore;
using NotesApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NotesApp.Data
{
    public class NoteRepository : INoteRepository
    {
        private readonly ApplicationDbContext context;
        private DbSet<Note> notes;

        public NoteRepository(ApplicationDbContext context)
        {
            this.context = context;
            notes = context.Set<Note>();
        }

        public List<Note> GetAll(string userId)
        {
            return notes
                .Where(z => z.UserId.Equals(userId))
                .ToList();
        }

        public Note Get(int? id, string userId)
        {
            return notes
                .Where(z => z.UserId.Equals(userId))
                .SingleOrDefault(z => z.Id.Equals(id));
        }

        public void Insert(Note note)
        {
            if (note == null)
            {
                throw new ArgumentNullException("note");
            }
            notes.Add(note);
            context.SaveChanges();
        }

        public void Update(Note note)
        {
            if (note == null)
            {
                throw new ArgumentNullException("note");
            }
            notes.Update(note);
            context.SaveChanges();
        }

        public void Delete(Note note)
        {
            if (note == null)
            {
                throw new ArgumentNullException("note");
            }
            notes.Remove(note);
            context.SaveChanges();
        }
    }
}
