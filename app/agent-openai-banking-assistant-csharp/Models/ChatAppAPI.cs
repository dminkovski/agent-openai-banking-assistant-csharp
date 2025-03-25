using Microsoft.SemanticKernel.ChatCompletion;
using System.Text.Json.Serialization;

public record ResponseMessage( string Content, string Role, List<string>? Attachments);

public record ChatAppRequestOverrides(
    bool? SemanticRanker,
    bool? SemanticCaptions,
    string? ExcludeCategory,
    int? Top,
    float? Temperature,
    string? PromptTemplate,
    string? PromptTemplatePrefix,
    string? PromptTemplateSuffix,
    bool? SuggestFollowupQuestions,
    bool? UseOidSecurityFilter,
    bool? UseGroupsSecurityFilter,
    string? SemanticKernelMode);
public record ChatAppRequestContext
{
    public ChatAppRequestOverrides? Overrides { get; init; }
    public ChatAppRequestContext() { }

    [JsonConstructor]
    public ChatAppRequestContext(ChatAppRequestOverrides? overrides)
    {
        Overrides = overrides;
    }
}

public record ChatAppRequest(List<ResponseMessage> Messages, bool Stream, ChatAppRequestContext Context, List<string>? Attachments, string Approach);

public record ResponseChoice(int Index, ResponseMessage Message, ResponseContext Context, ResponseMessage Delta);

public record ResponseContext(string Thoughts, List<string> DataPoints);

public class ChatResponse
{
    public List<ResponseChoice> Choices { get; set; }

    public ChatResponse(List<ResponseChoice> Choices)
    {
        this.Choices = Choices;
    }

    public static ChatResponse BuildChatResponse(ChatHistory chatHistory, AgentContext agentContext)
    {
        var dataPoints = new List<string>();
        string thoughts = string.Empty;
        var attachments = new List<string>();

        if (agentContext.ContainsKey("dataPoints") && agentContext["dataPoints"] != null)
            dataPoints.AddRange((List<string>)agentContext["dataPoints"]);
        if (agentContext.ContainsKey("thoughts") && agentContext["thoughts"] != null)
            thoughts = (string)agentContext["thoughts"];
        if (agentContext.ContainsKey("attachments") && agentContext["attachments"] != null)
            attachments.AddRange((List<string>)agentContext["attachments"]);

        return new ChatResponse(new List<ResponseChoice>
            {
                new ResponseChoice(
                    0,
                    new ResponseMessage(
                        chatHistory[chatHistory.Count-1].Content,
                        "assistant",
                        attachments),
                    new ResponseContext(thoughts, dataPoints),
                    new ResponseMessage(
                        chatHistory[chatHistory.Count-1].Content,
                        "assistant",
                        attachments))
            });
    }
}