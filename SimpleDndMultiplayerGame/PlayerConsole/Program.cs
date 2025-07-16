using PlayerConsole;

List<Player> players = new()
{
    new Player("P1", 11),
    new Player("P2", 13),
    new Player("P3", 12),
    new Player("P4", 10)
};

object lockObj = new();
Random random = new();
bool gameRunning = true;
int currentTurnIndex = 0;
int round = 1;

// Start player threads
for (int i = 0; i < players.Count; i++)
{
    int localIndex = i;
    var thread = new Thread(() => PlayerAction(localIndex));
    thread.Start();
}

void PlayerAction(int playerIndex)
{
    var attacker = players[playerIndex];

    while (gameRunning)
    {
        lock (lockObj)
        {
            if (!attacker.IsAlive || !gameRunning)
                continue;

            if (currentTurnIndex != playerIndex)
                continue;

            // Display round start if first player
            if (currentTurnIndex == players.FindIndex(p => p.IsAlive))
            {
                Console.WriteLine($"\n====== ROUND {round} ======");
            }

            // Pick a target
            var targets = players.Where(p => p != attacker && p.IsAlive).ToList();
            if (targets.Count == 0)
            {
                gameRunning = false;
                return;
            }

            var target = targets[random.Next(targets.Count)];

            Console.WriteLine($"\n🎯 {attacker.Name}'s turn. Target: {target.Name} (AC {target.AC})");
            Console.Write("🎲 Press any key to roll d20... ");
            Console.ReadKey(true);

            int roll = random.Next(1, 21);
            Console.WriteLine($"{attacker.Name} rolls a {roll}!");

            if (roll > target.AC)
            {
                target.HP = Math.Max(0, target.HP - 2);
                Console.WriteLine($"  ➤ Hit! {target.Name}'s HP is now {target.HP}");
            }
            else
            {
                Console.WriteLine("  ➤ Miss!");
            }

            Console.WriteLine("  HP Status: " + string.Join(", ",
                players.Select(p => $"{p.Name}:{p.HP}")));

            if (players.Count(p => p.IsAlive) == 1)
            {
                var winner = players.First(p => p.IsAlive);
                Console.WriteLine($"\n🏆 {winner.Name} wins the game!");
                gameRunning = false;
                return;
            }

            // Advance to next alive player's turn
            int nextIndex = currentTurnIndex;
            do
            {
                nextIndex = (nextIndex + 1) % players.Count;
            } while (!players[nextIndex].IsAlive);

            currentTurnIndex = nextIndex;

            // If next turn is back to first alive player, increment round
            if (currentTurnIndex == players.FindIndex(p => p.IsAlive))
            {
                round++;
            }
        }

        Thread.Sleep(50);
    }
}