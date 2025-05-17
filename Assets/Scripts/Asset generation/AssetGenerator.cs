using System;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.AI.Navigation;
using UnityEngine;

public class AssetGenerator : MonoBehaviour
{    
    [SerializeField] private RectInt dungeonBounds;
    
    [SerializeField] private List<RectInt> rooms = new List<RectInt>();
    
    [SerializeField] private RectInt door;

    [SerializeField] private GameObject wall;

    [SerializeField] private GameObject floorPrefab;

    [SerializeField] private NavMeshSurface navMeshSurface;
    private void Start()
    {
         SpawnDungeonAssets();
         BakeNavMesh();
    }

    [Button]
    public void SpawnDungeonAssets()
    {
        GameObject roomsParent = new GameObject("Rooms");

        foreach (var room in rooms)
        {
            GameObject roomParent = new GameObject($"Room_{room.x}_{room.y}");
            roomParent.transform.parent = roomsParent.transform;

            // spawn floor
            GameObject floor = Instantiate(floorPrefab, new Vector3(room.center.x, 0f, room.center.y),Quaternion.Euler(90, 0, 0), roomParent.transform);
            floor.transform.localScale = new Vector3(room.width, room.height, 1);

            // Spawn wall
            for (int x = 0; x < room.width; x++)
            {
                for (int y = 0; y < room.height; y++)
                {
                    bool isBorder = x == 0 || y == 0 || x == room.width - 1 || y == room.height - 1;
                    bool isDoor = room.x + x == door.x && room.y + y == door.y;

                    if (isBorder && !isDoor)
                    {
                        Vector3 position = new Vector3(room.x + x + 0.5f, 1f, room.y + y + 0.5f);
                        Instantiate(wall, position, Quaternion.identity, roomParent.transform);

                    }
                }
            }
        }
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
