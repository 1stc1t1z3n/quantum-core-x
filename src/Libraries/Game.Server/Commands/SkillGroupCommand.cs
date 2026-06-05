using CommandLine;
using EnumsNET;
using QuantumCore.API.Game;
using QuantumCore.API.Game.Types.Entities;
using QuantumCore.API.Game.Types.Skills;
using QuantumCore.Game.Skills;

namespace QuantumCore.Game.Commands;

[Command("skillgroup", "Choose your skill path (1 or 2). One-time choice at level 5+.")]
[CommandNoPermission]
public class SkillGroupCommand : ICommandHandler<SkillGroupCommandOptions>
{
    public Task ExecuteAsync(CommandContext<SkillGroupCommandOptions> context)
    {
        var player = context.Player;

        if (player.GetPoint(EPoint.LEVEL) < PlayerSkills.MINIMUM_LEVEL)
        {
            player.SendChatInfo($"You must reach level {PlayerSkills.MINIMUM_LEVEL} to choose a skill path.");
            return Task.CompletedTask;
        }

        if ((byte)player.Player.SkillGroup != 0)
        {
            player.SendChatInfo("You have already chosen your skill path. It cannot be changed.");
            return Task.CompletedTask;
        }

        if (!Enums.TryToObject<ESkillGroup>(context.Arguments.Group, out var group, EnumValidation.IsDefined))
        {
            player.SendChatInfo("Invalid group. Use /skillgroup 1 or /skillgroup 2.");
            return Task.CompletedTask;
        }

        player.Skills.SetSkillGroup(group);
        player.SendChatInfo("Skill path chosen! Open the Skill window (K) and start leveling your skills.");

        return Task.CompletedTask;
    }
}

public class SkillGroupCommandOptions
{
    [Value(0, Required = true)] public byte Group { get; set; } = 0;
}
