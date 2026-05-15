namespace MafiaAssist.Services
{
    public class Player
    {
        public string Id { get; set; }
        public string ConnectionId { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public bool IsConnected { get; set; } = true;
    }

    public class RoleConfig
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public bool Enabled { get; set; }
    }
}
