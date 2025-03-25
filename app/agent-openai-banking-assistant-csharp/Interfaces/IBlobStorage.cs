namespace agent_openai_banking_assistant_csharp.Interfaces
{
    public interface IBlobStorage
    {
        public Task<string> GetFileAsString(string fileName);
        public Task StoreFile(string fileName, Stream content);

        public Uri GetFileUri(string fileName);
    }
}
