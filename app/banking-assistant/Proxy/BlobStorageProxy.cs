public class BlobStorageProxy : IBlobStorage
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName = "";
    private ILogger<BlobStorageProxy> _logger;

    public BlobStorageProxy(BlobServiceClient blobServiceClient, ILogger<BlobStorageProxy> logger, IConfiguration configuration)
    {
        _logger = logger;
        _blobServiceClient = blobServiceClient;
        _containerName = configuration[key: "Storage:ContainerName"] ?? throw new ArgumentNullException(nameof(configuration), "Storage container configuration is missing.");
    }
    public async Task<byte[]> GetFileAsBytesAsync(string fileName)
    {
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        BlobClient blobClient = containerClient.GetBlobClient(fileName);
        BlobDownloadResult downloadResult = await blobClient.DownloadContentAsync();
        return downloadResult.Content.ToArray();
    }

    public Uri GetFileUri(string fileName)
    {
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        BlobClient blobClient = containerClient.GetBlobClient(fileName);
        return blobClient.Uri;
    }
    public async Task StoreFileAsync(string fileName, Stream content)
    {
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

        // Create the container if it doesn't exist.
        await containerClient.CreateIfNotExistsAsync();

        try
        {
            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            blobClient.Upload(content, overwrite: true);
        }catch(Exception e)
        {
            _logger.LogError($"Storing a file failed: {e.Message}");
        }
    }
}

