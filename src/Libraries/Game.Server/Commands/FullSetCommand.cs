using CommandLine;
using QuantumCore.API.Core.Models;
using QuantumCore.API.Extensions;
using QuantumCore.API.Game;
using QuantumCore.API.Game.Types.Players;
using QuantumCore.API.Game.World;
using QuantumCore.Game.Extensions;
using QuantumCore.Game.Persistence;

namespace QuantumCore.Game.Commands;

[Command("full_set", "Gives a full set of equipment for the target's class +9")]
public class FullSetCommand : ICommandHandler<FullSetCommandOptions>
{
    private readonly IItemRepository _itemRepository;
    private readonly IWorld _world;

    private static readonly uint HairBaseId = 73001;

    private static readonly uint[] SharedItems = [13069, 17209, 14209, 15229, 16209];
    private static readonly uint[] WarriorItems = [12249, 189, 3169, 11299, HairBaseId];
    private static readonly uint[] NinjaItems = [12389, 189, 1139, 2189, 11499, HairBaseId + 250];
    private static readonly uint[] SuraItems = [12529, 189, 11699, HairBaseId + 500];
    private static readonly uint[] ShamanItems = [12669, 5129, 11899, HairBaseId + 750];

    public FullSetCommand(IItemRepository itemRepository, IWorld world)
    {
        _itemRepository = itemRepository;
        _world = world;
    }

    public async Task ExecuteAsync(CommandContext<FullSetCommandOptions> context)
    {
        var target = string.Equals(context.Arguments.Target, "$self", StringComparison.InvariantCultureIgnoreCase)
            ? context.Player
            : _world.GetPlayer(context.Arguments.Target);

        if (target is null)
        {
            context.Player.SendChatInfo("Target not found");
            return;
        }

        foreach (var item in SharedItems)
        {
            var instance = new ItemInstance {ItemId = item, Count = 1, PlayerId = target.Player.Id};
            if (!await target.Inventory.PlaceItem(instance))
            {
                context.Player.SendChatInfo("No place in inventory");
                return;
            }

            await instance.Persist(_itemRepository);
            target.SendItem(instance);
        }

        var jobItems = target.Player.PlayerClass.GetClass() switch
        {
            EPlayerClass.WARRIOR => WarriorItems,
            EPlayerClass.NINJA => NinjaItems,
            EPlayerClass.SURA => SuraItems,
            EPlayerClass.SHAMAN => ShamanItems,
            _ => throw new ArgumentOutOfRangeException(nameof(context),
                $"No default items for player job {target.Player.PlayerClass}")
        };
        foreach (var item in jobItems)
        {
            var instance = new ItemInstance {ItemId = item, Count = 1, PlayerId = target.Player.Id};
            if (!await target.Inventory.PlaceItem(instance))
            {
                context.Player.SendChatInfo("No place in inventory");
                return;
            }

            await instance.Persist(_itemRepository);
            target.SendItem(instance);
        }
    }
}

public class FullSetCommandOptions
{
    [Value(0)] public string Target { get; set; } = "$self";
}
