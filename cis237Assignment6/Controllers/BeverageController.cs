using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using cis237Assignment6.Models;

namespace cis237Assignment6.Controllers
{
    [Authorize]
    public class BeverageController : Controller
    {
        private BeverageNSmithEntities db = new BeverageNSmithEntities();

        // GET: /Beverage/
        public ActionResult Index()
        {
            //Setup a variable to hold beverage
            DbSet<Beverage> BeveragesToFilter = db.Beverages;

            //Setup strings to hold the search parameters from session
            string filterName = "";
            string filterPackage = "";
            string filterMin = "";
            string filterMax = "";

            //Define a min and a max for the pricing
            int min = 0;
            int max = 340;

            //Check to see if there is a value in the session, and if there is, apply it to its appropriate variable
            if(Session["name"] != null && !String.IsNullOrWhiteSpace((string)Session["name"]))
            {
                filterName = (string)Session["name"];
            }

            if(Session["package"] != null && !String.IsNullOrWhiteSpace((string)Session["package"]))
            {
                filterPackage = (string)Session["package"];
            }

            try
            {
                if (Session["min"] != null && !String.IsNullOrWhiteSpace((string)Session["min"]))
                {
                    filterMin = (string)Session["min"];
                    min = Int32.Parse(filterMin);
                }
            }
            catch
            {

            }

            try
            {
                if (Session["max"] != null && !String.IsNullOrWhiteSpace((string)Session["max"]))
                {
                    filterMax = (string)Session["max"];
                    max = Int32.Parse(filterMax);
                }
            }
            catch
            {

            }

            //Do the filter on the BeveragesToSearch Dataset, using where with multiple lambdas to get multiple results
            IEnumerable<Beverage> filtered = BeveragesToFilter.Where(beverage => beverage.price >= min &&
                                                                                                    beverage.price <= max &&
                                                                                                    beverage.name.Contains(filterName) &&
                                                                                                    beverage.pack.Contains(filterPackage));
            
            //Convert the database set to a list so the view can display it
            IEnumerable<Beverage> finalFiltered = filtered.ToList();

            //Place the string representation of the values that were searched by the user into the view bag
            //So that they are still in the text boxes in the view for the user to see
            ViewBag.filterName = filterName;
            ViewBag.filterPackage = filterPackage;
            ViewBag.filterMin = filterMin;
            ViewBag.filterMax = filterMax;

            //Return the filtered view instead of the original view
            return View(finalFiltered);
        }

        // GET: /Beverage/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beverage beverage = db.Beverages.Find(id);
            if (beverage == null)
            {
                return HttpNotFound();
            }
            return View(beverage);
        }

        // GET: /Beverage/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Beverage/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="id,name,pack,price,active")] Beverage beverage)
        {
            if (ModelState.IsValid)
            {
                db.Beverages.Add(beverage);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(beverage);
        }

        // GET: /Beverage/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beverage beverage = db.Beverages.Find(id);
            if (beverage == null)
            {
                return HttpNotFound();
            }
            return View(beverage);
        }

        // POST: /Beverage/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="id,name,pack,price,active")] Beverage beverage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(beverage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(beverage);
        }

        // GET: /Beverage/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beverage beverage = db.Beverages.Find(id);
            if (beverage == null)
            {
                return HttpNotFound();
            }
            return View(beverage);
        }

        // POST: /Beverage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Beverage beverage = db.Beverages.Find(id);
            db.Beverages.Remove(beverage);
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

        //This method will allow the user to search through the data in the index via a series of text boxes in the view
        public ActionResult Filter()
        {
            //Get the form data into the session saved so it can be retrieved to filter, or narrow the data down to the user's input
            string name = Request.Form.Get("name");
            string package = Request.Form.Get("package");
            string min = Request.Form.Get("min");
            string max = Request.Form.Get("max");

            //This actually stores the data in the session for later use
            Session["name"] = name;
            Session["package"] = package;
            Session["min"] = min;
            Session["max"] = max;

            //This redirects the user to the index page, where they will view the now filtered data
            return RedirectToAction("Index");
        }
    }
}
