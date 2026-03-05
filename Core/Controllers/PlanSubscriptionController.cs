using Microsoft.AspNetCore.Mvc;
using Core.Collections;
using Core.Enums;
using Core.Models;

[ApiController]
[Route("api/[controller]")]
public class PlanSubscriptionController : ControllerBase
{
    private readonly IPlanSubscriptionCollection _planSubscriptionCollection;

    public PlanSubscriptionController(IPlanSubscriptionCollection planSubscriptionCollection)
    {
        _planSubscriptionCollection = planSubscriptionCollection;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlanSubscription>>> GetAll()
    {
        var subscriptions = await _planSubscriptionCollection.GetAllAsync();
        return Ok(subscriptions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PlanSubscription>> GetById(Guid id)
    {
        var subscription = await _planSubscriptionCollection.GetByIdAsync(id);
        if (subscription == null)
        {
            return NotFound();
        }
        return Ok(subscription);
    }

    [HttpGet("{id}/status")]
    public async Task<ActionResult<string>> GetSubscriptionStatus(Guid id)
    {
        var subscription = await _planSubscriptionCollection.GetByIdAsync(id);
        if (subscription == null)
        {
            return NotFound();
        }

        string status = DetermineSubscriptionStatus(subscription);
        return Ok(new { status });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _planSubscriptionCollection.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }

    private string DetermineSubscriptionStatus(PlanSubscription subscription)
    {
        // Si el status es Cancelled, devolver Cancelled
        if (subscription.Status == SubscriptionStatus.Cancelled)
        {
            return "Cancelled";
        }

        // Si se ha creado pero no se ha pagado, devolver PendingPayment
        if (subscription.Status == SubscriptionStatus.PendingPayment)
        {
            return "PendingPayment";
        }

        DateTime now = DateTime.UtcNow;

        // Si la fecha actual está comprendida entre startdate y enddate, devolver active
        if (now >= subscription.StartDate && now <= subscription.EndDate)
        {
            return "Active";
        }

        // Si la fecha actual es posterior a enddate, devolver expired
        if (now > subscription.EndDate)
        {
            return "Expired";
        }

        return "PendingPayment";
    }
}
