using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

public class BattleHub : Hub
{
    // In-memory store for joined users per battle
    private static readonly ConcurrentDictionary<int, HashSet<string>> BattlePlayers = new();

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var battleIdStr = httpContext.Request.Query["battleId"].ToString();
        if (int.TryParse(battleIdStr, out int battleId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"battle-{battleId}");
        }
        await base.OnConnectedAsync();
    }

    public async Task JoinBattle(int battleId, string userName)
    {
        var players = BattlePlayers.GetOrAdd(battleId, _ => new HashSet<string>());
        lock (players)
        {
            players.Add(userName);
        }
        // Notify all clients in the group of the updated player list
        await Clients.Group($"battle-{battleId}").SendAsync("PlayersUpdated", players.ToList());
    }

    public static IReadOnlyCollection<string> GetPlayers(int battleId)
    {
        if (BattlePlayers.TryGetValue(battleId, out var players))
            return players;
        return new List<string>();
    }
} 