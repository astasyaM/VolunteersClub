using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VolunteersClub.Models
{
    public class MarkWithEventViewModel
    {
        public int MarkID { get; set; }
        public double CurrentMark { get; set; }
        public string Notes { get; set; }
        public string EventName { get; set; }
        public int EventID {  get; set; }
    }
}
