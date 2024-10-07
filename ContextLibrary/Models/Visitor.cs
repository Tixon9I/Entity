using System.ComponentModel.DataAnnotations;

namespace ContextLibrary.Models
{
    public class Visitor
    {
        [Key]
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string TypeFile { get; set; }
        public string IdentNumDoc { get; set; }
    }
}
