using Images.Dto;
using Images.Services;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;

namespace Images.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ImagesController(IImageManager images, ILogger<ImagesController> logger) : ControllerBase
{
    [HttpGet("all")]
    public async Task<ActionResult<IReadOnlyList<ImageResponse>>> GetAll(CancellationToken ct) =>
        Ok(await images.QueryAsync(new SieveModel(), ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ImageResponse>> GetById(Guid id, CancellationToken ct)
    {
        var image = await images.GetAsync(id, ct);
        return image == null ? NotFound() : Ok(image);
    }

    [HttpGet("query")]
    public async Task<ActionResult<IReadOnlyList<ImageResponse>>> GetQuery([FromQuery] SieveModel model, CancellationToken ct) =>
        Ok(await images.QueryAsync(model, ct));

    [HttpGet("single-image")]
    public async Task<ActionResult<ImageResponse>> GetSingleQuery([FromQuery] SieveModel model, CancellationToken ct)
    {
        var result = await images.QueryAsync(model, ct);
        return result.Count == 0 ? NotFound() : Ok(result[0]);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ImageUpdateRequest request, CancellationToken ct) =>
        await images.UpdateAsync(id, request, ct) ? NoContent() : NotFound();

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct) =>
        await images.DeleteAsync(id, ct) ? NoContent() : NotFound();

    [HttpDelete("by-name/{imageName}")]
    public async Task<IActionResult> DeleteByName(string imageName, CancellationToken ct) =>
        await images.DeleteByNameAsync(imageName, ct) ? NoContent() : NotFound();

    [HttpPost("upload")]
    [RequestSizeLimit(134_217_728)]
    public async Task<IActionResult> UploadImages([FromForm] ImageDto request, CancellationToken ct)
    {
        if (request.files == null || request.files.Count == 0
            || request.files.Any(file => file == null || file.Length <= 0 || file.Length > 32L * 1024 * 1024))
            return BadRequest(new { message = "Seleccione archivos de imagen de hasta 32 MB." });
        if (string.IsNullOrWhiteSpace(request.OwnerId) || string.IsNullOrWhiteSpace(request.OwnerType))
            return BadRequest(new { message = "Se requieren ownerId y ownerType." });

        var saved = new List<ImageResponse>();
        try
        {
            foreach (var file in request.files)
                saved.Add(await images.UploadAsync(file, request.OwnerId, request.OwnerType, ct));
            return Ok(saved);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested) { throw; }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message, images = saved });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error subiendo imagenes; {Count} imagenes guardadas", saved.Count);
            return StatusCode(500, new { message = "No se pudieron guardar todas las imagenes.", images = saved });
        }
    }
}
