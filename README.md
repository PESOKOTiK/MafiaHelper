# 🕵️‍♂️ MafiaHelper

**The ultimate companion for your Mafia game nights.**

MafiaHelper is a real-time web application designed to streamline the role-playing game Mafia. It handles role distribution, voting phases, timers, and game state management, allowing the Game Master (GM) to focus on the narrative and players to focus on the deception.

Built with **ASP.NET Core**, **SignalR**, and **Tailwind CSS**.

---

### ✨ Features

- **Real-Time Synchronization**: Instant updates for all players using WebSockets.
- **Dynamic Role Dealing**: GM can configure and randomize roles (Mafia, Don, Sheriff, Doctor, etc.).
- **Digital Voting System**: No more counting hands—players vote on their phones, results calculated instantly.
- **Game Master Dashboard**: Complete control over game flow, player status (alive/dead), and game phases.
- **Mobile-First Design**: Optimized for players to use on their smartphones.

---

### 📱 Player Experience

Players can join a session using a unique room code. Once the game starts, they receive their secret role card and can participate in voting rounds directly from their device.

![Join and Role Screen](https://i.imgur.com/qQqiolh.png)

---

### 🛠️ Game Master Dashboard

The command center for the host. Create sessions, manage settings, kill/revive players, and advance through day/night phases with a single click.

![GM Dashboard](https://i.imgur.com/PAibkq0.png)

---

### 🗳️ Interactive Voting & Phases

When the town needs to decide who to eliminate, the voting interface appears on everyone's screens. Real-time counters and automatic result calculation make the process seamless.

![Voting Interface](https://i.imgur.com/HSOrT9Y.png)

---

### 🚀 Tech Stack

- **Backend**: C# / ASP.NET Core 8
- **Real-Time Communication**: SignalR
- **Frontend**: Razor Pages + JavaScript
- **Styling**: Tailwind CSS

### 🏃‍♂️ Getting Started

1. Download selfhosted version.
2. Run the application:
   ```bash
   dotnet run --project MafiaHelper
   ```
3. Open `http://localhost:5000` (or the URL shown in console).
4. **GM**: Navigate to `/GameMaster` to be gm.
5. **Players**: Go to the home page to play.

---

###OR

1. Go to https://144.21.34.14:5165 to play right now

---

*Enjoy the game, and trust no one.* 🌑
