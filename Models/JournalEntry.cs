namespace deneme2._0.Models
{
    public class JournalEntry
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        public User? User { get; set; }
        
        // ÇOKLU RESİM İÇİN 
        public virtual ICollection<JournalImage> Images { get; set; } = new List<JournalImage>();

        // Klasör ilişkisi
        public virtual ICollection<JournalEntryFolder> JournalEntryFolders { get; set; }
            = new List<JournalEntryFolder>();
    }
}