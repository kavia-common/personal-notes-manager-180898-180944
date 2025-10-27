using System.ComponentModel.DataAnnotations;

namespace NotesApp.Models
{
    /// <summary>
    /// Represents a Note entity stored by the application.
    /// </summary>
    public class Note
    {
        /// <summary>
        /// Unique identifier of the note.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Title of the note.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public required string Title { get; set; }

        /// <summary>
        /// Content of the note (optional).
        /// </summary>
        public string? Content { get; set; }

        /// <summary>
        /// Timestamp when the note was created (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Timestamp when the note was last updated (UTC).
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
