using System.Collections.Concurrent;
using NotesApp.Models;

namespace NotesApp.Repositories
{
    /// <summary>
    /// In-memory repository for Notes. Intended for development/testing.
    /// Thread-safe operations using ConcurrentDictionary.
    /// </summary>
    public class InMemoryNoteRepository : INoteRepository
    {
        private readonly ConcurrentDictionary<Guid, Note> _store = new();

        public IEnumerable<Note> GetAll()
        {
            return _store.Values.OrderByDescending(n => n.UpdatedAt);
        }

        public Note? GetById(Guid id)
        {
            _store.TryGetValue(id, out var note);
            return note;
        }

        public void Create(Note note)
        {
            _store[note.Id] = note;
        }

        public bool Update(Note note)
        {
            if (!_store.ContainsKey(note.Id)) return false;
            _store[note.Id] = note;
            return true;
        }

        public bool Delete(Guid id)
        {
            return _store.TryRemove(id, out _);
        }

        /// <summary>
        /// Seed example notes for development convenience.
        /// </summary>
        public void SeedIfEmpty()
        {
            if (_store.IsEmpty)
            {
                var now = DateTime.UtcNow;
                var note1 = new Note
                {
                    Id = Guid.NewGuid(),
                    Title = "Welcome to Notes",
                    Content = "This is your first note. Feel free to edit or delete it.",
                    CreatedAt = now.AddMinutes(-30),
                    UpdatedAt = now.AddMinutes(-30)
                };
                var note2 = new Note
                {
                    Id = Guid.NewGuid(),
                    Title = "Things to do",
                    Content = "- Add a new note\n- Update this list\n- Explore the API at /docs",
                    CreatedAt = now.AddMinutes(-20),
                    UpdatedAt = now.AddMinutes(-10)
                };
                Create(note1);
                Create(note2);
            }
        }
    }
}
