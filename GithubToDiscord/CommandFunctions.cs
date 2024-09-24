using Discord.WebSocket;
using Discord.Commands;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace GithubToDiscord
{
    internal class CommandFunctions
    {
        public static async Task getReposAsChannels(string token, string name, SocketGuild guild) 
        {
            var gitHubClient = new GitHubClient(new ProductHeaderValue("GithubToDiscord"));
            gitHubClient.Credentials = new Credentials(token);

            try
            {
                // Repositories 
                var repositories = name == null
                    ? await gitHubClient.Repository.GetAllForCurrent()
                    : await gitHubClient.Repository.GetAllForOrg(name);

                foreach (var repo in repositories)
                {
                    var exist = guild.Channels.OfType<SocketCategoryChannel>().FirstOrDefault(c => c.Name == $"{repo.Name}");
                    if (exist == null) { //if not exist

                        var isPrivateRepo = repo.Private;
                        var categoryPermissions = new OverwritePermissions(
                            viewChannel: isPrivateRepo ? PermValue.Deny : PermValue.Allow,
                            sendMessages: isPrivateRepo ? PermValue.Deny : PermValue.Allow
                        );

                        //category
                        var category = await guild.CreateCategoryChannelAsync(repo.Name, properties =>
                        {
                            properties.PermissionOverwrites = new List<Overwrite>
                            {
                                new Overwrite(guild.EveryoneRole.Id, PermissionTarget.Role, categoryPermissions)
                            };
                        });

                        //channels
                        var repoChannel = await guild.CreateTextChannelAsync($"{repo.Name} Repository", properties => properties.CategoryId = category.Id);
                        var pullRequestChannel = await guild.CreateTextChannelAsync($"{repo.Name} Pull Requests", properties => properties.CategoryId = category.Id);
                        var BugReportsChannel = await guild.CreateTextChannelAsync($"{repo.Name} Bug Reports", properties => properties.CategoryId = category.Id);
                        var UpdateInfoChannel = await guild.CreateTextChannelAsync($"{repo.Name} Update Infos", properties => properties.CategoryId = category.Id);

                        var repositoryUrl = repo.HtmlUrl.ToString();
                        await repoChannel.SendMessageAsync($"Repository URL: {repositoryUrl}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public static async Task removeReposFromChannel(string token, string name, SocketGuild guild)
        {
            var gitHubClient = new GitHubClient(new ProductHeaderValue("GithubToDiscord"));
            gitHubClient.Credentials = new Credentials(token);

            try
            {
                var repositories = name == null
                    ? await gitHubClient.Repository.GetAllForCurrent()
                    : await gitHubClient.Repository.GetAllForOrg(name);

                foreach (var repo in repositories)
                {
                    var category = guild.Channels.OfType<SocketCategoryChannel>().FirstOrDefault(c => c.Name == $"{repo.Name}");
                    if (category != null)
                    {
                        foreach (var channel in category.Channels.ToList())
                        {
                            await channel.DeleteAsync();
                        }
                        await category.DeleteAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
