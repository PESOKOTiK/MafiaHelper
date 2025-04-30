using Microsoft.AspNetCore.SignalR;
using MafiaHelper.Services;

namespace MafiaHelper.Hubs
{
    public class RoleConfigDto
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public bool Enabled { get; set; }
    }

    public class GameHub : Hub
    {
        private readonly GameService _game;
        public GameHub(GameService game) => _game = game;

        public async Task Join(string playerName)
        {
            var player = new Player
            {
                ConnectionId = Context.ConnectionId,
                Name = playerName,
                Role = null
            };
            _game.AddPlayer(player);
            // Notify GMs of updated roster
            await Clients.Group("gamemasters")
                         .SendAsync("PlayersUpdated", _game.Players);
        }

        public override async Task OnDisconnectedAsync(System.Exception exception)
        {
            _game.RemovePlayer(Context.ConnectionId);
            await Clients.Group("gamemasters")
                         .SendAsync("PlayersUpdated", _game.Players);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task RegisterGM()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "gamemasters");
            // Send current players
            await Clients.Caller.SendAsync("PlayersUpdated", _game.Players);
            // Send current role configs
            var cfg = _game.RoleConfigs.Select(rc => new RoleConfigDto
            {
                Name = rc.Name,
                Count = rc.Count,
                Enabled = rc.Enabled
            }).ToList();
            await Clients.Caller.SendAsync("RoleConfigsUpdated", cfg);
        }

        public async Task UpdateRoleConfigs(List<RoleConfigDto> configs)
        {
            // Map DTOs back to internal RoleConfig
            var newConfigs = configs.Select(c => new RoleConfig
            {
                Name = c.Name,
                Count = c.Count,
                Enabled = c.Enabled
            }).ToList();
            _game.SetRoleConfigs(newConfigs);
            // Broadcast new configs to all GMs
            await Clients.Group("gamemasters")
                         .SendAsync("RoleConfigsUpdated", configs);
        }

        public async Task DealRoles()
        {
            _game.DealRoles();
            // Send each player their own role
            foreach (var p in _game.Players)
            {
                await Clients.Client(p.ConnectionId)
                             .SendAsync("RoleAssigned", p.Role);
            }
            // Send updated roster to GMs
            await Clients.Group("gamemasters")
                         .SendAsync("PlayersUpdated", _game.Players);
        }
    }

}
