namespace VolunteersClub.Models
{
    public class ViewEventForVolunteers
    {
        public IEnumerable<Event> Events { get; set; }
        public IEnumerable<EventType> EventTypes { get; set; }
    }
}
