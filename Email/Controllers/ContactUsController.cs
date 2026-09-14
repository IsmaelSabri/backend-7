using Email.Models;
using Email.Services;
using Microsoft.AspNetCore.Mvc;

namespace Email.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactUsController : ControllerBase
    {
        private readonly IContactUsService _contactUsService;

        public ContactUsController(IContactUsService contactUsService)
        {
            _contactUsService = contactUsService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContactMessage>>> GetAll()
        {
            return Ok(await _contactUsService.GetAllAsync());
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ContactMessage>> GetById(Guid id)
        {
            var contact = await _contactUsService.GetByIdAsync(id);
            return contact is null ? NotFound() : Ok(contact);
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<ContactMessage>>> GetByStatus(string status)
        {
            return Ok(await _contactUsService.GetByStatusAsync(status));
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<ContactMessage>>> GetByCategory(string category)
        {
            return Ok(await _contactUsService.GetByCategoryAsync(category));
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<object>> GetDashboard()
        {
            return Ok(await _contactUsService.GetDashboardAsync());
        }

        [HttpPost("contact-message")]
        public async Task<ActionResult<ContactMessage>> CreateContactMessage([FromBody] ContactMessage message)
        {
            if (message == null)
            {
                return BadRequest();
            }

            if (string.IsNullOrWhiteSpace(message.Email))
            {
                return BadRequest("El email es obligatorio.");
            }

            var created = await _contactUsService.CreateAsync(message);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ContactMessage>> UpdateContactMessage(Guid id, [FromBody] ContactMessage message)
        {
            if (message == null)
            {
                return BadRequest();
            }

            var updated = await _contactUsService.UpdateAsync(id, message);
            return updated is null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteContactMessage(Guid id)
        {
            var deleted = await _contactUsService.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
