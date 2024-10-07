using System.ComponentModel.DataAnnotations;

namespace ContextLibrary.Models
{
    public class AuthorBook
    {
        [Key]
        public int AuthorId { get; set; }
        public Author Author { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; }
    }


}
