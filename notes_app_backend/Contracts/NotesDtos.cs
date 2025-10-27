using System.ComponentModel.DataAnnotations;

namespace NotesApp.Contracts
{
    /// <summary>
    /// Payload for creating a Note.
    /// </summary>
    public class CreateNoteRequest
    {
        [Required]
        [MaxLength(200)]
        public string? Title { get; set; }

        public string? Content { get; set; }
    }

    /// <summary>
    /// Payload for updating a Note.
    /// </summary>
    public class UpdateNoteRequest
    {
        [Required]
        [MaxLength(200)]
        public string? Title { get; set; }

        public string? Content { get; set; }
    }
}
