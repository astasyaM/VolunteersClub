﻿using System.ComponentModel.DataAnnotations;

namespace VolunteersClub.Models
{
    public class VolunteerStatus
    {
        [Key]
        public int VolunteerStatusID {  get; set; }
        public string Status { get; set;  }
    }
}
