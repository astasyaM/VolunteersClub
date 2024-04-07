using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VolunteersClub.Models
{
    public class VolunteersForMarks
    {
        public int VolunteerID { get; set; }
        public string UserID { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public string Surname { get; set; }
        public DateOnly BirthDate { get; set; }
        public int EventTypeID { get; set; }
        public virtual EventType EventType { get; set; }
        public int VolunteerStatusID { get; set; }
        public virtual VolunteerStatus VolunteerStatus { get; set; }
        public string VK { get; set; }
        public string Telegram { get; set; }
        public int RecordID { get; set; }
    }
}
