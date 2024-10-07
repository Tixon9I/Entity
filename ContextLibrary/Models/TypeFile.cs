using System.ComponentModel.DataAnnotations;

namespace ContextLibrary.Models
{
    public class TypeFile
    {
        [Key]
        public string Name { get; set; }

        public ICollection<Visitor> Visitors { get; set; }
    }
}
