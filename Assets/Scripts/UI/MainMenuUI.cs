using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField]
    private List<Text> buttonTexts;

    [SerializeField]
    private GameObject continueBtn;

    private void Awake()
    {
        if (!File.Exists(Application.persistentDataPath
                   + "/board.dat"))
        {
            continueBtn.SetActive(false);     
        }
    }
    public static PlayerType GetNextPlayerType(PlayerType playerType, bool isPlayer3)
    {
        if (playerType == PlayerType.Human)
        {
            return PlayerType.Dumb;
        } else if (playerType == PlayerType.Dumb)
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
/*        for(int i = 1; i <= 3; i++ )
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
        {*/
            buttonTexts[playerNum - 1].text = nextPlayerType.ToString();
/*        }*/
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;
    }

    public void ContinueGame()
    {
        if (!File.Exists(Application.persistentDataPath
                   + "/board.dat"))
        {
            Debug.LogError("Cannot continue because data dont exists, maybe the continue btn isn't hided");
        }

        Config.IsContinue = true;
        SceneManager.LoadScene("GameScene");
    }
}
