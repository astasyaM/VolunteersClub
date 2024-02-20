using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VolunteersClub.Models
{
    public class Mark
    {
        [Key]
        public int MarkID { get; set; }
        [ForeignKey("RecordID")]
        public int ActivityRecordID { get; set; }
        public int CurrentMark {  get; set; }
        public string Notes {  get; set; }
    }
}
