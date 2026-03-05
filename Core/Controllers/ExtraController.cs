using Microsoft.AspNetCore.Mvc;
using Core.Collections;
using Core.Extensions;
using Core.Models;

[ApiController]
[Route("api/[controller]")]
public class ExtraController : ControllerBase
{
    private readonly IExtraCollection _extraCollection;

    public ExtraController(IExtraCollection extraCollection)
    {
        _extraCollection = extraCollection;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Extra>>> GetAll()
    {
        var extras = await _extraCollection.GetAllAsync();
        return Ok(extras);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Extra>> GetById(Guid id)
    {
        var extra = await _extraCollection.GetByIdAsync(id);
        if (extra == null)
        {
            return NotFound();
        }
        return Ok(extra);
    }

    [HttpPost]
    public async Task<ActionResult<Extra>> Create([FromBody] Extra extra)
    {
        await _extraCollection.AddAsync(extra);
        return CreatedAtAction(nameof(GetById), new { id = extra.Id }, extra);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Extra extra)
    {
        if (!extra.Id.IsValid())
        {
            return BadRequest();
        }
        await _extraCollection.UpdateAsync(extra);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _extraCollection.DeleteAsync(id);
        return NoContent();
    }
}