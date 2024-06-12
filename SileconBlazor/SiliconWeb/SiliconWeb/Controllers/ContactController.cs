using ClassLibrary.Data;
using ClassLibrary.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SiliconWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public ContactController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet("GetServices")]
        public async Task<ActionResult<List<Service>>> GetAllServicesAsync()
        {
            return Ok(await context.Services.ToListAsync());
        }

        [HttpPost("SendContact")]
        public async Task<ActionResult> SendContactAsync(Contact contact)
        {
            if(ModelState.IsValid)
            {
                context.Contacts.Add(contact);
                await context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest(ModelState);
        }
    }
}
