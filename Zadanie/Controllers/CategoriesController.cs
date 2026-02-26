using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zadanie.Data;

namespace Zadanie.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context) {
            _context = context;
        }


        //zwracanie kategorii
        [HttpGet]
        public async Task<IActionResult> Get() {
            var categories = await _context.Categories.ToListAsync();
            return Ok(categories);
        }
    }
}
