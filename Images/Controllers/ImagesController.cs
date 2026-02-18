using Microsoft.AspNetCore.Mvc;
using Images.Collections;
using Images.Models;
using Images.Dto;
using Sieve.Models;
using Sieve.Services;
using Microsoft.EntityFrameworkCore;

namespace Images.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class ImagesController : ControllerBase
  {
    private readonly IImageCollection _imageCollection;
    private readonly HttpClient _httpClient;
    private readonly string _imgbbApiKey;
    private const string ImgBBUploadUrl = "https://api.imgbb.com/1/upload"; // Endpoint de ImgBB
    private readonly SieveProcessor sieveProcessor;


    public ImagesController(IImageCollection imageCollection, IConfiguration configuration, SieveProcessor _sieveProcessor)
    {
      _imageCollection = imageCollection;
      _httpClient = new HttpClient();
      _imgbbApiKey = configuration["ImgBB:ApiKey"] ?? Environment.GetEnvironmentVariable("IMGBB_API_KEY") ?? string.Empty;
      sieveProcessor = _sieveProcessor;
    }

    private async Task TryDeleteFromImgBB(string? deleteUrl)
    {
      if (string.IsNullOrWhiteSpace(deleteUrl))
        return;

      try
      {
        // ImgBB provides a delete URL that can be requested to remove the uploaded image
        var resp = await _httpClient.GetAsync(deleteUrl);
        // ignore response status - best-effort removal
      }
      catch
      {
        // ignore any errors when contacting ImgBB
      }
    }

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<Image>>> GetAll()
    {
      try
      {
        var images = await _imageCollection.GetAllAsync();
        return Ok(images);
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Image>> GetById(string id)
    {
      try
      {
        var image = await _imageCollection.GetImageById(id);
        if (image == null)
        {
          return NotFound(new { message = "Image not found" });
        }
        return Ok(image);
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }

    [HttpPost]
    public async Task<ActionResult<Image>> Create([FromBody] Image image)
    {
      try
      {
        if (image == null)
        {
          return BadRequest(new { message = "Image data is required" });
        }
        await _imageCollection.NewImage(image);
        return CreatedAtAction(nameof(GetById), new { id = image.Id }, image);
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] Image image)
    {
      try
      {
        if (image == null || image.Id != id)
        {
          return BadRequest(new { message = "Invalid image data" });
        }
        await _imageCollection.UpdateImage(image);
        return NoContent();
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }



    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
      try
      {
        var image = await _imageCollection.GetImageById(id);
        if (image == null)
        {
          return NotFound(new { message = "Image not found" });
        }
        if (!string.IsNullOrWhiteSpace(image.ImageDeleteUrl))
        {
          await TryDeleteFromImgBB(image.ImageDeleteUrl);
        }
        await _imageCollection.DeleteImage(image);
        return NoContent();
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }

    [HttpDelete("by-name/{imageName}")]
    public async Task<IActionResult> DeleteByName(string imageName)
    {
      try
      {
        if (string.IsNullOrWhiteSpace(imageName))
          return BadRequest(new { message = "imageName is required" });

        var image = await _imageCollection.GetImageByNameAsync(imageName);
        if (image == null)
        {
          return NotFound(new { message = "Image not found" });
        }
        if (!string.IsNullOrWhiteSpace(image.ImageDeleteUrl))
        {
          await TryDeleteFromImgBB(image.ImageDeleteUrl);
        }
        await _imageCollection.DeleteImage(image);
        return NoContent();
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }

    [HttpGet("query")] // return a collection
    public IActionResult GetQuery([FromQuery] SieveModel model)
    {
      var imagesResult = sieveProcessor.Apply(model, _imageCollection.GetPagedImages().AsNoTracking());
      return Ok(imagesResult);
    }

    [HttpGet("single-image")] // return single json if exists
    public IActionResult GetSingleQuery([FromQuery] SieveModel model)
    {
      var imageResult = sieveProcessor.Apply(model, _imageCollection.GetPagedImages().AsNoTracking()).Single();
      return Ok(imageResult);
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadImages([FromForm] ImageDto dto)
    {
      if (dto == null || dto.files == null || dto.files.Count == 0)
      {
        return BadRequest("No se han proporcionado archivos.");
      }

      if (string.IsNullOrWhiteSpace(_imgbbApiKey))
        return StatusCode(500, "ImgBB API key not configured.");

      var savedImages = new List<Image>();

      foreach (var file in dto.files)
      {
        if (file == null || file.Length == 0)
          continue;

        try
        {
          using var memoryStream = new MemoryStream();
          await file.CopyToAsync(memoryStream);
          var fileBytes = memoryStream.ToArray();

          using var formData = new MultipartFormDataContent();
          var base64 = Convert.ToBase64String(fileBytes);
          formData.Add(new StringContent(base64), "image");

          var response = await _httpClient.PostAsync($"{ImgBBUploadUrl}?key={_imgbbApiKey}", formData);
          // var dump = ObjectDumper.Dump(response);
          // Console.WriteLine(dump);
          if (!response.IsSuccessStatusCode)
          {
            return StatusCode((int)response.StatusCode, $"Error al subir la imagen: {response.ReasonPhrase}");
          }

          var responseContent = await response.Content.ReadAsStringAsync();
          var json = Newtonsoft.Json.Linq.JObject.Parse(responseContent);
          var data = json["data"];

          if (data == null)
            continue;

          //var imageId = data["id"]?.ToString() ?? Guid.NewGuid().ToString();
          var imageUrl = data["url"]?.ToString() ?? data["display_url"]?.ToString();
          var imageName = data["title"]?.ToString() ?? data["image"]?["filename"]?.ToString() ?? Path.GetFileName(file.FileName);
          var deleteUrl = data["delete_url"]?.ToString();

          var img = new Image
          {
            //Id = imageId,
            ImageUrl = imageUrl,
            ImageName = imageName,
            OwnerId = dto.OwnerId,
            OwnerType = dto.OwnerType,
            ImageDeleteUrl = deleteUrl
          };

          await _imageCollection.NewImage(img);
          savedImages.Add(img);
        }
        catch (Exception ex)
        {
          return StatusCode(500, $"Error interno: {ex.Message}");
        }
      }

      return Ok(new { images = savedImages });
    }
  }
}
