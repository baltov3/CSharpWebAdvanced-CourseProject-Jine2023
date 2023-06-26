﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HouseRentingSystem.Coomon.EntityValidationConstants.PhoneNumber;

namespace HouseRentingSystem.Data.Models
{
    public class Agent
    {
        public Agent()
        {
            Id = Guid.NewGuid();    
            this.OwnedHouses = new HashSet<House>();    
                    
        }

        [Key]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(PhoneNumberMaxLenght)]
        public string PhoneNumber { get; set; } = null!;
        public Guid UserId { get; set; }
        public virtual ApplicationUser User { get; set; } = null!;
        public virtual ICollection<House> OwnedHouses { get; set; }
    }
}
