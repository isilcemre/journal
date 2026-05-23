namespace deneme2._0.Models
{
    public class Folder
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Emoji { get; set; } = "📁";
        public bool IsDefault { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public User? User { get; set; }
        public virtual ICollection<JournalEntry> Entries { get; set; }
            = new List<JournalEntry>();
        public virtual ICollection<JournalEntryFolder> JournalEntryFolders { get; set; }
            = new List<JournalEntryFolder>();
    }
}