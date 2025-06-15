using System.Collections.Generic;
using UnityEngine;

public class DungeonGraph : Graph<RectInt>
{
    public void Creategraph(List<RectInt> rooms, List<RectInt> doors)
    {
        foreach (RectInt room in rooms)//Gives a node to every room in the list of rooms
        {
            AddNode(room);
        }


        foreach (RectInt door in doors)
        {
            //Gives a node to every door in the list of doors and creates the edge between the doors and the rooms
            foreach (RectInt room in rooms)
            {
                if (room.Overlaps(door))
                {
                    AddNode(door);
                    AddEdge(room,door);
                }
            }
        }
    }
    public bool IsFullyConnected()
    {
        List<RectInt> nodes = GetNodes();
        if (nodes.Count == 0) return true;

        // Start BFS from the first node
        List<RectInt> visited = new List<RectInt>();
        Queue<RectInt> queue = new Queue<RectInt>();

        queue.Enqueue(nodes[0]);
        visited.Add(nodes[0]);

        while (queue.Count > 0)
        {
            RectInt current = queue.Dequeue();
            foreach (var neighbor in GetNeighbors(current))
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }

        // Log comparison result
        Debug.Log($"Visited nodes: {visited.Count} / Total nodes: {nodes.Count}");

        return visited.Count == nodes.Count;
    }
}
