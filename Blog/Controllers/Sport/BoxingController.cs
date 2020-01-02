using Blog.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Blog.Controllers.Sport
{
   
    
        [Authorize(Roles = "User")]
        public class BoxingController : Controller
        {
            // GET: boxing
            public ActionResult Boxings()
            {
                return RedirectToAction("List"); //View();
            }

        public ActionResult Search(string searchString)
        {
            using (BlogDbContext database = new BlogDbContext())
            {
                var boxing = from m in database.Boxings
                             select m;

                if (!String.IsNullOrEmpty(searchString))
                {
                    boxing = boxing.Where(s => s.Title.Contains(searchString)); //.OrderByDescending(p => p.Date);
                }
                return View(boxing.ToList());

            }
        }

            // GET: boxing/List
            public ActionResult List()
            {
                using (var database = new BlogDbContext())
                {
                    // Get Boxing from database
                    var Boxings = database.Boxings
                        .Include(a => a.Author)
                        //.Include(a => a.Tags)
                        .ToList();


                    return View(Boxings);

                }
            }

            //
            // GET:Boxing/Details
            public ActionResult Details(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                }

                using (var database = new BlogDbContext())
                {
                    // Get the boxing from database
                    var boxing = database.Boxings
                        .Where(a => a.Id == id)
                        .Include(a => a.Author)
                        //.Include(a => a.Tags)
                        .First();

                    if (boxing == null)
                    {
                        return HttpNotFound();
                    }

                    return View(boxing);
                }
            }


            //
            // GET: Food/Create Boxing
            [Authorize]
            public ActionResult Create()
            {
                using (var database = new BlogDbContext())
                {
                    var model = new BoxingViewModel();

                    return View(model);

                }
            }

            //
            // POST: boxing/Create Boxing
            [HttpPost]
            [Authorize]
            public ActionResult Create(BoxingViewModel model, HttpPostedFileBase file)
            {
                if (ModelState.IsValid)
                {
                    // Insert boxing in DB

                    using (var database = new BlogDbContext())
                    {
                        // Get author id
                        var authorId = database.Users
                            .Where(u => u.UserName == this.User.Identity.Name)
                            .First()
                            .Id;




                        var boxing = new Blog.Models.Boxing(authorId, model.Title, model.Content);

                        if (file != null)
                        {
                            boxing.Image = new byte[file.ContentLength];
                            file.InputStream.Read(boxing.Image, 0, file.ContentLength);
                        }
                        //var boxing = new boxing(authorId, model.Title, model.Content, model.CategoryId);
                        //
                        //this.SetBoxingTags(boxing, model, database);

                        // Save boxing in DB
                        database.Boxings.Add(boxing);
                        database.SaveChanges();

                        return RedirectToAction("List");

                    }
                }

                return View(model);
            }

            //
            // GET: boxing/Delete
            public ActionResult Delete(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                using (var database = new BlogDbContext())
                {
                    // Get boxing from database
                    var boxing = database.Boxings
                        .Where(a => a.Id == id)
                        .Include(a => a.Author)
                        //.Include(a => a.Category)
                        .First();

                    // if (!IsUserAuthorizedToEdit(boxing))
                    // {
                    //     return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                    // }
                    //
                    // ViewBag.TagsStrings = string.Join(", ", boxing.Tags.Select(t => t.Name));

                    // Check if Boxing exists
                    if (boxing == null)
                    {
                        return HttpNotFound();
                    }

                    // Pass Boxing to view
                    return View(boxing);
                }
            }


            //POST: //
            //POST: Boxing/Delete
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
                    // Get Boxing from database
                    var boxing = database.Boxings
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
                    .First();

                    //Check if boxing exists
                    if (boxing == null)
                    {
                        return HttpNotFound();
                    }
                    // Delete boxing from database
                    //Remove boxing from db
                    database.Boxings.Remove(boxing);
                    database.SaveChanges();

                    // Redirect to index page
                    return RedirectToAction("List");
                }
            }

            //
            // GET: boxing/Edit
            public ActionResult Edit(int? id)
            {
                if (id == null)
                {

                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                using (var database = new BlogDbContext())
                {
                    // Get boxing from database
                    var boxing = database.Boxings
                    .Where(a => a.Id == id)
                    .First();

                    // if (!IsUserAuthorizedToEdit(boxing))
                    // {
                    //     return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                    // }
                    // Check if boxing exists
                    if (boxing == null)
                    {
                        return HttpNotFound();
                    }
                    // Create the view model

                    var model = new BoxingViewModel();
                    model.Id = boxing.Id;
                    model.Title = boxing.Title;
                    model.Content = boxing.Content;
                    // model.CategoryId = boxing.CategoryId;
                    // model.Categories = database.Categories
                    // .OrderBy(c => c.Name)
                    // .ToList();

                    // model.Tags = string.Join(", ", boxing.Tags.Select(t => t.Name));

                    // Pass the view model to view 
                    return View(model);
                }
            }

            // 
            // POST: boxing/Edit
            [HttpPost]
            public ActionResult Edit(BoxingViewModel model)
            {
                // Check if model state is valid
                if (ModelState.IsValid)
                {
                    using (var database = new BlogDbContext())
                    {
                        // Get boxing from database 
                        var boxing = database.Boxings
                            .FirstOrDefault(a => a.Id == model.Id);
                        // Set boxing properties
                        boxing.Title = model.Title;
                        boxing.Content = model.Content;
                        // boxing.CategoryId = model.CategoryId;
                        // this.SetboxingTags(boxing, model, database);
                        // Save boxing state in database
                        database.Entry(boxing).State = EntityState.Modified;
                        database.SaveChanges();
                        // Redirect to the index page
                        return RedirectToAction("List");
                    }
                }

                // If model state is invalid, return the same view
                return View(model);
            }

            // private void SetboxingTags(boxing boxing, boxingViewModel model, BlogDbContext db)
            // {
            //     // Split tags
            //     var tagsStrings = model.Tags.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(t => t.ToLower()).Distinct();
            //
            //     // Clear all current boxing tags
            //     boxing.Tags.Clear();
            //
            //     // Set new boxing tags
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
            //             boxing.Tags.Add(tag);
            //         }
            //     }
            // }



            private bool IsUserAuthorizedToEdit(Models.Boxing boxing)
            {
                bool isAdmin = this.User.IsInRole("Admin");
                bool isAuthor = boxing.IsAuthor(this.User.Identity.Name);

                return isAdmin || isAuthor;
            }

        }
    
}