using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VolunteersClub.Models
{
    public class Volunteer
    {
        [Key]
        public int VolunteerID { get; set; }
        [ForeignKey("UserID")]
        public string UserID { get; set; }
        public string Name {  get; set; }
        public string Patronymic { get; set; }
        public string Surname {  get; set; }
        public DateOnly BirthDate {  get; set; }
        [ForeignKey("EventTypeID")]
        public int EventTypeID { get; set; }
        public virtual EventType EventType { get; set; }
        [ForeignKey("VolunteerStatusID")]
        public int VolunteerStatusID {  get; set; }
        public virtual VolunteerStatus VolunteerStatus { get; set; }
        public string VK {  get; set; }
        public string Telegram {  get; set; }
    }
}
