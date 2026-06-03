using QuantumCore.API;
using QuantumCore.API.Game.Party;
using QuantumCore.API.PluginTypes;
using QuantumCore.Game.Extensions;
using QuantumCore.Game.Packets.Party;

namespace QuantumCore.Game.PacketHandlers.Game.Party;

public class PartyParameterHandler : IGamePacketHandler<PartyParameterIn>
{
    private readonly IPartyManager _partyManager;

    public PartyParameterHandler(IPartyManager partyManager)
    {
        _partyManager = partyManager;
    }

    public Task ExecuteAsync(GamePacketContext<PartyParameterIn> ctx, CancellationToken token = default)
    {
        var player = ctx.Connection.Player;
        if (player is null) return Task.CompletedTask;

        var party = _partyManager.GetPartyByMemberId(player.Player.Id);
        if (party is null || party.LeaderId != player.Player.Id) return Task.CompletedTask;

        _partyManager.SetDistributeMode(party.Id, ctx.Packet.DistributeMode);

        // Broadcast new distribute mode to all members
        var world = player.Map!.World;
        foreach (var memberId in party.MemberIds)
        {
            var member = world.GetPlayerById(memberId);
            member?.Connection.Send(new PartyParameterOut { DistributeMode = ctx.Packet.DistributeMode });
        }

        return Task.CompletedTask;
    }
}
