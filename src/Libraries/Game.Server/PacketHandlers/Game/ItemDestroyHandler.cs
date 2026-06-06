using QuantumCore.API;
using QuantumCore.API.PluginTypes;
using QuantumCore.Caching;
using QuantumCore.Game.Extensions;
using QuantumCore.Game.Packets;
using QuantumCore.Game.Persistence;

namespace QuantumCore.Game.PacketHandlers.Game;

public class ItemDestroyHandler : IGamePacketHandler<ItemDestroy>
{
    private readonly ICacheManager _cacheManager;
    private readonly IItemRepository _itemRepository;

    public ItemDestroyHandler(ICacheManager cacheManager, IItemRepository itemRepository)
    {
        _cacheManager = cacheManager;
        _itemRepository = itemRepository;
    }

    public async Task ExecuteAsync(GamePacketContext<ItemDestroy> ctx, CancellationToken token = default)
    {
        var player = ctx.Connection.Player;
        if (player is null)
        {
            ctx.Connection.Close();
            return;
        }

        var item = player.GetItem(ctx.Packet.Window, ctx.Packet.Position);
        if (item is null)
        {
            return;
        }

        player.RemoveItem(item);
        player.SendRemoveItem(ctx.Packet.Window, ctx.Packet.Position);
        await item.Destroy(_cacheManager, _itemRepository);
    }
}
