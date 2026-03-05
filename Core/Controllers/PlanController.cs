using Microsoft.AspNetCore.Mvc;
using Core.Collections;
using Core.Extensions;
using Core.Models;

[ApiController]
[Route("api/[controller]")]
public class PlanController : ControllerBase
{
    private readonly IPlanCollection _planCollection;

    public PlanController(IPlanCollection planCollection)
    {
        _planCollection = planCollection;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Plan>>> GetAll()
    {
        var plans = await _planCollection.GetAllAsync();
        return Ok(plans);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Plan>> GetById(Guid id)
    {
        var plan = await _planCollection.GetByIdAsync(id);
        if (plan == null)
        {
            return NotFound();
        }
        return Ok(plan);
    }

    [HttpPost]
    public async Task<ActionResult<Plan>> Create([FromBody] Plan plan)
    {
        await _planCollection.AddAsync(plan);
        return CreatedAtAction(nameof(GetById), new { id = plan.Id }, plan);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Plan plan)
    {
        if (!plan.Id.IsValid())
        {
            return BadRequest();
        }
        await _planCollection.UpdateAsync(plan);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _planCollection.DeleteAsync(id);
        return NoContent();
    }
}