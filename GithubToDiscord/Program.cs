﻿using Octokit;
using Newtonsoft.Json;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace GithubToDiscord;
class Program
{
    static Config config;

    static async Task Main(string[] args)
    {
        // Deine bisherigen Initialisierungen
        InitAPIKeys();

        // Starte den Discord-Bot
        var discordBot = new DiscordBot(config.discordBotToken);
        await discordBot.StartAsync();
    }

    static void InitAPIKeys()
    {
        bool isValid = true;
        try
        {
            using (StreamReader reader = new StreamReader("config.json"))
            {
                string json = reader.ReadToEnd();
                config = JsonConvert.DeserializeObject<Config>(json);
            }
            if (string.IsNullOrEmpty(config.githubToken)) { isValid = false; Console.WriteLine("GitHub Token is Missing. Please insert it in config.json"); }
            if (string.IsNullOrEmpty(config.discordChannelID)) { isValid = false; Console.WriteLine("Discord Channel ID is Missing. Please insert it in config.json"); }
            if (string.IsNullOrEmpty(config.discordBotToken)) { isValid = false; Console.WriteLine("Discord Bot Token is Missing. Please insert it in config.json"); }
        }
        catch (Exception ex)
        {
            isValid = false;
            Console.WriteLine(ex);
        }

        if (!isValid) { Environment.Exit(1); }
    }
}
