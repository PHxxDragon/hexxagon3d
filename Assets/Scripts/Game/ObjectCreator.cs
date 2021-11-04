using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCreator : MonoBehaviour
{
    [SerializeField] private GameObject[] tilesPrefab;

    [SerializeField] private GameObject[] piecesPrefab;

    private Dictionary<string, GameObject> nameToTileDict = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> nameToPieceDict = new Dictionary<string, GameObject>();

    void Awake()
    {
        foreach(var tile in tilesPrefab)
        {
            nameToTileDict.Add(tile.name, tile);
        }
        foreach(var piece in piecesPrefab)
        {
            nameToPieceDict.Add(piece.name, piece);
        }
    }

    public GameObject CreateTile(Team team, Vector3 position)
    {
        GameObject prefab = nameToTileDict[team.ToString()];
        GameObject newTile = Instantiate(prefab);
        newTile.transform.position = position;
        return newTile;
    }

    public GameObject CreatePiece(Team team, Vector3 position)
    {
        GameObject prefab = nameToPieceDict[team.ToString()];
        GameObject newPiece = Instantiate(prefab);
        newPiece.transform.position = position;
        return newPiece;
    }
}
