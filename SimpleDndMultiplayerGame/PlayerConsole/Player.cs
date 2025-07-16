namespace PlayerConsole;

internal class Player
{
    public string Name { get; }
    public int HP { get; set; } = 20;
    public int AC { get; }
    public bool IsAlive => HP > 0;

    public Player(string name, int ac)
    {
        Name = name;
        AC = ac;
    }
}
