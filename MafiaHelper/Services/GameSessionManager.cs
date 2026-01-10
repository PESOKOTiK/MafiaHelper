using System.Collections.Concurrent;

namespace MafiaHelper.Services
{
    public class GameSessionManager
    {
        private readonly ConcurrentDictionary<string, GameSession> _sessions = new();
        private readonly ConcurrentDictionary<string, string> _connectionSessions = new();

        public GameSession CreateSession(string gmConnectionId)
        {
            string code;
            do
            {
                code = GenerateCode();
            } while (_sessions.ContainsKey(code));

            var session = new GameSession(code);
            session.GameMasterId = gmConnectionId;
            
            _sessions.TryAdd(code, session);
            _connectionSessions.TryAdd(gmConnectionId, code);

            return session;
        }

        public GameSession? GetSession(string code)
        {
            if (string.IsNullOrEmpty(code)) return null;
            _sessions.TryGetValue(code.ToUpper(), out var session);
            return session;
        }

        public GameSession? GetSessionByConnectionId(string connectionId)
        {
            if (_connectionSessions.TryGetValue(connectionId, out var code))
            {
                return GetSession(code);
            }
            return null;
        }

        public void AddPlayerToSession(string connectionId, string code)
        {
            _connectionSessions.TryAdd(connectionId, code.ToUpper());
        }

        public void RemoveConnection(string connectionId)
        {
            _connectionSessions.TryRemove(connectionId, out _);
        }

        private string GenerateCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 4)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
