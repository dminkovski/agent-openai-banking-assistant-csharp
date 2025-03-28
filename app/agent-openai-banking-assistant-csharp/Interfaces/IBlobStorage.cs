namespace agent_openai_banking_assistant_csharp.Interfaces
{
    public interface IBlobStorage
    {
        public Task<byte[]> GetFileAsBytesAsync(string fileName);
        public Task StoreFileAsync(string fileName, Stream content);

        public Uri GetFileUri(string fileName);
    }
}
