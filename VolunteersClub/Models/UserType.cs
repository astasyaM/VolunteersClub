using System.ComponentModel.DataAnnotations;
using VolunteersClub.Data;

namespace VolunteersClub.Models
{
    public class UserType
    {
        [Key]
        public int TypeID {  get; set; }
        public string Type { get; set; }
    }
}
