namespace QuantumCore.API.Game.Party;

public interface IPartyManager
{
    PartyData? GetPartyByMemberId(uint playerId);
    PartyData CreateParty(uint leaderId);
    void AddMember(Guid partyId, uint playerId);
    void RemoveMember(uint playerId);
    void SetDistributeMode(Guid partyId, byte mode);

    void SetPendingInvite(uint inviteeId, uint leaderId);
    uint? GetPendingInvite(uint inviteeId);
    void ClearPendingInvite(uint inviteeId);
}
