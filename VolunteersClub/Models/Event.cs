using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VolunteersClub.Models
{
    public class Event
    {
        [Key]
        public int EventID { get; set; }
        public string EventName { get; set; }
        public DateOnly EventDate {  get; set; }
        public TimeSpan StartTime { get; set; }
        public string Duration { get; set; }
        public int VolunteersNumber { get; set; }
        [ForeignKey("EventTypeID")]
        public int EventTypeID { get; set; }
        public virtual EventType EventType { get; set; }
        public string Adress {  get; set; }
        public string EventDescription {  get; set; }
        public string Image { get; set;  }
    }
}
