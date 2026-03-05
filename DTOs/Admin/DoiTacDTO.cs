using backend.Attribute;
using System.ComponentModel.DataAnnotations;

namespace backend.DTOs.Admin
{
    public class DoiTacDTO
    {
        
        public string? name { get; set; }
        [VNPhone]
        public string? phone { get; set; }
       
        public string? email { get; set; }
        
        public string? address { get; set; }

    }
}
