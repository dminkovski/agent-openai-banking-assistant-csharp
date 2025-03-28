
[Route("api/content")]
[ApiController]
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
            byte[] imageBytes = await _blobStorage.GetFileAsBytesAsync(fileName);

            // Determine the content type based on the file extension
            string contentType = GetContentType(fileName);

            return File(imageBytes, contentType);
        }
        catch (Exception ex)
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

        try
        {
            using (var blobStream = file.OpenReadStream())
            {
                await _blobStorage.StoreFileAsync(fileName, blobStream);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file to blob storage.");
            return StatusCode(500, "Error uploading file to blob storage.");
        }

        return Ok(fileName);
    }

    private string GetContentType(string fileName)
    {
        // You can expand this method to handle more file types
        string extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };
    }

}
