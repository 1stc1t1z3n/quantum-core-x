namespace QuantumCore.API.Game.Party;

public class PartyData
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public uint LeaderId { get; set; }
    public byte DistributeMode { get; set; }
    public List<uint> MemberIds { get; } = new();
}
