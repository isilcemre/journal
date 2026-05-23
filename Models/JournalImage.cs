using System.ComponentModel.DataAnnotations.Schema;

namespace deneme2._0.Models
{
    public class JournalImage
    {
        public int Id { get; set; }
        public int JournalEntryId { get; set; }
        public byte[] ImageData { get; set; } = new byte[0]; // BLOB
        public string ImageContentType { get; set; } = string.Empty; // image/jpeg, image/png
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property
        [ForeignKey("JournalEntryId")]
        public virtual JournalEntry? JournalEntry { get; set; }
    }
}