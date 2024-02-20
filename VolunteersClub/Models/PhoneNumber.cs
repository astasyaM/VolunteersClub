using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VolunteersClub.Models
{
    public class PhoneNumber
    {
        [Key]
        public string PhoneNum {  get; set; }
        [ForeignKey("VolunteerID")]
        public int VolunteerID { get; set; }
        
    }
}
