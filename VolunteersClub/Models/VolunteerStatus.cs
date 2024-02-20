using System.ComponentModel.DataAnnotations;

namespace VolunteersClub.Models
{
    public class VolunteerStatus
    {
        [Key]
        public int StatusID {  get; set; }
        public string Status { get; set;  }
    }
}
