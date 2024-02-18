using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VolunteersClub.Models
{
    public class PhoneNumber
    {
        [Key]
        public string PhoneMun {  get; set; }
        [ForeignKey("VolunteerID")]
        public int VolunteerID { get; set; }
        
    }
}
