using Microsoft.AspNetCore.Mvc;
using Core.Collections;
using Core.Models;

[ApiController]
[Route("api/[controller]")]
public class ExtraSubscriptionController : ControllerBase
{
    private readonly IExtraContent _extraContentCollection;

    public ExtraSubscriptionController(IExtraContent extraContentCollection)
    {
        _extraContentCollection = extraContentCollection;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExtraContent>>> GetAll()
    {
        var extras = await _extraContentCollection.GetAllAsync();
        return Ok(extras);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ExtraContent>> GetById(Guid id)
    {
        var extra = await _extraContentCollection.GetByIdAsync(id);
        if (extra == null)
        {
            return NotFound();
        }
        return Ok(extra);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var extra = await _extraContentCollection.GetByIdAsync(id);
        if (extra == null)
        {
            return NotFound();
        }
        await _extraContentCollection.DeleteAsync(id);
        return NoContent();
    }
}
