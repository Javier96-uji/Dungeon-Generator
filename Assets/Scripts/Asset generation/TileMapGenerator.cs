using System;
using System.Collections.Generic;
using System.Text;
using NaughtyAttributes;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(3)]
public class TileMapGenerator : MonoBehaviour
{

    [SerializeField]
    private UnityEvent onGenerateTileMap;

    [SerializeField]
    private DungeonGenerator dungeonGenerator;

    private int[,] _tileMap;

    private void Start()
    {
        dungeonGenerator = GetComponent<DungeonGenerator>();
    }

    [Button]
    public void GenerateTileMap()
    {
        RectInt bounds = GetDungeonBounds();
        int[,] tileMap = new int[bounds.height, bounds.width];

        foreach (var room in dungeonGenerator.rooms)
        {
            for (int x = room.xMin; x < room.xMax; x++)
            {
                for (int y = room.yMin; y < room.yMax; y++)
                {
                    bool isBorder = x == room.xMin || x == room.xMax - 1 || y == room.yMin || y == room.yMax - 1;
                    if (isBorder)
                        tileMap[y - bounds.yMin, x - bounds.xMin] = 1;
                }
            }
        }

        foreach (var door in dungeonGenerator.doors)
        {
            for (int x = door.xMin; x < door.xMax; x++)
            {
                for (int y = door.yMin; y < door.yMax; y++)
                {
                    tileMap[y - bounds.yMin, x - bounds.xMin] = 2;
                }
            }
        }

        _tileMap = tileMap;
        onGenerateTileMap.Invoke();
    }

    public string ToString(bool flip)
    {
        if (_tileMap == null) return "Tile map not generated yet.";

        int rows = _tileMap.GetLength(0);
        int cols = _tileMap.GetLength(1);
        var sb = new StringBuilder();

        int start = flip ? rows - 1 : 0;
        int end = flip ? -1 : rows;
        int step = flip ? -1 : 1;

        for (int i = start; i != end; i += step)
        {
            for (int j = 0; j < cols; j++)
            {
                sb.Append((_tileMap[i, j] == 0 ? '0' : '#'));
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }

    public int[,] GetTileMap()
    {
        return _tileMap.Clone() as int[,];
    }

    [Button]
    public void PrintTileMap()
    {
        Debug.Log(ToString(true));
    }

    public RectInt GetDungeonBounds()
    {
        int minX = int.MaxValue, maxX = int.MinValue;
        int minY = int.MaxValue, maxY = int.MinValue;

        foreach (var room in dungeonGenerator.rooms)
        {
            if (room.xMin < minX) minX = room.xMin;
            if (room.xMax > maxX) maxX = room.xMax;
            if (room.yMin < minY) minY = room.yMin;
            if (room.yMax > maxY) maxY = room.yMax;
        }

        return new RectInt(minX, minY, maxX - minX, maxY - minY);
    }
}