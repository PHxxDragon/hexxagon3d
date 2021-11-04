using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Board/Layout")]
public class InitialSetup : ScriptableObject
{
    [Serializable]
    private class TileSetup
    {
        public HexCoordinates position;
        public Team team;
    }

    [SerializeField] private TileSetup[] tileSetups;

    public int GetPiecesCount()
    {
        return tileSetups.Length;
    }

    public HexCoordinates getCoordsAtIndex(int index)
    {
        return tileSetups[index].position;
    }

    public Team getTeamAtIndex(int index)
    {
        return tileSetups[index].team;
    }
}
