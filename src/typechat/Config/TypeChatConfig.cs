namespace Microsoft.TypeChat.Config;

public abstract class TypeChatConfig
{
    /// <summary>
    /// Api endpoint
    /// </summary>
    public string Endpoint { get; set; }

    /// <summary>
    /// Model name
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// Http Settings
    /// </summary>
    public int TimeoutMs { get; set; } = 15 * 1000;

    public int MaxRetries { get; set; } = 3;

    public int MaxPauseMs { get; set; } = 1000; // 1000 milliseconds

    /// <summary>
    /// Validate the configuration
    /// </summary>
    /// <param name="configFileName">(optional) Config file the settings came from</param>
    public abstract void Validate(string configFileName = default);
}
