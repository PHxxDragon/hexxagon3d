using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Piece
{
    public HexCoordinates occupiedPosition;

    public Team team { get; set; }

    public Piece Copy()
    {
        return new Piece(occupiedPosition.Copy(), team);
    }

    public Piece(HexCoordinates coords, Team team)
    {
        occupiedPosition = coords;
        this.team = team;
    }

    public Piece ()
    {

    }
}
