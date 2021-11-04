using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public Team team { get; set; }
    public BoardObjectManager boardObjectManager { get; set; }
    public bool isAI { get; set; }
    public bool isLost { get; set; }
    public Player(Team team, BoardObjectManager boardObjectManager, bool isAI)
    {
        this.team = team;
        this.boardObjectManager = boardObjectManager;
        this.isAI = isAI;
        isLost = false;
    }
}
