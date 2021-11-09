using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private enum GameState { Init, Play, AI, Animate, Finished }
    [SerializeField] private InitialSetup initialSetup;
    [SerializeField] private InitialSetup InitialSetup3Player;
    [SerializeField] private BoardObjectManager boardObjectManager;

    private Player redPlayer;
    private Player bluePlayer;
    private Player greenPlayer;
    private Player activePlayer;
    private GameState state;

    private void SetState(GameState state)
    {
        this.state = state;
    }

    void Awake()
    {
        createPlayers();
        boardObjectManager.SetDependency(this);
    }


    private void createPlayers()
    {
        redPlayer = CreatePlayerFromPlayerType(Config.Player1, Team.Red);
        bluePlayer = CreatePlayerFromPlayerType(Config.Player2, Team.Blue);
        greenPlayer = CreatePlayerFromPlayerType(Config.Player3, Team.Green);
    }

    private Player CreatePlayerFromPlayerType(PlayerType playerType, Team team)
    {
        if (playerType == PlayerType.Human)
        {
            return new Player(team, boardObjectManager);
        } else if (playerType == PlayerType.Dump)
        {
            return new RandomPlayer(team, boardObjectManager);
        } else if (playerType == PlayerType.Smart)
        {
            return new MinimaxPlayer(team, boardObjectManager, 1);
        } else if (playerType == PlayerType.Smarter)
        {
            return new MinimaxPlayer(team, boardObjectManager, 3);
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
        StartNewGame();
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
        if (activePlayer.isAI)
        {
            SetState(GameState.AI);
            activePlayer.ProcessAI(() => SetState(GameState.Play));
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
        return ((!redPlayer.isAI && !redPlayer.isLost) ||
                (!bluePlayer.isAI && !bluePlayer.isLost) ||
                (!greenPlayer.isAI && !greenPlayer.isLost));
    }

    public void EndTurn()
    {
        if (boardObjectManager.GameEnded() || !HasHuman())
        {
            EndGame();
        }
        Team nextTeam = Board.getNextTeam(activePlayer.team);

        if (!boardObjectManager.IsLost(nextTeam))
        {
            activePlayer = getPlayerFromTeam(nextTeam);
            if (activePlayer.isAI)
            {
                SetState(GameState.AI);
                activePlayer.ProcessAI(() => SetState(GameState.Play));
            }
                
            return;
        } 
        else
        {
            nextTeam = Board.getNextTeam(nextTeam);
            if (!boardObjectManager.IsLost(nextTeam))
            {
                activePlayer = getPlayerFromTeam(nextTeam);
                if (activePlayer.isAI)
                {
                    SetState(GameState.AI);
                    activePlayer.ProcessAI(() => SetState(GameState.Play));
                }
                return;
            }
            EndGame();
        }
        }

    private void EndGame()
    {
        GameOverConfig.wonTeam = boardObjectManager.TeamWon();
        SceneManager.LoadScene("GameOverScene");
    }
}
