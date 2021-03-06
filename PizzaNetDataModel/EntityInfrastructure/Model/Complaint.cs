﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PizzaNetDataModel.Model;

namespace PizzaNetDataModelEntityInfrastructure.Model
{
    public class Complaint : Entity
    {
        [Key]
        public int ComplaintID { get; set; }

        [Required]
        public string Body { get; set; }
    }
}
