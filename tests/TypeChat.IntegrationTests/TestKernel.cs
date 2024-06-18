// Copyright (c) Microsoft. All rights reserved.

using Microsoft.TypeChat.Examples;

namespace Microsoft.TypeChat.Tests;

public class TestKernel : TypeChatTest, IClassFixture<ExampleConfig>
{
    ExampleConfig _config;

    public TestKernel(ITestOutputHelper output, ExampleConfig config)
        : base(output)
    {
        _config = config;
    }

    [SkippableFact]
    public void TestKernelBuild()
    {
        Skip.If(!CanRunEndToEndTest(_config));

        IKernelBuilder kb = Kernel.CreateBuilder();
        kb.WithChatModels(_config.OpenAI, ExampleConfig.ModelNames.Gpt35Turbo, ExampleConfig.ModelNames.Gpt4);
        Kernel kernel = kb.Build();

        var model = kernel.TextCompletionModel(ExampleConfig.ModelNames.Gpt35Turbo);
        Assert.NotNull(model);
        Assert.Equal(model.ModelInfo.Name, ExampleConfig.ModelNames.Gpt35Turbo);

        var languageModel = kernel.ChatLanguageModel(ExampleConfig.ModelNames.Gpt4);
        Assert.NotNull(languageModel);
        Assert.Equal(languageModel.ModelInfo.Name, ExampleConfig.ModelNames.Gpt4);
    }

    [SkippableFact]
    public async Task TestLanguageModel()
    {
        Skip.If(!CanRunEndToEndTest(_config));

        TextCompletionModel lm = new TextCompletionModel(_config.OpenAI);
        string response = await lm.CompleteAsync("Is Venus a planet?");
        Assert.NotNull(response);
        Assert.NotEmpty(response);
    }

    [SkippableFact]
    public async Task TestChatModel()
    {
        Skip.If(!CanRunEndToEndTest(_config));

        ChatLanguageModel cm = new ChatLanguageModel(_config.OpenAI);
        Prompt prompt = "Is Venus a planet?";
        Assert.Equal(prompt.Last().Source, PromptSection.Sources.User);

        string response = await cm.CompleteAsync(prompt);
        Validate(response, "Yes");
        prompt.AppendResponse(response);

        prompt.Append("What about Pluto?");
        response = await cm.CompleteAsync(prompt);
        Validate(response, "No");
        prompt.Append(response);

        Assert.Equal(prompt.Count, 4);
    }

    void Validate(Message message, string? contents = null)
    {
        Assert.NotNull(message.Body);
        if (!string.IsNullOrEmpty(contents))
        {
            Assert.True(message.GetText().Contains(contents, StringComparison.OrdinalIgnoreCase));
        }
    }
}
