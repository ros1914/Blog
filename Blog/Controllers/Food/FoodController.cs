using Blog.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Blog.Controllers.Food
{
    [Authorize(Roles = "User")]
    public class FoodController : Controller
    {
        // GET: Salat
        public ActionResult Foods()
        {
            return RedirectToAction("List"); //View();
        }

        public ActionResult Search(string searchString)
        {
            using (BlogDbContext database = new BlogDbContext())
            {
                var food = from m in database.Foods
                           select m;

                if (!String.IsNullOrEmpty(searchString))
                {
                    food = food.Where(s => s.Title.Contains(searchString));
                }
                return View(food.ToList());

            }
        }

        // GET: Salat/List
        public ActionResult List()
        {
            using (var database = new BlogDbContext())
            {
                // Get Foods from database
                var Foods = database.Foods
                    .Include(a => a.Author)
                    //.Include(a => a.Tags)
                    .ToList();


                return View(Foods);

            }
        }

        //
        // GET:Foods/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }

            using (var database = new BlogDbContext())
            {
                // Get the Salat from database
                var salat = database.Foods
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
                    //.Include(a => a.Tags)
                    .First();

                if (salat == null)
                {
                    return HttpNotFound();
                }

                return View(salat);
            }
        }


        //
        // GET: Food/Create Foods
        [Authorize]
        public ActionResult Create()
        {
            using (var database = new BlogDbContext())
            {
                var model = new FoodViewModel();

                return View(model);

            }
        }

        //
        // POST: Salat/Create Foods
        [HttpPost]
        [Authorize]
        public ActionResult Create(FoodViewModel model, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                // Insert salat in DB

                using (var database = new BlogDbContext())
                {
                    // Get author id
                    var authorId = database.Users
                        .Where(u => u.UserName == this.User.Identity.Name)
                        .First()
                        .Id;


                   

                    var food = new Blog.Models.Food(authorId, model.Title, model.Content);

                    if (file != null)
                    {
                        food.Image = new byte[file.ContentLength];
                        file.InputStream.Read(food.Image, 0, file.ContentLength);
                    }
                    //var salat = new Salat(authorId, model.Title, model.Content, model.CategoryId);
                    //
                    //this.SetFoodsTags(salat, model, database);

                    // Save Salat in DB
                    database.Foods.Add(food);
                    database.SaveChanges();

                    return RedirectToAction("List");

                }
            }

            return View(model);
        }

        //
        // GET: Salat/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                // Get Salat from database
                var salat = database.Foods
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
                    //.Include(a => a.Category)
                    .First();

               // if (!IsUserAuthorizedToEdit(salat))
               // {
               //     return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
               // }
               //
               // ViewBag.TagsStrings = string.Join(", ", salat.Tags.Select(t => t.Name));

                // Check if Foods exists
                if (salat == null)
                {
                    return HttpNotFound();
                }

                // Pass Foods to view
                return View(salat);
            }
        }


        //POST: //
        //POST: Foods/Delete
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                // Get Foods from database
                var salat = database.Foods
                .Where(a => a.Id == id)
                .Include(a => a.Author)
                .First();

                //Check if salat exists
                if (salat == null)
                {
                    return HttpNotFound();
                }
                // Delete salat from database
                //Remove salat from db
                database.Foods.Remove(salat);
                database.SaveChanges();

                // Redirect to index page
                return RedirectToAction("List");
            }
        }

        //
        // GET: Salat/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                // Get salat from database
                var salat = database.Foods
                .Where(a => a.Id == id)
                .First();

               // if (!IsUserAuthorizedToEdit(salat))
               // {
               //     return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
               // }
                // Check if salat exists
                if (salat == null)
                {
                    return HttpNotFound();
                }
                // Create the view model

                var model = new FoodViewModel();
                model.Id = salat.Id;
                model.Title = salat.Title;
                model.Content = salat.Content;
               // model.CategoryId = salat.CategoryId;
               // model.Categories = database.Categories
                   // .OrderBy(c => c.Name)
                   // .ToList();

               // model.Tags = string.Join(", ", salat.Tags.Select(t => t.Name));

                // Pass the view model to view 
                return View(model);
            }
        }

        // 
        // POST: salat/Edit
        [HttpPost]
        public ActionResult Edit(FoodViewModel model)
        {
            // Check if model state is valid
            if (ModelState.IsValid)
            {
                using (var database = new BlogDbContext())
                {
                    // Get salat from database 
                    var salat = database.Foods
                        .FirstOrDefault(a => a.Id == model.Id);
                    // Set salat properties
                    salat.Title = model.Title;
                    salat.Content = model.Content;
                   // salat.CategoryId = model.CategoryId;
                   // this.SetsalatTags(salat, model, database);
                    // Save salat state in database
                    database.Entry(salat).State = EntityState.Modified;
                    database.SaveChanges();
                    // Redirect to the index page
                    return RedirectToAction("List");
                }
            }

            // If model state is invalid, return the same view
            return View(model);
        }

       // private void SetsalatTags(Salat salat, SalatViewModel model, BlogDbContext db)
       // {
       //     // Split tags
       //     var tagsStrings = model.Tags.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(t => t.ToLower()).Distinct();
       //
       //     // Clear all current salat tags
       //     salat.Tags.Clear();
       //
       //     // Set new salat tags
       //     foreach (var tagString in tagsStrings)
       //     {
       //         // get tag ....
       //         Tag tag = db.Tags.FirstOrDefault(t => t.Name.Equals(tagString));
       //
       //         // if the tag ..... 
       //         if (tag == null)
       //         {
       //             tag = new Tag() { Name = tagString };
       //             db.Tags.Add(tag);
       //
       //             // Add tag....
       //             salat.Tags.Add(tag);
       //         }
       //     }
       // }


       
        private bool IsUserAuthorizedToEdit(Models.Food salat)
        {
            bool isAdmin = this.User.IsInRole("Admin");
            bool isAuthor = salat.IsAuthor(this.User.Identity.Name);

            return isAdmin || isAuthor;
        }

    }
}