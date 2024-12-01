using Azure;
using Azure.AI.TextAnalytics;
using Chat.Interfaces;
using Chat.Models;
using Microsoft.Extensions.Options;

namespace Chat.Services;

public class TextAnalyticsService : ITextAnalyticsService
{
    private readonly TextAnalyticsClient _client;

    public TextAnalyticsService(IOptions<AzureTextAnalyticsSettings> settings)
    {
        // Initialize AzureKeyCredential with the API key from the configuration
        var credentials = new AzureKeyCredential(settings.Value.ApiKey);

        // Create a new instance of TextAnalyticsClient with the endpoint and credentials
        _client = new TextAnalyticsClient(new Uri(settings.Value.Endpoint), credentials);
    }

    public async Task<string> AnalyzeSentimentAsync(string message)
    {
        // Analyze the sentiment of the message
        var result = await _client.AnalyzeSentimentAsync(message);
        return result.Value.Sentiment.ToString();
    }
}