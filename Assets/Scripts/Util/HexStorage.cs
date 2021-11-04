using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexStorage<T>
{
    T[,] storage;
    private int board_size;
    public HexStorage(int board_size)
    {
        storage = new T[2*board_size + 1, 2*board_size + 1];
        this.board_size = board_size;
    }

    public void put(HexCoordinates coords, T obj)
    {
        (int, int) t = convertHexCoordsToStoreageCoords(coords);
        storage[t.Item1, t.Item2] = obj;
    }

    public T get(HexCoordinates coords)
    {
        (int, int) t = convertHexCoordsToStoreageCoords(coords);
        return storage[t.Item1, t.Item2];
    }

    private (int, int) convertHexCoordsToStoreageCoords(HexCoordinates hexCoordinates)
    {
        return (hexCoordinates.q + board_size, hexCoordinates.r + board_size);
    }
}
