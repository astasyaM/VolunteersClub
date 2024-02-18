using System.ComponentModel.DataAnnotations;
using VolunteersClub.Data;

namespace VolunteersClub.Models
{
    public class UserType
    {
        [Key]
        public int UserTypeID {  get; set; }
        public string UserName { get; set; }
    }
}
