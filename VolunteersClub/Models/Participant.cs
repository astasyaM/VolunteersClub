using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VolunteersClub.Models
{
    public class Participant
    {
        [Key]
        public int RecordID { get; set; }
        [ForeignKey("EventID")]
        public int EventID { get; set; }
        [ForeignKey("VolunteerID")]
        public int VolunteerID { get; set; }
        [ForeignKey("ResponsibilityID")]
        public int ResponsibilityID { get; set; }
        public bool ConfirmedVolunteer { get; set; }
        public bool ConfirmedLeader { get; set; }
    }
}
