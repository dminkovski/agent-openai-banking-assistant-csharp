using Microsoft.AspNetCore.Authorization;

[Route("api/content")]
[ApiController]
[Authorize]
public class ContentController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly IBlobStorage _blobStorage;
    private readonly ILogger<ContentController> _logger;

    public ContentController(IWebHostEnvironment environment, IBlobStorage blobStorage, ILogger<ContentController> logger)
    {
        _environment = environment;
        _blobStorage = blobStorage;
        _logger = logger;
    }

    [HttpGet("{fileName}")]
    public async Task<IActionResult> Index(string fileName)
    {
        if (fileName == null || fileName.Length == 0)
        {
            _logger.LogWarning("No file name provided.");
            return BadRequest("No file name provided.");
        }
        try
        {
            var fileString = await _blobStorage.GetFileAsString(fileName);
            return Ok(fileString);
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error fetching file from blob storage.");
            return StatusCode(500, "Error fetching file from blob storage.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> UploadContent([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            _logger.LogWarning("File is missing.");
            return BadRequest("File is missing.");
        }
        var fileName = Path.GetFileName(file.FileName);

        // Save the file locally
        /*var uploadsFolder = Path.Combine(_environment.ContentRootPath, "Downloads");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var localFilePath = Path.Combine(uploadsFolder, fileName);

        try
        {
            using (var localStream = new FileStream(localFilePath, FileMode.Create))
            {
                await file.CopyToAsync(localStream);
            }
        }
        catch (IOException ex)
        {
            _logger.LogError(ex, "Error saving file locally.");
            return StatusCode(500, "Error saving file locally.");
        }
        */

        // Upload the file to Blob Storage
        try
        {
            // Use a fresh stream to avoid issues after file.CopyToAsync.
            using (var blobStream = file.OpenReadStream())
            {
                await _blobStorage.StoreFile(fileName, blobStream);
            }
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error uploading file to blob storage.");
            return StatusCode(500, "Error uploading file to blob storage.");
        }

        return Ok(fileName);
    }

}
