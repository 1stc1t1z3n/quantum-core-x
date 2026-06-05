using QuantumCore.API;
using QuantumCore.API.Core.Models;
using QuantumCore.API.Game.Types.Items;
using QuantumCore.API.Game.Types.Players;
using QuantumCore.Caching;
using QuantumCore.Game.Persistence;
using static QuantumCore.Game.Extensions.ItemConstants;

namespace QuantumCore.Game.Extensions;

public static class ItemExtensions
{
    public static uint GetMinWeaponBaseDamage(this ItemData item)
    {
        return (uint)item.Values[3];
    }

    public static uint GetMaxWeaponBaseDamage(this ItemData item)
    {
        return (uint)item.Values[4];
    }

    public static uint GetMinMagicWeaponBaseDamage(this ItemData item)
    {
        return (uint)item.Values[1];
    }

    public static uint GetMaxMagicWeaponBaseDamage(this ItemData item)
    {
        return (uint)item.Values[2];
    }

    public static int GetApplyValue(this ItemData item, EApplyType type)
    {
        var apply = item.Applies.FirstOrDefault(x => (EApplyType)x.Type == type);

        return (int)(apply?.Value ?? 0);
    }

    /// <summary>
    /// Weapon damage added additionally to the base damage
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static uint GetAdditionalWeaponDamage(this ItemData item)
    {
        return (uint)item.Values[5];
    }

    public static uint GetMinWeaponDamage(this ItemData item)
    {
        return item.GetMinWeaponBaseDamage() + item.GetAdditionalWeaponDamage();
    }

    public static uint GetMaxWeaponDamage(this ItemData item)
    {
        return item.GetMaxWeaponBaseDamage() + item.GetAdditionalWeaponDamage();
    }

    public static uint GetMinMagicWeaponDamage(this ItemData item)
    {
        return item.GetMinMagicWeaponBaseDamage() + item.GetAdditionalWeaponDamage();
    }

    public static uint GetMaxMagicWeaponDamage(this ItemData item)
    {
        return item.GetMaxMagicWeaponBaseDamage() + item.GetAdditionalWeaponDamage();
    }

    public static bool IsType(this ItemData item, EItemType type)
    {
        return (EItemType)item.Type == type;
    }
    
    public static bool IsSubtype(this ItemData item, EItemSubtype subtype)
    {
        return (EItemSubtype)item.Subtype == subtype;
    }

    public static uint GetHairPartOffsetForClient(this ItemInstance? itemInstance, EPlayerClass playerClass)
    {
        if (itemInstance is null)
        {
            return 0;
        }
       
        var itemId = itemInstance.ItemId;
        if (itemId < HairPartIdOffsets.WAR_OFFSET_BASE)
        {
            return 0;
        }
        
        switch (playerClass)
        {
            case EPlayerClass.WARRIOR:
                return itemId - HairPartIdOffsets.WAR_OFFSET_BASE; // 73001 - 72000 = 1001 start hair number from
            case EPlayerClass.NINJA:
                return itemId - HairPartIdOffsets.NINJA_OFFSET_BASE;
            case EPlayerClass.SURA:
                return itemId - HairPartIdOffsets.SURA_OFFSET_BASE;
            case EPlayerClass.SHAMAN:
                return itemId - HairPartIdOffsets.SHAMAN_OFFSET_BASE;
            default:
                throw new NotImplementedException();
        }
    }

    public static EquipmentSlot? GetWearSlot(this IItemManager itemManager, uint itemId)
    {
        var proto = itemManager.GetItem(itemId);
        if (proto is null)
        {
            return null;
        }

        return proto.GetWearSlot();
    }

    public static EquipmentSlot? GetWearSlot(this ItemData proto)
    {
        if (proto.IsType(EItemType.COSTUME))
        {
            if (proto.IsSubtype(EItemSubtype.COSTUME_BODY))
            {
                return EquipmentSlot.COSTUME;
            }
            if (proto.IsSubtype(EItemSubtype.COSTUME_HAIR))
            {
                return EquipmentSlot.HAIR;
            }
        }

        return ((EWearFlags)proto.WearFlags).GetWearSlot();
    }

    private static EquipmentSlot? GetWearSlot(this EWearFlags wearFlags)
    {
        if (wearFlags.HasFlag(EWearFlags.HEAD))
        {
            return EquipmentSlot.HEAD;
        }

        if (wearFlags.HasFlag(EWearFlags.SHOES))
        {
            return EquipmentSlot.SHOES;
        }

        if (wearFlags.HasFlag(EWearFlags.BRACELET))
        {
            return EquipmentSlot.BRACELET;
        }

        if (wearFlags.HasFlag(EWearFlags.WEAPON))
        {
            return EquipmentSlot.WEAPON;
        }

        if (wearFlags.HasFlag(EWearFlags.NECKLACE))
        {
            return EquipmentSlot.NECKLACE;
        }

        if (wearFlags.HasFlag(EWearFlags.EARRINGS))
        {
            return EquipmentSlot.EARRING;
        }

        if (wearFlags.HasFlag(EWearFlags.BODY))
        {
            return EquipmentSlot.BODY;
        }

        if (wearFlags.HasFlag(EWearFlags.SHIELD))
        {
            return EquipmentSlot.SHIELD;
        }

        return null;
    }

    public static async Task<ItemInstance?> GetItem(this IItemRepository repository, ICacheManager cacheManager,
        Guid id)
    {
        var key = "item:" + id;

        if (await cacheManager.Server.Exists(key) > 0)
        {
            try
            {
                return await cacheManager.Server.Get<ItemInstance>(key);
            }
            catch
            {
                // Stale or corrupt cache entry — evict and re-populate from DB
                await cacheManager.Server.Del(key);
            }
        }

        var item = await repository.GetItemAsync(id);
        if (item is not null)
            await cacheManager.Server.Set(key, item);
        return item;
    }

    public static async Task DeletePlayerItemAsync(this IItemRepository repository, ICacheManager cacheManager,
        uint playerId, uint itemId)
    {
        var key = $"item:{itemId}";

        await cacheManager.Del(key);

        await repository.DeletePlayerItemAsync(playerId, itemId);
    }

    public static async IAsyncEnumerable<ItemInstance> GetItems(this IItemRepository repository,
        ICacheManager cacheManager, uint player, WindowType window)
    {
        var key = "items:" + player + ":" + (byte)window;
        var list = cacheManager.Server.CreateList<Guid>(key);

        // Fast path: use cached list if entries are internally consistent.
        if (await cacheManager.Server.Exists(key) > 0)
        {
            var cachedIds = (await list.Range(0, -1)).ToArray();
            var cacheInvalid = cachedIds.Length == 0;
            var seenIds = new HashSet<Guid>();
            var cachedItems = new List<ItemInstance>(cachedIds.Length);

            foreach (var id in cachedIds)
            {
                if (!seenIds.Add(id))
                {
                    cacheInvalid = true;
                    continue;
                }

                var item = await GetItem(repository, cacheManager, id);
                if (item is null || item.PlayerId != player || item.Window != window)
                {
                    cacheInvalid = true;
                    continue;
                }

                cachedItems.Add(item);
            }

            if (!cacheInvalid)
            {
                foreach (var item in cachedItems)
                {
                    yield return item;
                }

                yield break;
            }
        }

        // Guardrail: if cache is stale/partial, rebuild the list from DB source-of-truth.
        var ids = (await repository.GetItemIdsForPlayerAsync(player, window)).ToArray();

        await cacheManager.Server.Del(key);
        list = cacheManager.Server.CreateList<Guid>(key);
        foreach (var id in ids)
        {
            await list.Push(id);
        }

        foreach (var id in ids)
        {
            var item = await GetItem(repository, cacheManager, id);
            if (item is not null)
            {
                yield return item;
            }
        }
    }

    public static async Task<bool> Destroy(this ItemInstance item, ICacheManager cacheManager,
        IItemRepository itemRepository)
    {
        var key = "item:" + item.Id;

        if (item.PlayerId != default)
        {
            var oldList = cacheManager.Server.CreateList<Guid>($"items:{item.PlayerId}:{item.Window}");
            await oldList.Rem(1, item.Id);
        }

        await cacheManager.Server.Del(key);
        await itemRepository.DeleteItemAsync(item.Id);
        return true;
    }

    public static Task Persist(this ItemInstance item, IItemRepository itemRepository)
    {
        return itemRepository.SaveItemAsync(item);
    }

    /// <summary>
    /// Sets the item position, window, and owner.
    /// Refresh the cache lists if needed, and persists the item
    /// </summary>
    /// <param name="item"></param>
    /// <param name="cacheManager"></param>
    /// <param name="owner">Owner the item is given to</param>
    /// <param name="window">Window the item is placed in</param>
    /// <param name="pos">Position of the item in the window</param>
    public static async Task Set(this ItemInstance item, ICacheManager cacheManager, uint owner, WindowType window,
        uint pos, IItemRepository itemRepository)
    {
        var previousOwner = item.PlayerId;
        var previousWindow = item.Window;
        var isPlayerDifferent = item.PlayerId != owner;
        var isWindowDifferent = item.Window != window;

        item.PlayerId = owner;
        item.Window = window;
        item.Position = pos;
        await Persist(item, itemRepository);

        if (isPlayerDifferent || isWindowDifferent)
        {
            if (previousOwner != default)
            {
                // Remove from last list if that cache key exists.
                var oldKey = $"items:{previousOwner}:{previousWindow}";
                if (await cacheManager.Server.Exists(oldKey) > 0)
                {
                    var oldList = cacheManager.Server.CreateList<Guid>(oldKey);
                    await oldList.Rem(1, item.Id);
                }
            }

            if (owner != default)
            {
                // Only mutate existing list cache.
                // Creating a new list here with a single item can hide other items until next rebuild.
                var newKey = $"items:{owner}:{window}";
                if (await cacheManager.Server.Exists(newKey) > 0)
                {
                    var newList = cacheManager.Server.CreateList<Guid>(newKey);
                    await newList.Rem(1, item.Id);
                    await newList.Push(item.Id);
                }
            }
        }
        else if (owner != default)
        {
            // Same owner/window move: ensure membership only if list cache already exists.
            var currentKey = $"items:{owner}:{window}";
            if (await cacheManager.Server.Exists(currentKey) > 0)
            {
                var currentList = cacheManager.Server.CreateList<Guid>(currentKey);
                await currentList.Rem(1, item.Id);
                await currentList.Push(item.Id);
            }
        }
    }
}
