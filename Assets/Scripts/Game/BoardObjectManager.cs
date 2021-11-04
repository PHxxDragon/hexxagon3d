using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectCreator))]
[RequireComponent(typeof(TileSelector))]
public class BoardObjectManager : MonoBehaviour
{
    public const int BOARD_SIZE = 5;

    [SerializeField] private GameObject origin;
    [SerializeField] private float radius;
    [SerializeField] private Vector3 pieceOffset;

    private ObjectCreator objectCreator;
    private GameController gameController;

    private TileSelector tileSelector;
    private HexStorage<GameObject> tilesGrid = new HexStorage<GameObject>(BOARD_SIZE);
    private HexStorage<GameObject> piecesGrid = new HexStorage<GameObject>(BOARD_SIZE);
    private HexCoordinates selected;
    private Board board;

    public void SetDependency(GameController gameController)
    {
        this.gameController = gameController;
    }

    public void OnSquareSelected(Vector3 inputPosition)
    {
        if (!gameController.IsPendingInput())
            return;

        HexCoordinates new_selected = CalculateCoordsFromPosition(inputPosition);
        bool hasPiece = board.HasPiece(new_selected);
        if (selected != null)
        {
            if (hasPiece && selected.Equals(new_selected))
                DeselectPiece();
            else if (hasPiece && gameController.IsTeamTurnActive(board.GetTeam(new_selected)))
                SelectPiece(new_selected);
            else if (!hasPiece)
            {
                if (board.IsValidMove(selected, new_selected))
                {
                    board.Move(selected, new_selected);
                    OnUpdateBoard();
                    DeselectPiece();
                } else
                {
                    DeselectPiece();
                }
            }
                
        }
        else
        {
            if (hasPiece && gameController.IsTeamTurnActive(board.GetTeam(new_selected)))
                SelectPiece(new_selected);
        }
    }

    private void OnUpdateBoard()
    {
        var actionHistories = board.GetActionHistories();
        foreach (var history in actionHistories)
        {
            if (history.action == Board.Action.Copy)
            {
                CreatePieceFromTeam(history.end, history.team);
                ChangeTileToTeam(history.end, history.team);
            } 
            else if (history.action == Board.Action.Move)
            {
                piecesGrid.put(history.end, piecesGrid.get(history.start));
                piecesGrid.put(history.start, null);
                piecesGrid.get(history.end).transform.position = convertHexCoordsToTransformPosition(history.end) + pieceOffset;
                ChangeTileToTeam(history.end, history.team);
                ChangeTileToTeam(history.start, Team.Empty);
            } 
            else if (history.action == Board.Action.Attack)
            {
                Destroy(piecesGrid.get(history.end));
                CreatePieceFromTeam(history.end, history.team);
                ChangeTileToTeam(history.end, history.team);
            }
        }
        EndTurn();
    }
    
    public bool HasMove(Team team)
    {
        foreach (HexCoordinates coods in board.IterateBoardPosition())
        {
            if (board.HasPiece(coods) && board.GetTeam(coods) == team && board.GetAvailableMoves(coods).Count > 0)
            {
                return true;
            }
        }
        return false;
    }

    private void EndTurn()
    {
        gameController.EndTurn();
    }

    private void SelectPiece(HexCoordinates coords)
    {
        selected = coords;
        Dictionary<HexCoordinates, bool> selection = board.GetAvailableMoves(selected);
        ShowSelectionSquares(selection);
    }

    private void ShowSelectionSquares(Dictionary<HexCoordinates, bool> selection)
    {
        Dictionary<Vector3, bool> dict = new Dictionary<Vector3, bool>();
        foreach (HexCoordinates coords in selection.Keys)
        {
            Vector3 position = convertHexCoordsToTransformPosition(coords);
            dict.Add(position + pieceOffset, selection[coords]);
        }
        tileSelector.ShowSelection(dict);
    }

    private void DeselectPiece()
    {
        selected = null;
        tileSelector.ClearSelection();
    }

    private GameObject GetPieceOnSquare(HexCoordinates coords)
    {
        if (board.CheckIfCoordsIsValid(coords))
        {
            return piecesGrid.get(coords);
        }
        return null;
    }

    void Awake()
    {
        SetDependencies();
    }

    private void SetDependencies()
    {
        objectCreator = GetComponent<ObjectCreator>();
        tileSelector = GetComponent<TileSelector>();
    }

    public void InitBoardFromSetup(InitialSetup initialSetup)
    {
        board = new Board(BOARD_SIZE, initialSetup);

        foreach (HexCoordinates coords in board.IterateBoardPosition())
        {
            if (board.HasPiece(coords))
            {
                CreatePieceFromTeam(coords, board.GetTeam(coords));
                CreateTileFromTeam(coords, board.GetTeam(coords));
            } else
            {
                CreateTileFromTeam(coords, Team.Empty);
            }
        }
    }

    private void CreatePieceFromTeam(HexCoordinates coords, Team team)
    {
        if (team != Team.Empty && team != Team.Invalid)
        {
            Vector3 newPosition = convertHexCoordsToTransformPosition(coords);
            piecesGrid.put(coords, CreatePiece(team, newPosition));
        }
    }

    private void CreateTileFromTeam(HexCoordinates coords, Team team)
    {
        if (team != Team.Invalid)
        {
            Vector3 newTilePosition = convertHexCoordsToTransformPosition(coords);
            tilesGrid.put(coords, CreateTile(team, newTilePosition));
        }
    }

    private GameObject CreateTile(Team team, Vector3 position)
    {
        return objectCreator.CreateTile(team, position);
    }

    private GameObject CreatePiece(Team team, Vector3 position)
    {
        return objectCreator.CreatePiece(team, position + pieceOffset);
    }

    private void ChangeTileToTeam(HexCoordinates coords, Team team)
    {
        if (tilesGrid.get(coords) != null)
        {
            Destroy(tilesGrid.get(coords));
        }
        CreateTileFromTeam(coords, team);
    }

    private Vector3 convertHexCoordsToTransformPosition(HexCoordinates hexCoordinates)
    {
        Vector3 originPosition = origin.transform.position;
        float x = radius * (Mathf.Sqrt(3) * hexCoordinates.q + Mathf.Sqrt(3) / 2 * hexCoordinates.r);
        float y = radius * (3f / 2 * hexCoordinates.r);
        return new Vector3(originPosition.x + x, originPosition.y, originPosition.z + y);
    }

    private HexCoordinates CalculateCoordsFromPosition(Vector3 inputPosition)
    {
        Vector3 originPosition = origin.transform.position;
        float x = inputPosition.x - originPosition.x;
        float y = inputPosition.z - originPosition.z;
        float q = (Mathf.Sqrt(3) / 3 * x - 1f / 3 * y) / radius;
        float r = 2f / 3 * y / radius;
        Debug.Log((x, y));
        Debug.Log((q, r));
        Debug.Log((Mathf.RoundToInt(q), Mathf.RoundToInt(r)));
        return new HexCoordinates(Mathf.RoundToInt(q), Mathf.RoundToInt(r));
    }

}