using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Host.Models.Domains {
    public class AppUser : IdentityUser<int> {
        public AppUser() { }

        public AppUser(string userName) : this() {
            UserName = userName;
        }
    }
}
