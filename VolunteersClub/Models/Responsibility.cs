using System.ComponentModel.DataAnnotations;

namespace VolunteersClub.Models
{
    public class Responsibility
    {
        [Key]
        public int ResponsibilityID { get; set; }
        public string ResponsibilityName { get; set;}
    }
}
