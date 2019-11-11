using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Kodee_Assignment.Models
{
    public class WeatherResult
    {
        public int ID { get; set; }
        public string Zipcode { get; set; }
        public DateTime Time { get; set; }
        public decimal Temp { get; set; }
        public decimal Min_temp { get; set; }
        public decimal Max_temp { get; set; }
    }
}