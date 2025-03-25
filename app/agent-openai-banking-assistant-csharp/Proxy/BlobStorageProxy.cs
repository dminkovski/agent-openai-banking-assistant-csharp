public class BlobStorageProxy : IBlobStorage
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly String _containerName = "";

    public BlobStorageProxy(BlobServiceClient blobServiceClient, IConfiguration configuration)
    {
        _blobServiceClient = blobServiceClient;
        _containerName = configuration[key: "Storage:ContainerName"] ?? throw new ArgumentNullException(nameof(configuration), "Storage container configuration is missing.");
    }
    public async Task<string> GetFileAsString(string fileName)
    {
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        BlobClient blobClient = containerClient.GetBlobClient(fileName);
        BlobDownloadResult downloadResult = await blobClient.DownloadContentAsync();
        string blobContents = downloadResult.Content.ToString();
        return blobContents;
    }

    public Uri GetFileUri(string fileName)
    {
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        BlobClient blobClient = containerClient.GetBlobClient(fileName);
        return blobClient.Uri;
    }
    public async Task StoreFile(String fileName, Stream content)
    {
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

        // Create the container if it doesn't exist.
        await containerClient.CreateIfNotExistsAsync();

        BlobClient blobClient = containerClient.GetBlobClient(fileName);
        await blobClient.UploadAsync(content, overwrite: true);
    }
}

