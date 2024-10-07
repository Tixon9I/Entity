using System.ComponentModel.DataAnnotations;

namespace ContextLibrary.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        public string? Title { get; set; }
        public int AuthorId { get; set; }
        public string? PublisherCode { get; set; }
        public string? TypePublisherCode { get; set; }
        public int Year { get; set; }
        public string? PublisherContry { get; set; }
        [Required]
        public string? PublisherCity { get; set; }

        public virtual Author? Author { get; set; }
        public ICollection<AuthorBook>? AuthorBooks { get; set; }
    }
}
