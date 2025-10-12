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
            await Clients.Group("gamemasters").SendAsync("PlayersUpdated", _game.Players);
        }

        public override async Task OnDisconnectedAsync(System.Exception exception)
        {
            _game.RemovePlayer(Context.ConnectionId);
            await Clients.Group("gamemasters").SendAsync("PlayersUpdated", _game.Players);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task RegisterGM()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "gamemasters");
            await Clients.Caller.SendAsync("PlayersUpdated", _game.Players);
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
            var newConfigs = configs.Select(c => new RoleConfig
            {
                Name = c.Name,
                Count = c.Count,
                Enabled = c.Enabled
            }).ToList();
            _game.SetRoleConfigs(newConfigs);
            await Clients.Group("gamemasters").SendAsync("RoleConfigsUpdated", configs);
        }

        public async Task DealRoles()
        {
            _game.DealRoles();
            foreach (var p in _game.Players)
            {
                await Clients.Client(p.ConnectionId).SendAsync("RoleAssigned", p.Role);
            }
            await Clients.Group("gamemasters").SendAsync("PlayersUpdated", _game.Players);
        }

        // SIMPLE VOTING METHODS
        public async Task ShowVoting(string playerNames, string currentVoter)
        {
            await Clients.All.SendAsync("ShowVoting", playerNames, currentVoter);
        }

        public async Task UpdateVotes(string playerName, int voteCount)
        {
            await Clients.All.SendAsync("UpdateVotes", playerName, voteCount);
        }

        public async Task UpdateCurrentVoter(string voterName)
        {
            await Clients.All.SendAsync("UpdateCurrentVoter", voterName);
        }

        public async Task ShowVoteResult(string result)
        {
            await Clients.All.SendAsync("ShowVoteResult", result);
        }

        public async Task HideVoting()
        {
            await Clients.All.SendAsync("HideVoting");
        }
    }
}
