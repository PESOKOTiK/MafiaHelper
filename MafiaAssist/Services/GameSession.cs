using MafiaAssist.Hubs;

namespace MafiaAssist.Services
{
    public class GameSession
    {
        public string Code { get; private set; }
        public string GameMasterId { get; set; }

        private readonly List<Player> _players = new();
        private List<RoleConfig> _roleConfigs = new()
        {
            new() { Name = "Mafia",     Count = 1, Enabled = true  },
            new() { Name = "Don",       Count = 1, Enabled = false },
            new() { Name = "Sheriff",   Count = 1, Enabled = false },
            new() { Name = "Doctor",    Count = 1, Enabled = false },
            new() { Name = "Putana",    Count = 1, Enabled = false },
            new() { Name = "Maniac",    Count = 1, Enabled = false },
            new() { Name = "Immortal",  Count = 1, Enabled = false }
        };

        public GameSession(string code)
        {
            Code = code;
        }

        public IReadOnlyList<Player> Players => _players;
        public IReadOnlyList<RoleConfig> RoleConfigs => _roleConfigs;

        public void AddPlayer(Player player)
        {
            _players.Add(player);
        }

        public void MarkPlayerDisconnected(string connId)
        {
            var player = _players.FirstOrDefault(p => p.ConnectionId == connId);
            if (player != null)
            {
                player.IsConnected = false;
            }
        }

        public void SetRoleConfigs(List<RoleConfig> configs)
        {
            _roleConfigs = configs;
        }

        public void DealRoles()
        {
            var roles = new List<string>();
            int maxMafia = Math.Max(1, _players.Count / 3);
            int currentMafia = 0;
            int totalSpecial = 0;

            var donConfig = _roleConfigs.FirstOrDefault(r => r.Name == "Don" && r.Enabled);
            if (donConfig != null && donConfig.Count > 0)
            {
                roles.Add("Don");
                currentMafia++;
                totalSpecial++;
            }

            var mafiaConfig = _roleConfigs.FirstOrDefault(r => r.Name == "Mafia" && r.Enabled);
            if (mafiaConfig != null)
            {
                for (int i = 0; i < mafiaConfig.Count && currentMafia < maxMafia; i++)
                {
                    roles.Add("Mafia");
                    currentMafia++;
                    totalSpecial++;
                }
            }

            foreach (var rc in _roleConfigs.Where(rc => rc.Enabled && rc.Name != "Don" && rc.Name != "Mafia"))
            {
                for (int i = 0; i < rc.Count && totalSpecial < _players.Count - 1; i++)
                {
                    roles.Add(rc.Name);
                    totalSpecial++;
                }
            }

            while (roles.Count < _players.Count)
                roles.Add("Citizen");

            var rng = new Random();
            roles = roles.OrderBy(_ => rng.Next()).ToList();

            for (int i = 0; i < _players.Count; i++)
                _players[i].Role = roles[i];
        }
    }
}
