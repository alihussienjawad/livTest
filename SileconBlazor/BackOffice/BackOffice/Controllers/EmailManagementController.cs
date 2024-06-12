using ClassLibrary.Data;
using ClassLibrary.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailManagementController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public EmailManagementController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet("All-Emails")]
        public async Task<ActionResult<List<Contact>>> GetAllEmailsAsync()
        {
            return Ok(await context.Contacts.Include(i=>i.Service).ToListAsync());
        }

        [HttpGet("get-Email/{id}")]
        public async Task<ActionResult<List<Contact>>> GetAllEmailByIdAsync(int id)
        {
            return Ok(await context.Contacts.Where(i=>i.Id==id).Include(i => i.Service).FirstOrDefaultAsync());
        }

        [HttpDelete("Delete-Email/{id}")]
        public async  Task<ActionResult>DeleteEmailAsync(int id)
        {
            Contact? contact=await context.Contacts.FindAsync(id);
            if (contact == null) { return BadRequest(); }
            context.Contacts.Remove(contact);
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
