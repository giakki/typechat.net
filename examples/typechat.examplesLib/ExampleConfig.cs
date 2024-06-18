// Copyright (c) Microsoft. All rights reserved.

using Microsoft.TypeChat.Config;

namespace Microsoft.TypeChat.Examples;

/// <summary>
/// Simplifies loading configuration for examples from settings files
/// </summary>
public class ExampleConfig
{
    public const string DefaultConfigFile = "appSettings.json";
    public const string DefaultConfigFile_Dev = "appSettings.Development.json";

    public static class ModelNames
    {
        public const string Gpt35Turbo = "gpt-35-turbo";
        public const string Gpt4 = "gpt-4";
        public const string Ad002 = "ada-002";
    }

    public static T LoadConfig<T>(string configFile, string configOverloadFile, string sectionName = default)
        where T : new()
    {
        var configBuilder = new ConfigurationBuilder()
                    .AddJsonFile(configFile, false, true);

        if (File.Exists(configOverloadFile))
        {
            configBuilder.AddJsonFile(configOverloadFile, false, true);
        }
        var config = configBuilder.Build();
        var configSection = config.GetSection(sectionName);
        if (configSection is null)
        {
            throw new ArgumentException($"{sectionName} not found");
        }

        T settings = new T();
        configSection.Bind(settings);
        return settings;
    }

    public static OpenAIConfig LoadOpenAI(string? sectionName = null)
    {
        sectionName ??= "OpenAI";
        OpenAIConfig config = LoadConfig<OpenAIConfig>(DefaultConfigFile, DefaultConfigFile_Dev, sectionName);

        return config;
    }

    public static OllamaConfig LoadOllama(string? sectionName = null)
    {
        sectionName ??= "Ollama";
        OllamaConfig config = LoadConfig<OllamaConfig>(DefaultConfigFile, DefaultConfigFile_Dev, sectionName);

        return config;
    }

    OpenAIConfig? _openAI;
    OpenAIConfig? _openAIEmbeddings;
    OllamaConfig? _ollama;
    OllamaConfig? _ollamaEmbeddings;

    public ExampleConfig()
    {
        if (File.Exists(DefaultConfigFile) && File.Exists(DefaultConfigFile_Dev))
        {
            _openAI = LoadOpenAI();
            _openAIEmbeddings = LoadOpenAI("OpenAI_Embeddings");
            _ollama = LoadOllama();
            _ollamaEmbeddings = LoadOllama("OpenAI_Embeddings");
        }
        else
        {
            _openAI = OpenAIConfig.FromEnvironment();
            _openAIEmbeddings = OpenAIConfig.FromEnvironment(isEmbedding: true);
            _ollama = OllamaConfig.FromEnvironment();
            _ollamaEmbeddings = OllamaConfig.FromEnvironment(isEmbedding: true);
        }
    }

    /// <summary>
    /// Configuration for OpenAI language models
    /// </summary>
    public OpenAIConfig OpenAI => _openAI;
    public OllamaConfig Ollama => _ollama;

    /// <summary>
    /// Configuration for OpenAI embeddings models
    /// </summary>
    public OpenAIConfig? OpenAIEmbeddings => _openAIEmbeddings;
    public OllamaConfig? OllamaEmbeddings => _ollamaEmbeddings;

    public bool HasOpenAI => (_openAI is not null);
    public bool HasOllama => (_ollama is not null);

    public bool HasOpenAIEmbeddings => (_openAIEmbeddings is not null);
    public bool HasOllamaEmbeddings => (_ollamaEmbeddings is not null);
}
