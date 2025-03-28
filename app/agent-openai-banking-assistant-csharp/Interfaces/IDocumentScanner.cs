namespace agent_openai_banking_assistant_csharp.Interfaces
{
    public interface IDocumentScanner
    {
        public Task<Dictionary<string, string>> Scan(string fileName);
    }
}
