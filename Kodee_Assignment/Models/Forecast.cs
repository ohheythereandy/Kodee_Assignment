using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kodee_Assignment.Models
{
    public class Forecast
    {
        public Coord coord { get; set; }
        public List<Weather> weather { get; set; }
        public Main main { get; set; }

    }

   
}

public class Coord
{
    public string lon { get; set; }
    public string lat { get; set; }
}

public class Weather
{
    public int id { get; set; }
    public string main { get; set; }
    public string description { get; set; }
    public string icon { get; set; }
}

public class Main
{
    public float temp { get; set; }
    public int pressure { get; set; }
    public int humidity { get; set; }
    public float temp_min { get; set; }
    public float temp_max { get; set; }

}