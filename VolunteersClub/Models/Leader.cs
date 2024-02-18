using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VolunteersClub.Models
{
    public class Leader
    {
        [Key]
        public int LeaderID { get; set; }
        [ForeignKey("UserID")]
        public int UserID {  get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public DateTime BirthDate {  get; set; }
    }
}
