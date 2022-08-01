using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using IoTAsema.Models;

namespace IoTAsema.Controllers
{
    public class measurementsController : Controller 
    {
        private IoTDBEntities db = new IoTDBEntities();

        public String Store(int id, string celsius, string fahrenheit, string humidity, string region, int? type)
        {
            measurements m = new measurements();
            m.DeviceID = id;
            CultureInfo en = new CultureInfo("en-US");
            DateTime localDate = DateTime.Now.AddHours(3); //lisätään kellonaikaan jotain, jottei olisi UTC, vaan HKI

            m.Celsius = double.Parse(celsius, en);
            m.Fahrenheit = double.Parse(fahrenheit, en);
            m.Humidity = double.Parse(humidity, en);
            m.Time = localDate;
            m.Type = type ?? 1; //jos mittauksen tyyppi on annettu, käytetään sitä, muuten oletetaan type=1 (lämpötila)
            m.Region = region;
            db.measurements.Add(m);
            db.SaveChanges();
            return "ok";
        }

        public ActionResult GetWeather(string key, int? id, int? type)
        {
            measurements m = new measurements();
            WebClient client = new WebClient();
            m.DeviceID = id ?? 87;
            CultureInfo en = new CultureInfo("en-US");
            DateTime localdate = DateTime.Now.AddHours(3);
            var mittaus = db.measurements.Include(d => d.devices);
            try
            {
                string weather = client.DownloadString("https://ilmatieteenlaitos.fi/saa/" + key);
                int index = weather.IndexOf("Temperature");
                if (index > 0)
                {
                    string data = weather.Substring(index + 12, 3).Replace(":", "").Replace("e", "");
                    m.Celsius = double.Parse(data, en);
                    m.Fahrenheit = double.Parse(data, en) * (9/5) + 32;
                    m.Time = localdate;
                    m.Type = type ?? 2;
                    m.Region = key;
                    db.measurements.Add(m);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            finally
            {
                client.Dispose();
            }
            return HttpNotFound("Unknown");
        }


        // GET: measurements
        public ActionResult Index(string searchstring)
        {
            var measurements = db.measurements.Include(m => m.devices);

            if (!string.IsNullOrEmpty(searchstring))
            {
                measurements = measurements.Where(a => a.devices.Device.Contains(searchstring)); // Hakee laitteen!
            }

            return View(measurements.ToList());
        }

        // GET: measurements/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            measurements measurements = db.measurements.Find(id);
            if (measurements == null)
            {
                return HttpNotFound();
            }
            return View(measurements);
        }

        // GET: measurements/Create
        public ActionResult Create()
        {
            ViewBag.DeviceID = new SelectList(db.devices, "DeviceID", "Device");
            return View();
        }

        // POST: measurements/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MeasurementID,DeviceID,Time,Type,Celsius,Fahrenheit,Humidity")] measurements measurements)
        {
            if (ModelState.IsValid)
            {
                db.measurements.Add(measurements);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DeviceID = new SelectList(db.devices, "DeviceID", "Device", measurements.DeviceID);
            return View(measurements);
        }

        // GET: measurements/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            measurements measurements = db.measurements.Find(id);
            if (measurements == null)
            {
                return HttpNotFound();
            }
            ViewBag.DeviceID = new SelectList(db.devices, "DeviceID", "Device", measurements.DeviceID);
            return View(measurements);
        }

        // POST: measurements/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MeasurementID,DeviceID,Time,Type,Celsius,Fahrenheit,Humidity")] measurements measurements)
        {
            if (ModelState.IsValid)
            {
                db.Entry(measurements).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DeviceID = new SelectList(db.devices, "DeviceID", "Device", measurements.DeviceID);
            return View(measurements);
        }

        // GET: measurements/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            measurements measurements = db.measurements.Find(id);
            if (measurements == null)
            {
                return HttpNotFound();
            }
            return View(measurements);
        }

        // POST: measurements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            measurements measurements = db.measurements.Find(id);
            db.measurements.Remove(measurements);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
