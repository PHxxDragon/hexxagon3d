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
        bluePlayer = new Player(Team.Blue, boardObjectManager, false);
        redPlayer = new Player(Team.Red, boardObjectManager, false);
        greenPlayer = new Player(Team.Green, boardObjectManager, false);
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

    private Player getNextPlayer(Player player)
    {
        if (player.team == Team.Red)
            return greenPlayer;
        else if (player.team == Team.Green)
            return bluePlayer;
        else
            return redPlayer;
    }

    public void EndTurn()
    {
        Player nextPlayer = getNextPlayer(activePlayer);
        if (!nextPlayer.isLost && boardObjectManager.HasMove(nextPlayer.team))
        {
            activePlayer = nextPlayer;
            return;
        } 
        else
        {
            nextPlayer.isLost = true;
            nextPlayer = getNextPlayer(nextPlayer);
            if (!nextPlayer.isLost && boardObjectManager.HasMove(nextPlayer.team))
            {
                activePlayer = nextPlayer;
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
