using Microsoft.AspNetCore.Mvc;
using Core.Collections;
using Core.Extensions;
using Core.Models;

[ApiController]
[Route("api/[controller]")]
public class OrganizationController : ControllerBase
{
    private readonly IOrganizationCollection _organizationCollection;

    public OrganizationController(IOrganizationCollection organizationCollection)
    {
        _organizationCollection = organizationCollection;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Organization>>> GetAll()
    {
        var organizations = await _organizationCollection.GetAllAsync();
        return Ok(organizations);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Organization>> GetById(Guid id)
    {
        var organization = await _organizationCollection.GetByIdAsync(id);
        if (organization == null)
        {
            return NotFound();
        }
        return Ok(organization);
    }

    [HttpPost]
    public async Task<ActionResult<Organization>> Create([FromBody] Organization organization)
    {
        await _organizationCollection.AddAsync(organization);
        return CreatedAtAction(nameof(GetById), new { id = organization.Id }, organization);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Organization organization)
    {
        if (!organization.Id.IsValid())
        {
            return BadRequest();
        }
        await _organizationCollection.UpdateAsync(organization);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _organizationCollection.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }
}
