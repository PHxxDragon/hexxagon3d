using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField]
    private List<Text> buttonTexts;
    public static PlayerType GetNextPlayerType(PlayerType playerType, bool isPlayer3)
    {
        if (playerType == PlayerType.Human)
        {
            return PlayerType.Dump;
        } else if (playerType == PlayerType.Dump)
        {
            return PlayerType.Smart;
        } else if (playerType == PlayerType.Smart)
        {
            return PlayerType.Smarter;
        } else if (playerType == PlayerType.Smarter)
        {
            if (isPlayer3)
            {
                return PlayerType.NoPlayer;
            }
            return PlayerType.Human;   
        } else if (playerType == PlayerType.NoPlayer)
        {
            return PlayerType.Human;
        }
        return PlayerType.Human;
    }

    public void ChangePlayer(string player)
    {
        int playerNum = int.Parse(player);
        PlayerType oldPlayerType = Config.GetPlayer(playerNum);
        PlayerType nextPlayerType = GetNextPlayerType(oldPlayerType, playerNum == 3);
        bool hasHuman = false;
        Config.SetPlayer(playerNum, nextPlayerType);
        for(int i = 1; i <= 3; i++ )
        {
            if (Config.GetPlayer(i) == PlayerType.Human)
            {
                hasHuman = true;
            }
        }
        if (!hasHuman)
        {
            Config.SetPlayer(playerNum, oldPlayerType);
        } else
        {
            buttonTexts[playerNum - 1].text = nextPlayerType.ToString();
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }
}
