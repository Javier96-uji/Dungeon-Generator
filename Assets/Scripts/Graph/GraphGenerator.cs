using System;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class GraphGenerator : MonoBehaviour
{
    [SerializeField]private RectInt dungeonBounds;
    [SerializeField] private TileMapGenerator tileMapGenerator;

    private Graph<Vector3> graph = new Graph<Vector3>();
    
    //public GameObject floor;

    [Button]
    public void GenerateGraph()
    {
        if (tileMapGenerator == null)
            tileMapGenerator = FindFirstObjectByType<TileMapGenerator>();

        dungeonBounds = tileMapGenerator.GetDungeonBounds();
        graph.Clear();

        int[,] tileMap = tileMapGenerator.GetTileMap();

        // Connect neighbors
        for (int x = dungeonBounds.xMin; x < dungeonBounds.xMax; x++)
        {
            for (int y = dungeonBounds.yMin; y < dungeonBounds.yMax; y++)
            {
                // Skip walls
                if (tileMap[y - dungeonBounds.yMin, x - dungeonBounds.xMin] == 1)
                    continue;

                Vector3 current = new Vector3(x, 0, y);
                graph.AddNode(current);

                // Connect to 4-neighbors if they're walkable
                foreach (var offset in new[] {
                new Vector2Int(1,0), new Vector2Int(-1,0),
                new Vector2Int(0,1), new Vector2Int(0,-1)
            })
                {
                    int nx = x + offset.x;
                    int ny = y + offset.y;

                    if (nx >= dungeonBounds.xMin && nx < dungeonBounds.xMax &&
                        ny >= dungeonBounds.yMin && ny < dungeonBounds.yMax &&
                        tileMap[ny - dungeonBounds.yMin, nx - dungeonBounds.xMin] != 1)
                    {
                        Vector3 neighbor = new Vector3(nx, 0, ny);
                        graph.AddEdge(current, neighbor);
                    }
                }
            }
        }

        //floor.transform.position = new Vector3( dungeonBounds.center.x - .5f, -.5f, dungeonBounds.center.y - .5f);
        //floor.transform.localScale = new Vector3(dungeonBounds.width, 1, dungeonBounds.height);
        Debug.Log("Nodes generated: " + graph.GetNodes().Count);
    }

    //Checks if a neighbor coordinate is within bounds, then, adds an edge in the graph
    /*private void TryConnectNeighbor(int nx, int ny, Vector3 currentPos)
    {
        // Only connect if neighbor is in bounds
        if (nx >= dungeonBounds.xMin && nx < dungeonBounds.xMax &&
            ny >= dungeonBounds.yMin && ny < dungeonBounds.yMax)
        {
            Vector3 neighborPos = new Vector3(nx, 0, ny);
            graph.AddEdge(currentPos, neighborPos);
        }
    }*/
    void Start()
    {
        var dungeon = FindFirstObjectByType<DungeonGenerator>();
        var tilemap = FindFirstObjectByType<TileMapGenerator>();

        Debug.Log("GraphGenerator Start");

        if (dungeon != null)
        {
            dungeon.GenerateDungeon(); // Make the rooms and doors
            Debug.Log("Dungeon Generated");
        }

        if (tilemap != null)
        {
            tilemap.GenerateTileMap(); // Fill the tile map
            Debug.Log("TileMap Generated");
        }
        GenerateGraph();//Here generates the low level graph
    }

    private void Update() //this visualizes the low level graph
    {
        /*foreach (var node in graph.GetNodes())
        {
            DebugExtension.DebugWireSphere(node, Color.cyan, .2f);
            foreach (var neighbor in graph.GetNeighbors(node))
            {
                Debug.DrawLine(node, neighbor, Color.cyan);
            }
        }*/
    }

    //this is for external access
    public Graph<Vector3> GetGraph()
    {
        return graph;
    }
    
}
//necesito crear un algoritmo que lea los nodos que se ahan creado y los compare con una lista de nodos visitados
//estos comprueban que todos los nodos (habitaciones) se pueden alcanzar desde las otras habitaciones.
//Comprobar que era el high graph y el low graph