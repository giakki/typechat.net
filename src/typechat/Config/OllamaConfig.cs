// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.TypeChat.Config;

/// <summary>
/// Ollama configuration
/// Can be initialized either from environment variables or from config files
/// </summary>
public class OllamaConfig : TypeChatConfig
{
    /// <summary>
    /// Names of environment variables
    /// </summary>
    public static class VariableNames
    {
        /// <summary>
        /// The Ollama endpoint, such as:
        /// http://localhost:11434/
        /// </summary>
        public const string OLLAMA_ENDPOINT = "OLLAMA_ENDPOINT";

        /// <summary>
        /// Name of the language model to use
        /// </summary>
        public const string OLLAMA_MODEL = "OLLAMA_MODEL";

        /// <summary>
        /// Name of the embedding model to use
        /// </summary>
        public const string OLLAMA_EMBEDDINGMODEL = "OLLAMA_EMBEDDINGMODEL";
    }

    public OllamaConfig() { }

    /// <summary>
    /// Validate the configuration
    /// </summary>
    /// <param name="configFileName">(optional) Config file the settings came from</param>
    public override void Validate(string configFileName = default)
    {
        configFileName ??= string.Empty;

        Verify(Endpoint, nameof(Endpoint), configFileName);
        Verify(Model, nameof(Model), configFileName);
    }

    void Verify(string value, string name, string fileName)
    {
        if (string.IsNullOrEmpty(value) || value == "?")
        {
            throw new ArgumentException($"OllamaConfig: {name} is not initialized in {fileName}");
        }
    }

    /// <summary>
    /// Load configuration from environment variables
    /// </summary>
    /// <param name="isEmbedding">Is this an embedding model?</param>
    /// <returns></returns>
    public static OllamaConfig FromEnvironment(bool isEmbedding = false)
    {
        OllamaConfig config = new OllamaConfig();
        config.Endpoint = Environment.GetEnvironmentVariable(VariableNames.OLLAMA_ENDPOINT);
        if (isEmbedding)
        {
            config.Model = Environment.GetEnvironmentVariable(VariableNames.OLLAMA_EMBEDDINGMODEL);
        }
        else
        {
            config.Model = Environment.GetEnvironmentVariable(VariableNames.OLLAMA_MODEL);
        }
        config.Validate();
        return config;
    }

    /// <summary>
    /// Load configuration from a json file. A trivial wrapper around the Json serializer
    /// </summary>
    /// <param name="jsonFilePath">json text</param>
    /// <returns>config object</returns>
    public static OllamaConfig LoadFromJsonFile(string jsonFilePath)
    {
        string json = File.ReadAllText(jsonFilePath);
        if (string.IsNullOrEmpty(json))
        {
            throw new ArgumentException($"{jsonFilePath} is empty");
        }
        return Json.Parse<OllamaConfig>(json);
    }
}
