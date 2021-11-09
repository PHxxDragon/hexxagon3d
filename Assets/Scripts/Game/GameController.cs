using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private enum GameState { Init, Play, AI, Animate, Finished }
    [SerializeField] private InitialSetup initialSetup;
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
        bluePlayer = new MinimaxPlayer(Team.Blue, boardObjectManager, 2);
        redPlayer = new Player(Team.Red, boardObjectManager);
        greenPlayer = new MinimaxPlayer(Team.Green, boardObjectManager, 2);
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
        InitFromSetup(initialSetup);
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

    public void EndTurn()
    {
        Player nextPlayer = getPlayerFromTeam(Board.getNextTeam(activePlayer.team));
        if (!nextPlayer.isLost && boardObjectManager.HasMove(nextPlayer.team))
        {
            activePlayer = nextPlayer;
            if (activePlayer.isAI)
            {
                SetState(GameState.AI);
                activePlayer.ProcessAI(() => SetState(GameState.Play));
            }
                
            return;
        } 
        else
        {
            nextPlayer.isLost = true;
            nextPlayer = getPlayerFromTeam(Board.getNextTeam(nextPlayer.team));
            if (!nextPlayer.isLost && boardObjectManager.HasMove(nextPlayer.team))
            {
                activePlayer = nextPlayer;
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
        //throw new NotImplementedException();
    }
}
