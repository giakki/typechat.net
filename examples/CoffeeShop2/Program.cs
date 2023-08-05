﻿// Copyright (c) Microsoft. All rights reserved.
using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.SemanticKernel;
using Microsoft.TypeChat;
using Microsoft.TypeChat.Schema;
using Microsoft.TypeChat.SemanticKernel;

namespace CoffeeShop;

public class CoffeeShop : ConsoleApp
{
    IVocabCollection _vocabs;
    IKernel _kernel;
    JsonTranslator<Cart> _translator;

    CoffeeShop()
    {
        var config = Config.LoadOpenAI();
        _kernel = KernelFactory.CreateKernel(config);

        // Load a standard vocabulary from file.
        // But you can also use a different vocab for each request.
        _vocabs = CoffeeShopVocabs.Load();
        // Here we crete a single translator and hold on to it.
        // But you can create instances of the translator on demand, one for each request. 
        // Each with a different vocab specific to the request
        // E.g. you could service a different vocab to a Vegan user. Or show more options to a Premimum user
        _translator = _kernel.JsonTranslator<Cart>(config.Model, _vocabs);
        _translator.MaxRepairAttempts = 3;
        //base.SubscribeAllEvents(_translator);
    }

    public TypeSchema Schema => _translator.Validator.Schema;

    protected override async Task ProcessRequestAsync(string input, CancellationToken cancelToken)
    {
        Cart cart = await _translator.TranslateAsync(input);

        string json = Json.Stringify(cart);
        Console.WriteLine(json);

        if (!PrintAnyUnknown(cart))
        {
            Console.WriteLine("Success!");
        }
    }

    bool PrintAnyUnknown(Cart cart)
    {
        if (cart.Items != null)
        {
            StringBuilder sb = new StringBuilder();
            cart.GetUnknown(sb);
            if (sb.Length > 0)
            {
                Console.WriteLine("I didn't understand the following:");
                Console.WriteLine(sb.ToString());
                return true;
            }
        }
        return false;
    }

    public static async Task<int> Main(string[] args)
    {
        try
        {
            CoffeeShop app = new CoffeeShop();
            // Un-comment to print auto-generated schema at start:
            //Console.WriteLine(app.Schema.Schema.Text);
            await app.RunAsync("☕> ", args.GetOrNull(0));
        }
        catch (Exception ex)
        {
            WriteError(ex);
        }

        return 0;
    }
}