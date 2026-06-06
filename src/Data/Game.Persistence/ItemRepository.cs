#nullable enable

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuantumCore.API.Core.Models;
using QuantumCore.API.Game.Types.Items;
using QuantumCore.Caching;
using QuantumCore.Game.Persistence.Entities;
using QuantumCore.Game.Persistence.Extensions;

namespace QuantumCore.Game.Persistence;

public interface IItemRepository
{
    Task<IEnumerable<Guid>> GetItemIdsForPlayerAsync(uint playerId, WindowType window);
    Task<ItemInstance?> GetItemAsync(Guid id);
    Task DeletePlayerItemsAsync(uint playerId);
    Task DeletePlayerItemAsync(uint playerId, uint itemId);
    Task DeleteItemAsync(Guid id);
    Task SaveItemAsync(ItemInstance item);
}

public class ItemRepository : IItemRepository
{
    private readonly IRedisStore _cacheManager;
    private readonly IServiceScopeFactory _scopeFactory;

    public ItemRepository(ICacheManager cacheManager, IServiceScopeFactory scopeFactory)
    {
        _cacheManager = cacheManager.Server;
        _scopeFactory = scopeFactory;
    }

    public async Task<IEnumerable<Guid>> GetItemIdsForPlayerAsync(uint playerId, WindowType window)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<GameDbContext>();
        return await db.Items
            .Where(x => x.PlayerId == playerId && x.Window == (byte)window)
            .Select(x => x.Id)
            .ToArrayAsync();
    }

    public async Task<ItemInstance?> GetItemAsync(Guid id)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<GameDbContext>();
        return await db.Items
            .Where(x => x.Id == id)
            .SelectInstance()
            .FirstOrDefaultAsync();
    }

    public async Task DeletePlayerItemsAsync(uint playerId)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<GameDbContext>();
        await db.Items.Where(x => x.PlayerId == playerId).ExecuteDeleteAsync();
    }

    public async Task DeletePlayerItemAsync(uint playerId, uint itemId)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<GameDbContext>();
        await db.Items.Where(x => x.PlayerId == playerId && x.ItemId == itemId).ExecuteDeleteAsync();
    }

    public async Task DeleteItemAsync(Guid id)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<GameDbContext>();
        await db.Items.Where(x => x.Id == id).ExecuteDeleteAsync();
    }

    public async Task SaveItemAsync(ItemInstance item)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<GameDbContext>();

        if (item.Id != Guid.Empty)
        {
            var rowsAffected = await db.Items.Where(x => x.Id == item.Id)
                .ExecuteUpdateAsync(p => p
                    .SetProperty(x => x.PlayerId, x => item.PlayerId)
                    .SetProperty(x => x.Count, x => item.Count)
                    .SetProperty(x => x.Window, x => (byte)item.Window)
                    .SetProperty(x => x.Position, x => item.Position)
                    .SetProperty(x => x.UpdatedAt, x => DateTime.UtcNow)
                );

            if (rowsAffected == 0)
            {
                // Row missing (e.g. stale cache after manual DB change) — re-insert it.
                var dbItem = new Item
                {
                    Id = item.Id,
                    PlayerId = item.PlayerId,
                    ItemId = item.ItemId,
                    Window = (byte)item.Window,
                    Position = item.Position,
                    Count = item.Count,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };
                db.Items.Add(dbItem);
                await db.SaveChangesAsync();
            }
        }
        else
        {
            var dbItem = new Item
            {
                Id = Guid.NewGuid(),
                PlayerId = item.PlayerId,
                ItemId = item.ItemId,
                Window = (byte)item.Window,
                Position = item.Position,
                Count = item.Count,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };
            db.Items.Add(dbItem);
            await db.SaveChangesAsync();
            item.Id = dbItem.Id;
        }

        var key = $"item:{item.Id}";
        await _cacheManager.Set(key, item);
    }
}
