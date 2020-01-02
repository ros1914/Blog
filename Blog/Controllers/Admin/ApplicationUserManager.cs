using Blog.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Blog.Controllers.Admin
{
    public class ApplicationUserManager<T>
    {
        private UserStore<ApplicationUser> userStore;
        //private UserStore<ApplicationUser> userStore1;

        //public ApplicationUserManager(UserStore<ApplicationUser> userStore1)
        //{
        //    this.userStore1 = userStore1;
        //}

       public ApplicationUserManager(UserStore<ApplicationUser> userStore)
       {
           this.userStore = userStore;
       }
    }
}