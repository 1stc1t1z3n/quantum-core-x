using QuantumCore.API;
using QuantumCore.API.Game.Party;
using QuantumCore.API.PluginTypes;
using QuantumCore.Game.Extensions;
using QuantumCore.Game.Packets.Party;

namespace QuantumCore.Game.PacketHandlers.Game.Party;

public class PartyInviteHandler : IGamePacketHandler<PartyInviteRequest>
{
    private readonly IPartyManager _partyManager;

    public PartyInviteHandler(IPartyManager partyManager)
    {
        _partyManager = partyManager;
    }

    public Task ExecuteAsync(GamePacketContext<PartyInviteRequest> ctx, CancellationToken token = default)
    {
        var inviter = ctx.Connection.Player;
        if (inviter is null) return Task.CompletedTask;

        var target = inviter.Map!.World.GetPlayerById(ctx.Packet.Vid);
        if (target is null)
        {
            inviter.SendChatInfo("Player not found.");
            return Task.CompletedTask;
        }

        if (_partyManager.GetPartyByMemberId(target.Player.Id) is not null)
        {
            inviter.SendChatInfo("That player is already in a party.");
            return Task.CompletedTask;
        }

        var inviterParty = _partyManager.GetPartyByMemberId(inviter.Player.Id);
        if (inviterParty is not null && inviterParty.LeaderId != inviter.Player.Id)
        {
            inviter.SendChatInfo("Only the party leader can invite players.");
            return Task.CompletedTask;
        }

        _partyManager.SetPendingInvite(target.Player.Id, inviter.Player.Id);

        target.Connection.Send(new PartyInvite { LeaderPid = inviter.Player.Id });
        return Task.CompletedTask;
    }
}
