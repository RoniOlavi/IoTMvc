using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using IoTAsema.Models;

namespace IoTAsema.Controllers
{
    public class commandsController : Controller
    {
        private IoTDBEntities db = new IoTDBEntities();

        // GET: commands
        public ActionResult Index()
        {
            var commands = db.commands.Include(c => c.devices);
            return View(commands.ToList());
        }

        public string GetCommand(int id) // Raspberryn etäohjausta varten!
        {
            string command = (from c in db.commands
                              where (c.DeviceID == id) && (c.Executed == false)
                              select c.Command).FirstOrDefault();
            if (command == null)
                command = "";
            return command;
        }
        public string Completed(int id) // Raspberryn etäohjausta varten!
        {
            commands command = (from c in db.commands
                                where (c.DeviceID == id) && (c.Executed == false)
                                select c).FirstOrDefault();
            if (command != null)
            {
                command.Executed = true;
                db.SaveChanges();
            }
            return "Ok";
        }

        // GET: commands/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            commands commands = db.commands.Find(id);
            if (commands == null)
            {
                return HttpNotFound();
            }
            return View(commands);
        }

        // GET: commands/Create
        public ActionResult Create()
        {
            ViewBag.DeviceID = new SelectList(db.devices, "DeviceID", "Device");
            return View();
        }

        // POST: commands/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CommandID,DeviceID,Command,Executed")] commands commands)
        {
            if (ModelState.IsValid)
            {
                db.commands.Add(commands);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DeviceID = new SelectList(db.devices, "DeviceID", "Device", commands.DeviceID);
            return View(commands);
        }

        // GET: commands/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            commands commands = db.commands.Find(id);
            if (commands == null)
            {
                return HttpNotFound();
            }
            ViewBag.DeviceID = new SelectList(db.devices, "DeviceID", "Device", commands.DeviceID);
            return View(commands);
        }

        // POST: commands/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CommandID,DeviceID,Command,Executed")] commands commands)
        {
            if (ModelState.IsValid)
            {
                db.Entry(commands).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DeviceID = new SelectList(db.devices, "DeviceID", "Device", commands.DeviceID);
            return View(commands);
        }

        // GET: commands/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            commands commands = db.commands.Find(id);
            if (commands == null)
            {
                return HttpNotFound();
            }
            return View(commands);
        }

        // POST: commands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            commands commands = db.commands.Find(id);
            db.commands.Remove(commands);
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
