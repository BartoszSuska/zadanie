using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zadanie.Data;
using Zadanie.Models;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ContactsController : ControllerBase {
    private readonly AppDbContext _context;

    public ContactsController(AppDbContext context) {
        _context = context;
    }

    //sprawdzanie siły hasła
    private bool IsPasswordValid(string password) {
        if (password.Length < 8)
            return false;
        if (!password.Any(char.IsUpper))
            return false;
        if (!password.Any(char.IsLower))
            return false;
        if (!password.Any(char.IsDigit))
            return false;
        if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
            return false;
        return true;
    }

    //lista kontaktów, dostępna dla wsyzstkich użytkowników, nawet niezalogowanych
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Get() {
        var contacts = await _context.Contacts.ToListAsync();
        return Ok(contacts);
    }

    
    //zwracanie kontaktu po id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id) {
        var contact = await _context.Contacts
            .Include(c => c.Category)
            .Include(c => c.Subcategory)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (contact == null) {
            return NotFound();
        }

        return Ok(contact);
    }

    //tworzenie kontaktu, sprawdzanie unikalności emaila i złożoności hasła
    [HttpPost]
    public async Task<IActionResult> Create(Contact contact) {
        if(await _context.Contacts.AnyAsync(c => c.Email == contact.Email))
            return BadRequest("Email already exists.");

        if(!IsPasswordValid(contact.Password))
            return BadRequest("Password is too weak.");

        _context.Contacts.Add(contact);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = contact.Id }, contact);
    }

    //aktualizacja kontaktu, sprawdzanie unikalności emaila i złożoności hasła
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Contact contact) {
        if (id != contact.Id) return BadRequest();

        _context.Entry(contact).State = EntityState.Modified;

        //sprawdzenie unikalności email
        if (await _context.Contacts.AnyAsync(c => c.Email == contact.Email && c.Id != id))
            return BadRequest("Email already exists.");

        //sprawdzenie złożoności hasła
        if (!IsPasswordValid(contact.Password))
            return BadRequest("Password is too weak.");

        try {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) {
            if (!await _context.Contacts.AnyAsync(c => c.Id == id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    //usuwanie kontaktu
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id) {
        var contact = await _context.Contacts.FindAsync(id);
        if (contact == null) {
            return NotFound();
        }

        _context.Contacts.Remove(contact);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
