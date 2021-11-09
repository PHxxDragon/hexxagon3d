using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    private int board_size;
    private HexStorage<Piece> grid;
    private List<HexCoordinates> invalids = new List<HexCoordinates>();
    private List<ActionHistory> actionHistories = new List<ActionHistory>();
    private Dictionary<Team, int> pieceNum = new Dictionary<Team, int>();
    private Dictionary<Team, bool> isLost = new Dictionary<Team, bool>();
/*    public List<(HexCoordinates, HexCoordinates)> moveHistory = new List<(HexCoordinates, HexCoordinates)>();*/

    public Dictionary<Team, int> GetPieceNum()
    {
        return pieceNum;
    }

    public enum Action { Move, Copy, Attack };
    public class ActionHistory
    {
        public Action action { get; set; }
        public HexCoordinates start;
        public HexCoordinates end;
        public Team team;

        public ActionHistory(Action action, HexCoordinates start, HexCoordinates end, Team team)
        {
            this.action = action;
            this.start = start;
            this.end = end;
            this.team = team;
        }
    }

    public Board Copy()
    {
        Board board = new Board(board_size);
        foreach (HexCoordinates coords in grid.IterateStorage())
        {
            board.grid.put(coords, grid.get(coords) != null ? grid.get(coords).Copy() : null);
        }
        for (int i = 0; i < invalids.Count; i++)
        {
            board.invalids.Add(invalids[i].Copy());
        }
        foreach (Team team in pieceNum.Keys)
        {
            board.pieceNum.Add(team, pieceNum[team]);
        }
        foreach (Team team in isLost.Keys)
        {
            board.isLost.Add(team, isLost[team]);
        }
        /* for (int i = 0; i < actionHistories.Count; i++)
        {
            board.actionHistories.Add(actionHistories[i]);
        }*/
/*        foreach (var move in moveHistory)
        {
            board.moveHistory.Add(move);
        }*/
        return board;

    }

    

    public bool HasMove(Team team)
    {
        return !isLost[team];
    }

    private void UpdateIsLost()
    {
        if (!isLost[Team.Red])
            isLost[Team.Red] = !CheckForTeamAlive(Team.Red);
        if (!isLost[Team.Blue])
            isLost[Team.Blue] = !CheckForTeamAlive(Team.Blue);
        if (!isLost[Team.Green])
            isLost[Team.Green] = !CheckForTeamAlive(Team.Green);
    }

    private bool CheckForTeamAlive(Team team)
    {
        foreach (HexCoordinates coords in IterateBoardPosition())
        {
            if (HasPiece(coords) && GetTeam(coords) == team && GetAvailableMoves(coords).Count > 0)
                return true;
        }
        return false;
    }

    public bool Is2TeamLeft()
    {
        return (isLost[Team.Red] || isLost[Team.Blue] || isLost[Team.Green]);
    }

    public bool IsLost(Team team)
    {
        return isLost[team];
    }

    public bool GameEnded()
    {
        return (isLost[Team.Red] && isLost[Team.Blue]) || (isLost[Team.Blue] && isLost[Team.Green]) || (isLost[Team.Red] && isLost[Team.Green]);
    }


    public Board(int board_size)
    {
        this.board_size = board_size;
        grid = new HexStorage<Piece>(board_size);
    }

    public Board(int board_size, InitialSetup initialSetup)
    {
        this.board_size = board_size;
        grid = new HexStorage<Piece>(board_size);
        InitPiecenum();

        for (int i = 0; i < initialSetup.GetPiecesCount(); i++)
        {
            HexCoordinates coords = initialSetup.getCoordsAtIndex(i);
            Team team = initialSetup.getTeamAtIndex(i);
            if (team != Team.Invalid)
            {
                Piece newPiece = new Piece(coords, team);
                grid.put(coords, newPiece);
                pieceNum[team] = pieceNum[team] + 1;
            } else
            {
                invalids.Add(coords);
            }
        }
        UpdateIsLost();
    }

    public int Evaluate(Team team)
    {
        return pieceNum[team];
    }

    public static Team getNextTeam(Team team)
    {
        if (team == Team.Red)
        {
            return Team.Green;
        } else if (team == Team.Green)
        {
            return Team.Blue;
        } else
        {
            return Team.Red;
        }
    }

    public static Team getLastTeam(Team team)
    {
        if (team == Team.Red)
        {
            return Team.Blue;
        }
        else if (team == Team.Green)
        {
            return Team.Red;
        }
        else
        {
            return Team.Green;
        }
    }

    private void InitPiecenum()
    {
        pieceNum.Add(Team.Red, 0);
        pieceNum.Add(Team.Blue, 0);
        pieceNum.Add(Team.Green, 0);
        isLost.Add(Team.Red, false);
        isLost.Add(Team.Blue, false);
        isLost.Add(Team.Green, false);
    }

    public bool HasPiece(HexCoordinates coords)
    {
        return CheckIfCoordsIsValid(coords) && grid.get(coords) != null;
    }

    public Team GetTeam(HexCoordinates coords)
    {
        if (HasPiece(coords))
        {
            return grid.get(coords).team;
        }
        return Team.Empty;
    }

    private void SetTeam(HexCoordinates coords, Team team)
    {
        if (HasPiece(coords))
        {
            grid.get(coords).team = team;
        }
    }

    public List<ActionHistory> GetActionHistories()
    {
        return actionHistories;
    }

    public IEnumerable<HexCoordinates> IterateBoardPosition()
    {
        for (int q = -board_size; q <= board_size; q++)
        {
            for (int r = -board_size; r <= board_size; r++)
            {
                HexCoordinates coords = new HexCoordinates(q, r);
                if (CheckIfCoordsIsValid(coords))
                {
                    yield return coords;
                }
            }
        }
    }

    public void Move(HexCoordinates selected, HexCoordinates new_selected)
    {
        if (!IsValidMove(selected, new_selected))
        {
            Debug.LogError("Invalid move !!!!!");
            return;
        }

        actionHistories = new List<ActionHistory>();

        int distance = HexCoordinates.Distance(selected, new_selected);
        Team currentTeam = GetTeam(selected);
        if (distance == 1)
        {
            actionHistories.Add(new ActionHistory(Action.Copy, selected, new_selected, currentTeam));
            pieceNum[currentTeam] = pieceNum[currentTeam] + 1;
        } 
        else if (distance == 2)
        {
            grid.put(selected, null);
            actionHistories.Add(new ActionHistory(Action.Move, selected, new_selected, currentTeam));
        }

        grid.put(new_selected, new Piece(new_selected, currentTeam));
        foreach (HexCoordinates coords in HexCoordinates.ListAllCoordsAtRange(new_selected, 1))
        {
            if (HasPiece(coords) && GetTeam(coords) != currentTeam)
            {
                pieceNum[GetTeam(coords)] -= 1;
                SetTeam(coords, currentTeam);
                pieceNum[currentTeam] = pieceNum[currentTeam] + 1;
                actionHistories.Add(new ActionHistory(Action.Attack, new_selected, coords, currentTeam));
            }
        }
        UpdateIsLost();
        CheckPieceNum();
    }

    private void CheckPieceNum()
    {
        int greenCount = 0;
        int redCount = 0;
        int blueCount = 0;
        foreach (HexCoordinates coords in IterateBoardPosition())
        {
            if (HasPiece(coords))
            {
                if (GetTeam(coords) == Team.Red)
                {
                    redCount += 1;
                } else if (GetTeam(coords) == Team.Blue)
                {
                    blueCount += 1;
                } else if (GetTeam(coords) == Team.Green)
                {
                    greenCount += 1;
                }
            }
        }
        if (greenCount != pieceNum[Team.Green] || redCount != pieceNum[Team.Red] || blueCount != pieceNum[Team.Blue])
        {
            Debug.LogError("Piece num is wrong !!!!");
        }
    }

    public bool IsValidMove(HexCoordinates selected, HexCoordinates new_selected)
    {
        return  CheckIfCoordsIsValid(selected) && CheckIfCoordsIsValid(new_selected) && 
                HasPiece(selected) && !HasPiece(new_selected) && 
                HexCoordinates.Distance(selected, new_selected) <= 2;
    }

    public IEnumerable<HexCoordinates> GetAvailablePiecesOfTeam(Team team)
    {
        foreach (HexCoordinates coords in IterateBoardPosition())
        {
            if (HasPiece(coords) && GetTeam(coords) == team)
            {
                yield return coords;
            }
        }
    }

    public Dictionary<HexCoordinates, bool> GetAvailableMoves(HexCoordinates selected)
    {
        Dictionary<HexCoordinates, bool> dict = new Dictionary<HexCoordinates, bool>();
        foreach (var coords in HexCoordinates.ListAllCoordsAtRange(selected, 1))
        {
            if (CheckIfCoordsIsValid(coords) && !HasPiece(coords))
            {
                dict.Add(coords, true);
            }
        }
        foreach(var coords in HexCoordinates.ListAllCoordsAtRange(selected, 2))
        {
            if (CheckIfCoordsIsValid(coords) && !HasPiece(coords))
            {
                dict.Add(coords, false);
            }
        }
        return dict;
    }

    public bool CheckIfCoordsIsValid(HexCoordinates coords)
    {
        return (coords.q >= -board_size && coords.q <= board_size &&
                coords.r >= -board_size && coords.r <= board_size &&
                coords.q + coords.r >= -board_size && coords.q + coords.r <= board_size &&
                !invalids.Contains(coords));
    }
 
}
