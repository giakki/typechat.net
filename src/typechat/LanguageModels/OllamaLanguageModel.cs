// Copyright (c) Microsoft. All rights reserved.

using Microsoft.TypeChat.Config;
using OllamaSharp;

namespace Microsoft.TypeChat.LanguageModels;

/// <summary>
/// A lightweight ILanguageModel implementation over Ollama or Azure Ollama Chat Completion REST API endpoint
/// </summary>
public class OllamaLanguageModel : ILanguageModel
{
    static TranslationSettings s_defaultSettings = new TranslationSettings();

    OllamaConfig _config;
    ModelInfo _model;
    private readonly OllamaApiClient _ollama;

    /// <summary>
    /// Create an OllamaLanguageModel object using the given OllamaConfig
    /// config.EndPoint must support the Chat Completion API
    /// </summary>
    /// <param name="config">configuration to use</param>
    /// <param name="model">information about the target model</param>
    /// <param name="client">http client to use</param>
    public OllamaLanguageModel(OllamaConfig config, ModelInfo? model = null, HttpClient? client = null)
    {
        ArgumentVerify.ThrowIfNull(config, nameof(config));
        config.Validate();

        _config = config;
        _model = model ?? _config.Model;
        _ollama = new OllamaApiClient(new Uri(config.Endpoint));
    }

    /// <summary>
    /// Information about the language model
    /// </summary>
    public ModelInfo ModelInfo => _model;

    /// <summary>
    /// Get a completion for the given prompt
    /// </summary>
    /// <param name="prompt">prompt</param>
    /// <param name="settings">translation settings such as temperature</param>
    /// <param name="cancelToken">cancellation token</param>
    /// <returns></returns>
    public async Task<string> CompleteAsync(Prompt prompt, TranslationSettings? settings = null, CancellationToken cancelToken = default)
    {
        ArgumentVerify.ThrowIfNullOrEmpty<IPromptSection>(prompt, nameof(prompt));

        var conversation = await _ollama.GetCompletion(new()
        {
            Model = _model.Name,
            Options = new()
            {
                Temperature = (float)settings.Temperature,
            },
            Prompt = prompt.ToString(),
            Stream = false,
        });

        return conversation.Response;
    }
}
