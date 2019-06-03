using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IdentityManager.Data
{
    public class IdentityDbContext:IdentityDbContext<User>
    {
        public IdentityDbContext() : base("DbConnection", throwIfV1Schema: false)
        { }
        public static IdentityDbContext Create()
        {
            return new IdentityDbContext();
        }
    }
}
