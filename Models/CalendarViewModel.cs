namespace deneme2._0.Models
{
    public class CalendarViewModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public Dictionary<DateTime, List<JournalEntry>> EntriesByDate { get; set; } = new();

        // Ayın adı (Ocak, Şubat...)
        public string MonthName => new DateTime(Year, Month, 1).ToString("MMMM");

        // Ayın ilk günü (Pazartesi, Salı...)
        public DayOfWeek FirstDayOfMonth => new DateTime(Year, Month, 1).DayOfWeek;

        // Ayın kaç gün olduğu
        public int DaysInMonth => DateTime.DaysInMonth(Year, Month);
    }
}