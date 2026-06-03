using QuantumCore.API;
using QuantumCore.API.Game.Party;
using QuantumCore.API.PluginTypes;
using QuantumCore.Game.Extensions;
using QuantumCore.Game.Packets.Party;

namespace QuantumCore.Game.PacketHandlers.Game.Party;

public class PartyLeaveHandler : IGamePacketHandler<PartyLeave>
{
    private readonly IPartyManager _partyManager;

    public PartyLeaveHandler(IPartyManager partyManager)
    {
        _partyManager = partyManager;
    }

    public Task ExecuteAsync(GamePacketContext<PartyLeave> ctx, CancellationToken token = default)
    {
        var requester = ctx.Connection.Player;
        if (requester is null) return Task.CompletedTask;

        var party = _partyManager.GetPartyByMemberId(requester.Player.Id);
        if (party is null) return Task.CompletedTask;

        // Pid == 0 means self-leave; otherwise it's a kick (leader only)
        var targetId = ctx.Packet.Pid == 0 ? requester.Player.Id : ctx.Packet.Pid;

        if (targetId != requester.Player.Id && party.LeaderId != requester.Player.Id)
        {
            requester.SendChatInfo("Only the party leader can kick members.");
            return Task.CompletedTask;
        }

        var world = requester.Map!.World;
        var target = world.GetPlayerById(targetId);

        // Notify remaining members before removing
        var remainingIds = party.MemberIds.Where(id => id != targetId).ToList();
        foreach (var id in remainingIds)
        {
            var member = world.GetPlayerById(id);
            member?.Connection.Send(new PartyRemove { Pid = targetId });
            member?.Connection.Send(new PartyUnlink { Pid = targetId, Vid = target?.Vid ?? 0 });
        }

        _partyManager.RemoveMember(targetId);

        target?.SetParty(null);
        target?.Connection.Send(new PartyRemove { Pid = targetId });

        // If party now has only one member, disband it
        var updatedParty = _partyManager.GetPartyByMemberId(party.LeaderId);
        if (updatedParty is null)
        {
            foreach (var id in remainingIds)
            {
                var member = world.GetPlayerById(id);
                if (member is null) continue;
                _partyManager.RemoveMember(id);
                member.SetParty(null);
            }
        }

        return Task.CompletedTask;
    }
}
