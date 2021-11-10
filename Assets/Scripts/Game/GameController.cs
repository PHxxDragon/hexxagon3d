using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private enum GameState { Init, Play, AI, Animate, Finished }
    [SerializeField] private InitialSetup initialSetup;
    [SerializeField] private InitialSetup InitialSetup3Player;
    [SerializeField] private BoardObjectManager boardObjectManager;
    [SerializeField] private Text redScore; 
    [SerializeField] private Text blueScore; 
    [SerializeField] private Text greenScore; 
    [SerializeField] private Text statusText; 
    [SerializeField] private GameObject greenGameObject; 

    private Player redPlayer;
    private Player bluePlayer;
    private Player greenPlayer;
    private Player activePlayer;
    private GameState state;
    private Thread aiThread;
    private bool autoPlay = false;

    private void SetState(GameState state)
    {
        this.state = state;
    }

    void Awake()
    {
        CheckContinue();
        CreatePlayers();
        boardObjectManager.SetDependency(this);
    }

    void OnDestroy()
    {
        PlayerPrefs.SetInt("ActivePlayer", (int)activePlayer.team);
        boardObjectManager.SaveGame();
    }

    private void CheckContinue()
    {
        if (Config.IsContinue)
        {
            Config.Player1 = (PlayerType)PlayerPrefs.GetInt("Player1");
            Config.Player2 = (PlayerType)PlayerPrefs.GetInt("Player2");
            Config.Player3 = (PlayerType)PlayerPrefs.GetInt("Player3");
        }
    }


    private void CreatePlayers()
    {
        redPlayer = CreatePlayerFromPlayerType(Config.Player1, Team.Red);
        bluePlayer = CreatePlayerFromPlayerType(Config.Player2, Team.Blue);
        greenPlayer = CreatePlayerFromPlayerType(Config.Player3, Team.Green);
        PlayerPrefs.SetInt("Player1", (int)Config.Player1);
        PlayerPrefs.SetInt("Player2", (int)Config.Player2);
        PlayerPrefs.SetInt("Player3", (int)Config.Player3);
        if (Config.Player1 != PlayerType.Human && Config.Player2 != PlayerType.Human && Config.Player3 != PlayerType.Human)
        {
            autoPlay = true;
        }
    }

    private Player CreatePlayerFromPlayerType(PlayerType playerType, Team team)
    {
        if (playerType == PlayerType.Human)
        {
            return new Player(team, boardObjectManager);
        } else if (playerType == PlayerType.Dumb)
        {
            return new RandomPlayer(team, boardObjectManager);
        } else if (playerType == PlayerType.Smart)
        {
            return new MinimaxPlayer(team, boardObjectManager, 1);
        } else if (playerType == PlayerType.Smarter)
        {
            return new MinimaxPlayer(team, boardObjectManager, 2);
        } else
        {
            return new Player(team, boardObjectManager);
        }
    }

    public bool IsPendingInput()
    {
        return state == GameState.Play;
    }

    void Start()
    {
        if (greenPlayer.playerType == PlayerType.NoPlayer)
        {
            greenGameObject.SetActive(false);
        }
        if (!Config.IsContinue)
        {
            StartNewGame();
        } else
        {
            boardObjectManager.LoadGame();
            activePlayer = getPlayerFromTeam((Team) PlayerPrefs.GetInt("ActivePlayer"));
            statusText.text = activePlayer.team.ToString() + " turn";
            if (activePlayer.isAI)
            {
                RunAI(activePlayer);
                statusText.text = activePlayer.team.ToString() + " is thinking...";
            }
            SetState(GameState.Play);
        }
        
    }

    private void StartNewGame()
    {
        SetState(GameState.Init);
        if (Config.Player3 == PlayerType.NoPlayer)
        {
            InitFromSetup(initialSetup);
        } else
        {
            InitFromSetup(InitialSetup3Player);
        }
        
        activePlayer = redPlayer;
        statusText.text = activePlayer.team.ToString() + " turn";
        if (activePlayer.isAI)
        {
            RunAI(activePlayer);
            statusText.text = activePlayer.team.ToString() + " is thinking...";
        }
        SetState(GameState.Play);
    }

    private void InitFromSetup(InitialSetup initialSetup)
    {
        boardObjectManager.InitBoardFromSetup(initialSetup);
    }

    public bool IsTeamTurnActive(Team team)
    {
        return activePlayer.team == team;
    }

    private Player getPlayerFromTeam(Team team)
    {
        if (team == Team.Red)
        {
            return redPlayer;
        } else if (team == Team.Blue)
        {
            return bluePlayer;
        } else
        {
            return greenPlayer;
        }
    }

    public bool HasHuman()
    {
        return (!redPlayer.isAI && !boardObjectManager.IsLost(Team.Red)) ||
                (!bluePlayer.isAI && !boardObjectManager.IsLost(Team.Blue)) ||
                (!greenPlayer.isAI && !boardObjectManager.IsLost(Team.Green));
    }

    public void EndTurn()
    {
/*        if ((boardObjectManager.GameEnded() || !HasHuman()) && !autoPlay)
        {
            EndGame();
            return;
        }*/
        Team nextTeam = Board.getNextTeam(activePlayer.team);

        if (!boardObjectManager.IsLost(nextTeam))
        {
            activePlayer = getPlayerFromTeam(nextTeam);
            statusText.text = activePlayer.team.ToString() + " turn";
            if (activePlayer.isAI)
            {
                RunAI(activePlayer);
                statusText.text = activePlayer.team.ToString() + " is thinking...";
            }
            return;
        } 
        else
        {
            nextTeam = Board.getNextTeam(nextTeam);
            if (!boardObjectManager.IsLost(nextTeam))
            {
                activePlayer = getPlayerFromTeam(nextTeam);
                statusText.text = activePlayer.team.ToString() + " turn";
                if (activePlayer.isAI)
                {
                    RunAI(activePlayer);
                    statusText.text = activePlayer.team.ToString() + " is thinking...";
                }
                return;
            }
            EndGame();
        }
        }

    public void UpdateGameScore()
    {
        redScore.text = boardObjectManager.GetBoard().GetPieceNum()[Team.Red].ToString();
        blueScore.text = boardObjectManager.GetBoard().GetPieceNum()[Team.Blue].ToString();
        greenScore.text = boardObjectManager.GetBoard().GetPieceNum()[Team.Green].ToString();
    }

    private void RunAI(Player player)
    {
        SetState(GameState.AI);
        aiThread = new Thread(player.ProcessAI);
        aiThread.Start();
        StartCoroutine(WaitForThreadCoroutine());
    }

    private IEnumerator WaitForThreadCoroutine()
    {
        while (true)
        {
            if (aiThread.IsAlive)
            {
                Debug.Log("Waiting...");
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                Debug.Log("Thread terminated");
                var move = activePlayer.aiMove;
                SetState(GameState.Play);
                boardObjectManager.Move(move.Item1, move.Item2);
                break;
            }
        }
    }
        


    private void EndGame()
    {
        GameOverConfig.wonTeam = boardObjectManager.TeamWon();
        SceneManager.LoadScene("GameOverScene");
    }
}
