using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GithubToDiscord
{
    internal class CommandsModule : ModuleBase<SocketCommandContext>
    {
        private readonly Config tokenConfig;
        public CommandsModule()
        {
            using (StreamReader reader = new StreamReader("config.json"))
            {
                string json = reader.ReadToEnd();
                tokenConfig = JsonConvert.DeserializeObject<Config>(json);
            }
        }

        [Command("initRepos")]
        public async Task InitReposAsync()
        {
            await ReplyAsync("Creating repo channels...");
            await CommandFunctions.getReposAsChannels(tokenConfig.githubToken, tokenConfig.githubOrgName, (SocketGuild)Context.Guild);
            await ReplyAsync("Done!");
        }

        [Command("removeRepos")]
        public async Task removeReposAsync()
        {
            await ReplyAsync("Remove repos channels...");
            await CommandFunctions.removeReposFromChannel(tokenConfig.githubToken, tokenConfig.githubOrgName, (SocketGuild)Context.Guild);
            await ReplyAsync("Done!");
        }
    }
}
