using NotesApp.Models;

namespace NotesApp.Repositories
{
    /// <summary>
    /// Abstraction for Note persistence operations.
    /// </summary>
    public interface INoteRepository
    {
        // PUBLIC_INTERFACE
        /// <summary>
        /// Returns all notes.
        /// </summary>
        /// <returns>IEnumerable of Note.</returns>
        IEnumerable<Note> GetAll();

        // PUBLIC_INTERFACE
        /// <summary>
        /// Get a note by its Id.
        /// </summary>
        /// <param name="id">Note Id</param>
        /// <returns>Note or null when not found</returns>
        Note? GetById(Guid id);

        // PUBLIC_INTERFACE
        /// <summary>
        /// Create a new note.
        /// </summary>
        /// <param name="note">Note to create</param>
        void Create(Note note);

        // PUBLIC_INTERFACE
        /// <summary>
        /// Update an existing note.
        /// </summary>
        /// <param name="note">Note with updated values</param>
        /// <returns>True if updated; false if not found</returns>
        bool Update(Note note);

        // PUBLIC_INTERFACE
        /// <summary>
        /// Delete a note by Id.
        /// </summary>
        /// <param name="id">Note Id</param>
        /// <returns>True if deleted; false if not found</returns>
        bool Delete(Guid id);
    }
}
