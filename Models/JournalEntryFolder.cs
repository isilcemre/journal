namespace deneme2._0.Models
{
    public class JournalEntryFolder
    {
        public int Id { get; set; }
        public int JournalEntryId { get; set; }
        public int FolderId { get; set; }

        // Navigation properties
        public virtual JournalEntry? JournalEntry { get; set; }
        public virtual Folder? Folder { get; set; }
    }
}