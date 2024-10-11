namespace GameEngine;

public class Game
{
    public static GameInfo GameInfo = null!;
    public static bool StartedUp() => GameInfo != null!;
}