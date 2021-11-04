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

    public Board(int board_size, InitialSetup initialSetup)
    {
        this.board_size = board_size;
        grid = new HexStorage<Piece>(board_size);

        for (int i = 0; i < initialSetup.GetPiecesCount(); i++)
        {
            HexCoordinates coords = initialSetup.getCoordsAtIndex(i);
            Team team = initialSetup.getTeamAtIndex(i);
            if (team != Team.Invalid)
            {
                Piece newPiece = new Piece(coords, team);
                grid.put(coords, newPiece);
            } else
            {
                invalids.Add(coords);
            }
        }
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

    public void SetTeam(HexCoordinates coords, Team team)
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
            return;
        }

        actionHistories = new List<ActionHistory>();

        int distance = HexCoordinates.Distance(selected, new_selected);
        Team currentTeam = GetTeam(selected);
        if (distance == 1)
        {
            actionHistories.Add(new ActionHistory(Action.Copy, selected, new_selected, currentTeam));
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
                SetTeam(coords, currentTeam);
                actionHistories.Add(new ActionHistory(Action.Attack, new_selected, coords, currentTeam));
            }
        }
    }

    public bool IsValidMove(HexCoordinates selected, HexCoordinates new_selected)
    {
        return  CheckIfCoordsIsValid(selected) && CheckIfCoordsIsValid(new_selected) && 
                HasPiece(selected) && !HasPiece(new_selected) && 
                HexCoordinates.Distance(selected, new_selected) <= 2;
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
