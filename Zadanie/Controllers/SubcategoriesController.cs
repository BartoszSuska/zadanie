using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zadanie.Data;

namespace Zadanie.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class SubcategoriesController : ControllerBase {
        private readonly AppDbContext _context;

        public SubcategoriesController(AppDbContext context) {
            _context = context;
        }

        //zwracanie podkategorii
        [HttpGet]
        public async Task<IActionResult> Get() {
            var subcategories = await _context.Subcategories.ToListAsync();
            return Ok(subcategories);
        }
    }
}
