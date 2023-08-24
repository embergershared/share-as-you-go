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
    public class DeleteModel : PageModel
    {
        private readonly WebApp1.Data.WebApp1EfDbContext _efDbContext;

        public DeleteModel(WebApp1.Data.WebApp1EfDbContext efDbContext)
        {
            _efDbContext = efDbContext;
        }

        [BindProperty]
      public Student Student { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _efDbContext.Student == null)
            {
                return NotFound();
            }

            var student = await _efDbContext.Student.FirstOrDefaultAsync(m => m.ID == id);

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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _efDbContext.Student == null)
            {
                return NotFound();
            }
            var student = await _efDbContext.Student.FindAsync(id);

            if (student != null)
            {
                Student = student;
                _efDbContext.Student.Remove(Student);
                await _efDbContext.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
