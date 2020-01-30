using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace DiscordBot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("debug")]
        public async Task DebugCommand()
        {
            await ReplyAsync("Your User ID is " + Context.User.Id);
        }

    }
}
