using System;
using System.Collections.Generic;

public class RandomPlayer : Player
{
    public RandomPlayer(Team team, BoardObjectManager boardObjectManager)
        :base(team, boardObjectManager)
    {
        isAI = true;
    }

    public override void ProcessAI()
    {
        Board board = boardObjectManager.GetBoard();
        List<(HexCoordinates, HexCoordinates)> availableMoves = new List<(HexCoordinates, HexCoordinates)>();
        foreach (HexCoordinates start in board.GetAvailablePiecesOfTeam(team))
        {
            Dictionary<HexCoordinates, bool> availableEnds = board.GetAvailableMoves(start);
            foreach (HexCoordinates end in availableEnds.Keys)
            {
                availableMoves.Add((start, end));
            }
        }
        Random rnd = new Random();
        int move = rnd.Next(0, availableMoves.Count);
        aiMove = availableMoves[move];
        //boardObjectManager.Move(availableMoves[move].Item1, availableMoves[move].Item2);
    }
}
