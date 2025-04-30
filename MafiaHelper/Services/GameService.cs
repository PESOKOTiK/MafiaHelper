
namespace MafiaHelper.Services
{
    public class Player
    {
        public string ConnectionId { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
    }

    public class RoleConfig
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public bool Enabled { get; set; }
    }

    public class GameService
    {
        private readonly List<Player> _players = new();
        private List<RoleConfig> _roleConfigs = new()
    {
        new() { Name = "Mafia",     Count = 1, Enabled = true  },
        new() { Name = "Sheriff",   Count = 1, Enabled = true  },
        new() { Name = "Doctor",    Count = 1, Enabled = true  },
        new() { Name = "Putana",    Count = 1, Enabled = true  },
        new() { Name = "Maniac",    Count = 1, Enabled = true  },
        new() { Name = "Immortal",  Count = 1, Enabled = true  }
    };

        public IReadOnlyList<Player> Players => _players;
        public IReadOnlyList<RoleConfig> RoleConfigs => _roleConfigs;

        public void AddPlayer(Player player) => _players.Add(player);
        public void RemovePlayer(string connId) => _players.RemoveAll(p => p.ConnectionId == connId);

        public void SetRoleConfigs(List<RoleConfig> configs)
        {
            // Replace existing configs while preserving role order
            _roleConfigs = configs;
        }

        public void DealRoles()
        {
            // Build role list
            var roles = new List<string>();
            foreach (var rc in _roleConfigs.Where(rc => rc.Enabled))
            {
                for (int i = 0; i < rc.Count; i++)
                    roles.Add(rc.Name);
            }
            // Fill remaining slots with Villagers
            while (roles.Count < _players.Count)
                roles.Add("Citizen");

            // Shuffle
            var rng = new Random();
            roles = roles.OrderBy(_ => rng.Next()).ToList();

            // Assign
            for (int i = 0; i < _players.Count; i++)
                _players[i].Role = roles[i];
        }
    }
}
