using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;

namespace IoTAsema.Controllers
{
    public class WeatherController : Controller
    {
        // GET: Weather
        public ActionResult Index()
        {
            return View();
        }

        public string GetWeather(string key)
        {
            WebClient client = new WebClient();
            try
            {
                string weather = client.DownloadString("https://ilmatieteenlaitos.fi/saa/" + key);
                int index = weather.IndexOf("Temperature");
                if (index > 0)
                {
                    string data = weather.Substring(index + 40, 3).Replace(":", "").Replace("e", "");
                    return ("Sää paikkakunnalla : " + key + " " + data + " astetta.");
                }
            }
            finally
            {
                client.Dispose();
            }
            return ("Unknown");
        }
    }
}