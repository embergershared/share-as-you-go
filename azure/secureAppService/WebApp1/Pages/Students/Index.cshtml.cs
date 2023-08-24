using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp1.Data;
using WebApp1.Models;

namespace WebApp1.Pages.Students
{
    public class IndexModel : PageModel
    {
        private readonly WebApp1.Data.WebApp1EfDbContext _efDbContext;

        public IndexModel(WebApp1.Data.WebApp1EfDbContext efDbContext)
        {
            _efDbContext = efDbContext;
        }

        public IList<Student> Student { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_efDbContext.Students != null)
            {
                Student = await _efDbContext.Students.ToListAsync();
            }
        }
    }
}
