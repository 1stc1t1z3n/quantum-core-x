using System.Collections.Concurrent;
using QuantumCore.API.Game.Party;

namespace QuantumCore.Game;

public class PartyManager : IPartyManager
{
    // partyId → party
    private readonly ConcurrentDictionary<Guid, PartyData> _parties = new();
    // playerId → partyId
    private readonly ConcurrentDictionary<uint, Guid> _memberIndex = new();
    // inviteeId → leaderId
    private readonly ConcurrentDictionary<uint, uint> _pendingInvites = new();

    public PartyData? GetPartyByMemberId(uint playerId)
    {
        if (_memberIndex.TryGetValue(playerId, out var partyId) &&
            _parties.TryGetValue(partyId, out var party))
            return party;
        return null;
    }

    public PartyData CreateParty(uint leaderId)
    {
        var party = new PartyData { LeaderId = leaderId };
        party.MemberIds.Add(leaderId);
        _parties[party.Id] = party;
        _memberIndex[leaderId] = party.Id;
        return party;
    }

    public void AddMember(Guid partyId, uint playerId)
    {
        if (!_parties.TryGetValue(partyId, out var party)) return;
        if (_memberIndex.ContainsKey(playerId)) return;
        party.MemberIds.Add(playerId);
        _memberIndex[playerId] = partyId;
    }

    public void RemoveMember(uint playerId)
    {
        if (!_memberIndex.TryRemove(playerId, out var partyId)) return;
        if (!_parties.TryGetValue(partyId, out var party)) return;

        party.MemberIds.Remove(playerId);

        if (party.MemberIds.Count <= 1)
        {
            // Disband — remove the last member too
            foreach (var id in party.MemberIds)
                _memberIndex.TryRemove(id, out _);
            _parties.TryRemove(partyId, out _);
        }
        else if (party.LeaderId == playerId)
        {
            // Promote first remaining member to leader
            party.LeaderId = party.MemberIds[0];
        }
    }

    public void SetDistributeMode(Guid partyId, byte mode)
    {
        if (_parties.TryGetValue(partyId, out var party))
            party.DistributeMode = mode;
    }

    public void SetPendingInvite(uint inviteeId, uint leaderId) =>
        _pendingInvites[inviteeId] = leaderId;

    public uint? GetPendingInvite(uint inviteeId) =>
        _pendingInvites.TryGetValue(inviteeId, out var leaderId) ? leaderId : null;

    public void ClearPendingInvite(uint inviteeId) =>
        _pendingInvites.TryRemove(inviteeId, out _);
}
