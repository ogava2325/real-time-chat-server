namespace Chat.Models;

/// <summary>
/// Represents the configuration settings required for Azure Text Analytics service.
/// This class is typically used to bind settings from a configuration file, such as appsettings.json.
/// </summary>
public class AzureTextAnalyticsSettings
{
    /// <summary>
    /// Gets or sets the endpoint URL for the Azure Text Analytics service.
    /// </summary>
    public string Endpoint { get; set; }

    /// <summary>
    /// Gets or sets the API key used to authenticate with the Azure Text Analytics service.
    /// </summary>
    public string ApiKey { get; set; }
}