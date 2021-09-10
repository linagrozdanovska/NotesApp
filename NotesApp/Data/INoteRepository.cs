using NotesApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotesApp.Data
{
    public interface INoteRepository 
    {
        List<Note> GetAll(string userId);
        Note Get(int? id, string userId);
        void Insert(Note note);
        void Update(Note note);
        void Delete(Note note);
    }
}
