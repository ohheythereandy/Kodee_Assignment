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
using Kodee_Assignment.Models.Context;

namespace Kodee_Assignment.Controllers
{
    public class WeatherController : Controller
    {
        private WeatherContext db = new WeatherContext();
        private string _usZipRegEx = @"^\d{5}(?:[-\s]\d{4})?$";
        private const string weatherURL = "http://api.openweathermap.org/data/2.5/weather";
        private string appKeyQueryString = "APPID=a82297e58abde9d899a4dffa85b72020";

        // GET: Weather
        public ActionResult Index()
        {
            var forecasts = from w in db.WeatherResult
                            select w;
            return View(forecasts.ToList());
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
                bool isZip = isZipCode(vm.Address) ? true : false;

                //post to openweather for temperature, min/max
                string queryString = "";

                if (isZip)
                {
                    //check to see if there exists a record with same zip code captured in last 30 minutes
                    WeatherResult cacheRes = getFreshCache(vm.Address, DateTime.Now);
                    
                    if (cacheRes != null)
                    {
                        vm.temp = cacheRes.Temp;
                        vm.temp_max = cacheRes.Max_temp;
                        vm.temp_min = cacheRes.Min_temp;
                        vm.cacheHit = true;
                        return View("ForecastDetails", vm);
                    }
                    queryString = "?zip=" + vm.Address + ",us&units=imperial&" + appKeyQueryString;
                }
                else
                {
                    queryString = "?q=" + vm.Address + ",us&units=imperial&" + appKeyQueryString; 
                }

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(weatherURL);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync(queryString).Result;
                if(response.IsSuccessStatusCode)
                {
                    //parse results and set to view model details
                    string jsonRes = await response.Content.ReadAsStringAsync();
                    Forecast forecast = JsonConvert.DeserializeObject<Forecast>(jsonRes);
                    
                    vm.temp = forecast.main.temp;
                    vm.temp_max = forecast.main.temp_max;
                    vm.temp_min = forecast.main.temp_min;

                    //if request was by zip, cache results for later storage
                    if (isZip)
                        storeWeatherResults(vm.Address, DateTime.Now, vm.temp, vm.temp_max, vm.temp_min);
                    
                    return View("ForecastDetails", vm);
                }

                client.Dispose();
            }
            return View(vm);
        }

        private WeatherResult getFreshCache(string zip, DateTime now)
        {
            DateTime windowStart = now.Add(new TimeSpan(0, -30, 0));
            WeatherResult res = db.WeatherResult.Where(m => m.Zipcode == zip && m.Time >= windowStart && m.Time <= now).FirstOrDefault();
            return res;
        }

        private void storeWeatherResults(string address, DateTime now, decimal temp, decimal max_temp, decimal min_temp)
        {
            WeatherResult weather_res = new WeatherResult
            {
                Zipcode = address,
                Time = now,
                Temp = temp,
                Max_temp = max_temp,
                Min_temp = min_temp
            };
            db.WeatherResult.Add(weather_res);
            db.SaveChanges();

        }

        private bool isZipCode(string zipcode)
        {
            if (Regex.Match(zipcode, _usZipRegEx).Success)
                return true;
            return false;
        }
    }
}