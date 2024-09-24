using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GithubToDiscord
{
    internal class Config
    {
        public string githubToken { get; set; }
        public string discordBotToken { get; set; }
        public string discordChannelID { get; set; }
        public string githubOrgName { get; set; }
    }
}
