using QuantumCore.API.Game.Types.Items;

namespace QuantumCore.Game.World.Entities;

public class ActiveAffect
{
    public uint Type { get; init; }
    public EApplyType ApplyOn { get; init; }
    public int Value { get; init; }
    public uint Flag { get; init; }
    public int SpCost { get; init; }
    public bool IsPassive { get; init; }
    public long ExpiryEventId { get; set; } = -1;
}
