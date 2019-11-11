using Kodee_Assignment.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Kodee_Assignment.Models;
using System.Threading.Tasks;

namespace Kodee_Assignment.Controllers
{
    public class WeatherController : Controller
    {

        private string _usZipRegEx = @"^\d{5}(?:[-\s]\d{4})?$";
        private const string weatherURL = "http://api.openweathermap.org/data/2.5/weather";
        private string appKeyQueryString = "APPID=a82297e58abde9d899a4dffa85b72020";

        // GET: Weather
        public ActionResult Index()
        {
            return View();
        }

        //GET: Weather/Forecast
        public ActionResult Forecast()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Forecast(ForecastViewModel vm)
        {
            if (ModelState.IsValid)
            {
                //try to parse vm address as an integer for zip code, or text for city and state
                bool isZip = false;
                if (isZipCode(vm.Address))
                    isZip = true;

                //post to openweather for temperature, min/max
                string queryString = "";

                //use zip code
                if (isZip)
                {
                    queryString = "?zip=" + vm.Address + ",us&units=imperial&" + appKeyQueryString;
                }
                else
                {
                    queryString = "?q=" + vm.Address + ",us&units=imperial&" + appKeyQueryString; 
                }

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(weatherURL);

                //add accept header for json format
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.GetAsync(queryString).Result;
                if(response.IsSuccessStatusCode)
                {
                    //parse results and set to view model details
                    string jsonRes = await response.Content.ReadAsStringAsync();
                    Forecast forecast = JsonConvert.DeserializeObject<Forecast>(jsonRes);

                    Console.WriteLine(forecast.main.temp);
                    //set view model to display result content
                    vm.containsDetails = true;

                    vm.temp = forecast.main.temp;
                    vm.temp_max = forecast.main.temp_max;
                    vm.temp_min = forecast.main.temp_min;

                    return View("ForecastDetails", vm);
                }

                client.Dispose();
            }
            return View(vm);
        }

        private bool isZipCode(string zipcode)
        {
            if (Regex.Match(zipcode, _usZipRegEx).Success)
                return true;
            return false;
        }
    }
}