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
}
