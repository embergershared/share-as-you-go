using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApp1.Models;

namespace WebApp1.Data
{
    public class WebApp1EfDbContext : DbContext
    {
        public WebApp1EfDbContext (DbContextOptions<WebApp1EfDbContext> options)
            : base(options)
        {
        }

        public DbSet<WebApp1.Models.Student> Student { get; set; } = default!;
    }
}
