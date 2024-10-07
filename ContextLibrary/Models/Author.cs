using System.ComponentModel.DataAnnotations;

namespace ContextLibrary.Models
{
    public class Author
    {
        [Key]
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Pseudonym { get; set; }
        public DateTime Birthdate { get; set; }
        public ICollection<AuthorBook> AuthorBooks { get; set; }
    }
}
