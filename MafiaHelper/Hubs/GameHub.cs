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
        private readonly GameSessionManager _manager;

        public GameHub(GameSessionManager manager) => _manager = manager;

        public async Task<string> CreateSession()
        {
            var session = _manager.CreateSession(Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, "GM_" + session.Code);
            await Groups.AddToGroupAsync(Context.ConnectionId, "Session_" + session.Code);
            return session.Code;
        }

        public async Task Join(string playerName, string code)
        {
            var session = _manager.GetSession(code);
            if (session == null)
            {
                throw new HubException("Session not found");
            }

            var player = new Player
            {
                ConnectionId = Context.ConnectionId,
                Name = playerName,
                Role = null
            };
            session.AddPlayer(player);
            _manager.AddPlayerToSession(Context.ConnectionId, session.Code);

            await Groups.AddToGroupAsync(Context.ConnectionId, "Session_" + session.Code);

            await Clients.Group("GM_" + session.Code).SendAsync("PlayersUpdated", session.Players);
        }

        public override async Task OnDisconnectedAsync(System.Exception exception)
        {
            var session = _manager.GetSessionByConnectionId(Context.ConnectionId);
            if (session != null)
            {
                session.RemovePlayer(Context.ConnectionId);
                _manager.RemoveConnection(Context.ConnectionId);
                
                // If it was a player, update GM
                // If it was GM... well, session technically continues but no GM, i guess players should start again :)
                await Clients.Group("GM_" + session.Code).SendAsync("PlayersUpdated", session.Players);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task GetInitialState()
        {
             var session = _manager.GetSessionByConnectionId(Context.ConnectionId);
             if (session != null && session.GameMasterId == Context.ConnectionId)
             {
                 await Clients.Caller.SendAsync("PlayersUpdated", session.Players);
                 var cfg = session.RoleConfigs.Select(rc => new RoleConfigDto
                 {
                     Name = rc.Name,
                     Count = rc.Count,
                     Enabled = rc.Enabled
                 }).ToList();
                 await Clients.Caller.SendAsync("RoleConfigsUpdated", cfg);
             }
        }

        public async Task UpdateRoleConfigs(List<RoleConfigDto> configs)
        {
            var session = _manager.GetSessionByConnectionId(Context.ConnectionId);
            if (session == null) return;

            var newConfigs = configs.Select(c => new RoleConfig
            {
                Name = c.Name,
                Count = c.Count,
                Enabled = c.Enabled
            }).ToList();
            session.SetRoleConfigs(newConfigs);
        
            await Clients.Group("GM_" + session.Code).SendAsync("RoleConfigsUpdated", configs);
        }

        public async Task DealRoles()
        {
            var session = _manager.GetSessionByConnectionId(Context.ConnectionId);
            if (session == null) return;

            session.DealRoles();
            foreach (var p in session.Players)
            {
                await Clients.Client(p.ConnectionId).SendAsync("RoleAssigned", p.Role);
            }
            await Clients.Group("GM_" + session.Code).SendAsync("PlayersUpdated", session.Players);
        }

        // VOTING METHODS
        public async Task ShowVoting(string playerNames, string currentVoter)
        {
            var session = _manager.GetSessionByConnectionId(Context.ConnectionId);
            if (session != null)
            {
                await Clients.Group("Session_" + session.Code).SendAsync("ShowVoting", playerNames, currentVoter);
            }
        }

        public async Task UpdateVotes(string playerName, int voteCount)
        {
            var session = _manager.GetSessionByConnectionId(Context.ConnectionId);
            if (session != null)
            {
                await Clients.Group("Session_" + session.Code).SendAsync("UpdateVotes", playerName, voteCount);
            }
        }

        public async Task UpdateCurrentVoter(string voterName)
        {
            var session = _manager.GetSessionByConnectionId(Context.ConnectionId);
            if (session != null)
            {
                await Clients.Group("Session_" + session.Code).SendAsync("UpdateCurrentVoter", voterName);
            }
        }

        public async Task ShowVoteResult(string result)
        {
            var session = _manager.GetSessionByConnectionId(Context.ConnectionId);
            if (session != null)
            {
                await Clients.Group("Session_" + session.Code).SendAsync("ShowVoteResult", result);
            }
        }

        public async Task HideVoting()
        {
            var session = _manager.GetSessionByConnectionId(Context.ConnectionId);
            if (session != null)
            {
                await Clients.Group("Session_" + session.Code).SendAsync("HideVoting");
            }
        }
    }
}
