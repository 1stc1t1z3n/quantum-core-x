using System.ComponentModel.DataAnnotations;
using CommandLine;
using QuantumCore.API.Game;
using QuantumCore.API.Game.Guild;
using QuantumCore.Game.Extensions;

namespace QuantumCore.Game.Commands.Guild;

[Command("deleteguild", "Delete guild")]
public class GuildDeleteCommand : ICommandHandler<GuildDeleteCommandOptions>
{
    private readonly IGuildManager _guildManager;

    public GuildDeleteCommand(IGuildManager guildManager)
    {
        _guildManager = guildManager;
    }

    public async Task ExecuteAsync(CommandContext<GuildDeleteCommandOptions> context)
    {
        var guild = await _guildManager.GetGuildByNameAsync(context.Arguments.GuildName);
        if (guild is null)
        {
            context.Player.SendChatInfo("Guild not found");
            return;
        }

        var onlineMembers = context.Player.Map?.World
            .GetGuildMembers(guild.Id)
            .ToList();

        await _guildManager.RemoveGuildAsync(guild.Id);

        if (onlineMembers is not null)
            foreach (var member in onlineMembers)
                await member.RefreshGuildAsync();

        await context.Player.RefreshGuildAsync();
    }
}

public class GuildDeleteCommandOptions
{
    [Value(0)] [Required] public string GuildName { get; set; } = "";
}
