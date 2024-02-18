using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VolunteersClub.Data.Enum;

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
        public bool VolunteerConfirmed { get; set; }
        public bool LeaderConfirmed { get; set; }
    }
}
