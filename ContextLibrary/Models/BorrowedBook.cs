namespace ContextLibrary.Models
{
    public class BorrowedBook
    {
        public int Id { get; set; }
        public int VisitorId { get; set; }
        public Visitor Visitor { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
        public DateTime BorrowedDate { get; set; } = DateTime.Now;
        public DateTime DueDate { get; set; } = DateTime.Now.AddDays(30);
        public DateTime? ReturnDate { get; set; }
    }
}
