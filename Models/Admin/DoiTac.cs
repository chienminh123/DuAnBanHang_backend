using backend.Attribute;
using System.ComponentModel.DataAnnotations;

namespace backend.Models.Admin
{
    public class DoiTac
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [VNPhone]
        public string Phone { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Address { get; set; }
        

    }
}
