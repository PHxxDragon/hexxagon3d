using System;

public static class Config
{
    static Config()
    {
        Reset();
    }

    public static void Reset()
    {
        Player1 = PlayerType.Human;
        Player2 = PlayerType.Human;
        Player3 = PlayerType.Human;
        IsContinue = false;
    }

    public static PlayerType Player1 { get; set; }
    public static PlayerType Player2 { get; set; }
    public static PlayerType Player3 { get; set; }
    public static bool IsContinue { get; set; }
    public static PlayerType GetPlayer(int player)
    {
        if (player == 1) return Player1;
        if (player == 2) return Player2;
        if (player == 3) return Player3;
        return Player1;
    }
    public static void SetPlayer(int player, PlayerType playerType)
    {
        if (player == 1) Player1 = playerType;
        if (player == 2) Player2 = playerType;
        if (player == 3) Player3 = playerType;
    }
}
