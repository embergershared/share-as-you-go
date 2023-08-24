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
    public class DetailsModel : PageModel
    {
        private readonly WebApp1.Data.WebApp1EfDbContext _efDbContext;

        public DetailsModel(WebApp1.Data.WebApp1EfDbContext efDbContext)
        {
            _efDbContext = efDbContext;
        }

      public Student Student { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _efDbContext.Students == null)
            {
                return NotFound();
            }

            //var student = await _efDbContext.Students.FirstOrDefaultAsync(m => m.ID == id);

            var student = await _efDbContext.Students
                .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            if (student == null)
            {
                return NotFound();
            }
            else 
            {
                Student = student;
            }

            return Page();
        }
    }
}
