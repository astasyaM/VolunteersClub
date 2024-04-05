namespace VolunteersClub.Models
{
    public class EventViewModel
    {
        public IEnumerable<Event> Events { get; set; }
        public IEnumerable<EventType> EventTypes { get; set; }
    }
}
