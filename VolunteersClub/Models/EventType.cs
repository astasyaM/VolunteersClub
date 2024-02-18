using System.ComponentModel.DataAnnotations;

namespace VolunteersClub.Models
{
    public class EventType
    {
        [Key]
        public int EventTypeID { get; set; }
        public string EventName { get; set; }
    }
}
