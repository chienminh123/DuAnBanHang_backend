using System.ComponentModel.DataAnnotations;

namespace backend.DTOs.Admin
{
    public class ShopInfoDTO
    {

        public string name { get; set; }

        public string phone { get; set; }
 
        public string address { get; set; }
        
        public string city { get; set; }
    }
}
