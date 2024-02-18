﻿using System.ComponentModel.DataAnnotations.Schema;
using VolunteersClub.Data.Enum;
using System.ComponentModel.DataAnnotations;

namespace VolunteersClub.Models
{
    public class Volunteer
    {
        [Key]
        public int VolunteerID { get; set; }
        [ForeignKey("UserID")]
        public int UserID { get; set; }
        public string Name {  get; set; }
        public string Patronymic { get; set; }
        public string Surname {  get; set; }
        public DateTime BirthDate {  get; set; }
        [ForeignKey("EventTypeID")]
        public int EventTypeID { get; set; }
        [ForeignKey("VolunteerStatusID")]
        public int VolunteerStatusID {  get; set; }
    }
}