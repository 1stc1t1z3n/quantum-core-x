using QuantumCore.API;
using QuantumCore.API.Game.Party;
using QuantumCore.API.PluginTypes;
using QuantumCore.Game.Extensions;
using QuantumCore.Game.Packets.Party;

namespace QuantumCore.Game.PacketHandlers.Game.Party;

public class PartyInviteAnswerHandler : IGamePacketHandler<PartyInviteAnswer>
{
    private readonly IPartyManager _partyManager;

    public PartyInviteAnswerHandler(IPartyManager partyManager)
    {
        _partyManager = partyManager;
    }

    public Task ExecuteAsync(GamePacketContext<PartyInviteAnswer> ctx, CancellationToken token = default)
    {
        var invitee = ctx.Connection.Player;
        if (invitee is null) return Task.CompletedTask;

        var pendingLeaderId = _partyManager.GetPendingInvite(invitee.Player.Id);
        if (pendingLeaderId is null || pendingLeaderId != ctx.Packet.LeaderPid)
            return Task.CompletedTask;

        _partyManager.ClearPendingInvite(invitee.Player.Id);

        if (ctx.Packet.Accept == 0)
            return Task.CompletedTask;

        var world = invitee.Map!.World;
        var leader = world.GetPlayerById(ctx.Packet.LeaderPid);
        if (leader is null) return Task.CompletedTask;

        // Get or create party
        var party = _partyManager.GetPartyByMemberId(leader.Player.Id)
                    ?? _partyManager.CreateParty(leader.Player.Id);

        _partyManager.AddMember(party.Id, invitee.Player.Id);

        // Notify all current members (including the new one) of every member
        var members = party.MemberIds
            .Select(id => world.GetPlayerById(id))
            .Where(p => p is not null)
            .ToList();

        foreach (var member in members)
        {
            member!.SetParty(party);

            // Tell this member about all others
            foreach (var other in members.Where(o => o!.Player.Id != member.Player.Id))
            {
                member.Connection.Send(new PartyAdd { Pid = other!.Player.Id, Name = other.Player.Name });
                member.Connection.Send(new PartyLink { Pid = other.Player.Id, Vid = other.Vid });
                member.Connection.Send(new PartyUpdate
                {
                    Pid = other.Player.Id,
                    PercentHp = other.HealthPercentage
                });
            }
        }

        return Task.CompletedTask;
    }
}
