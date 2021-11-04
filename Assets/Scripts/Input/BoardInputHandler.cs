using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoardObjectManager))]
public class BoardInputHandler : MonoBehaviour, IInputHandler
{
    private BoardObjectManager board;
    private void Awake()
    {
        board = GetComponent<BoardObjectManager>();
    }
    public void ProcessInput(Vector3 inputPosition, GameObject selectedObject, Action callback)
    {
        board.OnSquareSelected(inputPosition);
    }
}

