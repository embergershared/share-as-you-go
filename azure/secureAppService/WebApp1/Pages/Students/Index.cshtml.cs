using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebApp1.Models;

namespace WebApp1.Pages.Students
{
    public class IndexModel : PageModel
    {
        private readonly Data.WebApp1EfDbContext _efDbContext;
        private readonly IConfiguration _configuration;

        public IndexModel(
            Data.WebApp1EfDbContext efDbContext,
            IConfiguration configuration
        )
        {
            _efDbContext = efDbContext;
            _configuration = configuration;
        }

        public IList<Student> Student { get;set; } = default!;

        public async Task OnGetAsync()
        {
            ViewData["EfContext"] = "WebApp1EfDbContext-MI";
            ViewData["connString"] = _configuration.GetConnectionString("WebApp1EfDbContext-MI");

            if (_efDbContext.Students != null)
            {
                Student = await _efDbContext.Students.ToListAsync();
            }
        }
    }
}
