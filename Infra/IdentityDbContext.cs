using IdentityApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityApi.Infra
{
    public class IdentityDbContext: DbContext
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options):base(options)
        {

        }

        public DbSet<User> Users { get; set; }
    }
}
