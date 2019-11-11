using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Kodee_Assignment.Models.ViewModels
{
    public class ForecastViewModel
    {
        [Required(ErrorMessage= "Please enter a valid address.")]
        public string Address { get; set; }
        public bool containsDetails { get; set; } = false;
        public float temp { get; set; }
        public float temp_min { get; set; }
        public float temp_max { get; set; }
/*        public string City { get; set; }
        public string State { get; set; }
        public string County { get; set; }
        public string Zip { get; set; }*/
    }
}