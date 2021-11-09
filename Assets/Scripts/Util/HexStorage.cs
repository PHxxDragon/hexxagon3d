using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexStorage<T>
{
    T[,] storage;
    private int board_size;

    public IEnumerable<HexCoordinates> IterateStorage()
    {
        for (int i = 0; i < 2 * board_size + 1; i++)
        {
            for (int j = 0; j < 2 * board_size + 1; j++)
            {
                yield return convertStorageCoordsToHexCoords(i, j);
            }
        }
    }

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

    private HexCoordinates convertStorageCoordsToHexCoords(int a, int b)
    {
        return new HexCoordinates(a - board_size, b - board_size);
    }
}
