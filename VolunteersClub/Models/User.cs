using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VolunteersClub.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }
        [ForeignKey("UserTypeID")]
        public int UserTypeID { get; set; }
        public string Password {  get; set; }
        public string Login {  get; set; }
    }
}
