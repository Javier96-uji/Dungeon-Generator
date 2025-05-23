using UnityEngine;
using System.Collections.Generic;

public class FloodfillSpawner : MonoBehaviour
{
    [SerializeField] private TileMapGenerator tileMapGenerator;
    [SerializeField] private DungeonGenerator dungeonGenerator;
    [SerializeField] private GameObject floorPrefab;

    public void SpawnFloodFillFloor()
    {
        int[,] map = tileMapGenerator.GetTileMap();
        RectInt bounds = tileMapGenerator.GetDungeonBounds();
        int rows = map.GetLength(0);
        int cols = map.GetLength(1);

        bool[,] visited = new bool[rows, cols];
        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        RectInt room = dungeonGenerator.rooms[0];
        int cx = (room.xMin + room.xMax) / 2;
        int cy = (room.yMin + room.yMax) / 2;

        Vector2Int start = new Vector2Int(cx - bounds.xMin, cy - bounds.yMin);
        queue.Enqueue(start);
        visited[start.y, start.x] = true;

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            float worldX = bounds.xMin + current.x + 0.5f;
            float worldZ = bounds.yMin + current.y + 0.5f;
            Vector3 spawnPos = new Vector3(worldX, 0f, worldZ);
            Instantiate(floorPrefab, spawnPos, Quaternion.identity, transform);

            foreach (Vector2Int dir in directions)
            {
                Vector2Int next = current + dir;
                if (next.x >= 0 && next.x < cols && next.y >= 0 && next.y < rows)
                {
                    if (!visited[next.y, next.x] && map[next.y, next.x] !=1)
                    {
                        queue.Enqueue(next);
                        visited[next.y, next.x] = true;
                    }
                }
            }
        }
    }
}