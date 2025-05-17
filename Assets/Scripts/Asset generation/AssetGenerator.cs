using System;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.AI.Navigation;
using UnityEngine;

public class AssetGenerator : MonoBehaviour
{    
    [SerializeField] private GameObject wall;

    [SerializeField] private GameObject floorPrefab;

    [SerializeField] private NavMeshSurface navMeshSurface;

    private List<RectInt> rooms;
    private List<RectInt> doors;
    public void Initialize(List<RectInt> generatedRooms, List<RectInt> generatedDoors)
    {
        rooms = generatedRooms;
        doors = generatedDoors;
    }
    public void GenerateAssets()
    {
         SpawnDungeonAssets();
         BakeNavMesh();
    }
    public void SpawnDungeonAssets()
    {
        GameObject roomsParent = new GameObject("Rooms");

        foreach (var room in rooms)
        {
            GameObject roomParent = new GameObject($"Room_{room.x}_{room.y}");
            roomParent.transform.parent = roomsParent.transform;

            GameObject floor = Instantiate(floorPrefab, new Vector3(room.center.x + 0.5f, 0f, room.center.y + 0.5f), Quaternion.Euler(90, 0, 0), roomParent.transform);
            floor.transform.localScale = new Vector3(room.width, room.height, 1);

            foreach (var pos in GetRoomBorderTiles(room))
            {
                if (!IsDoorAt(pos))
                {
                    Vector3 position = new Vector3(pos.x + 0.5f, 1f, pos.y + 0.5f);
                    Instantiate(wall, position, Quaternion.identity, roomParent.transform);
                }
            }
        }
        BakeNavMesh();
    }

    private IEnumerable<Vector2Int> GetRoomBorderTiles(RectInt room)
    {
        for (int x = room.xMin; x < room.xMax; x++)
        {
            yield return new Vector2Int(x, room.yMin);
            yield return new Vector2Int(x, room.yMax - 1);
        }
        for (int y = room.yMin + 1; y < room.yMax - 1; y++)
        {
            yield return new Vector2Int(room.xMin, y);
            yield return new Vector2Int(room.xMax - 1, y);
        }
    }
    private bool IsDoorAt(Vector2Int position)
    {
        foreach (var door in doors)
        {
            if (door.Contains(position))
                return true;
        }
        return false;
    }
    private void BakeNavMesh()
    {
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
        }
        else
        {
            Debug.LogError("NavMeshSurface reference is missing!");
        }
    }
}
