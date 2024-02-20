using System.ComponentModel.DataAnnotations;

namespace VolunteersClub.Models
{
    public class EventType
    {
        [Key]
        public int EventTypeID { get; set; }
        public string EventTypeName { get; set; }
        public string Image {  get; set; }
    }
}
