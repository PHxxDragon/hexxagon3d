using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MinimaxPlayer : Player
{
    public int depth { get; set; }
    public MinimaxPlayer(Team team, BoardObjectManager boardObjectManager, int depth)
        : base(team, boardObjectManager)
    {
        isAI = true;
        this.depth = depth;
    }

    public override void ProcessAI()
    {
        Board board = boardObjectManager.GetBoard();
        List<(HexCoordinates, HexCoordinates)> availableMoves = GetAvailableMoves(board, team);
        Debug.Log(availableMoves.Count);
        int max_score = -1000;
        int min_score = 1000;
        var max_move = availableMoves[0];
        foreach (var move in availableMoves)
        {
            var new_board = board.Copy();
            new_board.Move(move.Item1, move.Item2);
            /*            new_board.moveHistory.Add(move);*/
            var final_board = (!board.Is2TeamLeft()) ? Minimax(new_board, Board.getNextTeam(team), depth/2) : AlphaBeta(new_board, Board.getNextTeam(team), depth, max_score, min_score);
            int score = final_board.Evaluate(team);
            if (max_score < score)
            {
                max_score = score;
                max_move = move;
            }
        }
        aiMove = max_move;
        //boardObjectManager.Move(max_move.Item1, max_move.Item2);
    }

    private Board AlphaBeta(Board board, Team team, int remainDepth, int alpha, int beta)
    {
        if (remainDepth == 0 || board.GameEnded())
        {
            return board;
        }
        if (board.IsLost(team))
        {
            return AlphaBeta(board, Board.getNextTeam(team), remainDepth, alpha, beta);
        }
        List<(HexCoordinates, HexCoordinates)> availableMoves = GetAvailableMoves(board, team);
        if (availableMoves.Count == 0)
        {
            //This case wont happen
            Debug.Log("number of available move is 0");
            return AlphaBeta(board, Board.getNextTeam(team), remainDepth - 1, alpha, beta);
        }
        int v = team == this.team ? - 1000 : 1000;
        Board max_board = null;
        foreach (var move in availableMoves)
        {
            var new_board = board.Copy();
            new_board.Move(move.Item1, move.Item2);
/*            new_board.moveHistory.Add(move);*/
            var final_board = AlphaBeta(new_board, Board.getNextTeam(team), remainDepth - 1, alpha, beta);
            int score = final_board.Evaluate(this.team);
            if (team == this.team)
            {
                if (v < score)
                {
                    v = score;
                    max_board = final_board;
                }
                if (alpha < v)
                {
                    alpha = v;
                }
                if (beta <= alpha)
                {
                    return max_board;
                }
            } else
            {
                if (v > score)
                {
                    v = score;
                    max_board = final_board;
                }
                if (beta > v)
                {
                    beta = v;
                }
                if (beta <= alpha)
                {
                    return max_board;
                }
            }
        }
        return max_board;
    }

    private Board Minimax(Board board, Team team, int remainDepth)
    {
        if (remainDepth == 0 || board.GameEnded())
        {
            return board;
        }
        if (board.IsLost(team))
        {
            return Minimax(board, Board.getNextTeam(team), remainDepth);
        }
        List<(HexCoordinates, HexCoordinates)> availableMoves = GetAvailableMoves(board, team);
        if (availableMoves.Count == 0)
        {
            //This case wont happen
            Debug.Log("number of available move is 0");
            return Minimax(board, Board.getNextTeam(team), remainDepth - 1);
        }
        int max_score = -1000;
        Board max_board = null;
        foreach (var move in availableMoves)
        {
            var new_board = board.Copy();
            new_board.Move(move.Item1, move.Item2);
            var final_board = Minimax(new_board, Board.getNextTeam(team), remainDepth - 1);
            int score = final_board.Evaluate(team);
            if (max_score < score)
            {
                max_score = score;
                max_board = final_board;
            }
        }
        return max_board;
    }
    private List<(HexCoordinates, HexCoordinates)> GetAvailableMoves(Board board, Team team)
    {
        List<(HexCoordinates, HexCoordinates)> availableMoves = new List<(HexCoordinates, HexCoordinates)>();
        HashSet<HexCoordinates> copyEnds = new HashSet<HexCoordinates>();
        foreach (HexCoordinates start in board.GetAvailablePiecesOfTeam(team))
        {
            Dictionary<HexCoordinates, bool> availableEnds = board.GetAvailableMoves(start);
            foreach (HexCoordinates end in availableEnds.Keys)
            {
                if (!availableEnds[end]) availableMoves.Add((start, end));
                else
                {
                    if (!copyEnds.Contains(end))
                    {
                        copyEnds.Add(end);
                        availableMoves.Add((start, end));
                    }
                }
            }
        }
        Utils.Shuffle<(HexCoordinates, HexCoordinates)>(availableMoves);
        return availableMoves;
    }
}
