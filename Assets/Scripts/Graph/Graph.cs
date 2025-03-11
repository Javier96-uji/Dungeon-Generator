using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Graph<T>
{
    private Dictionary<T, List<T>> adjacencyList;
    public Graph() { adjacencyList = new Dictionary<T, List<T>>(); }
    public void AddNode(T node)
    {
        if (!adjacencyList.ContainsKey(node))
        {
            adjacencyList[node] = new List<T>();
        }
    }
    public void AddEdge(T fromNode, T toNode)
    {
        if (!adjacencyList.ContainsKey(fromNode) || !adjacencyList.ContainsKey(toNode))
        {
            Debug.Log("One or both nodes do not exist in the graph.");
            return;
        }
        adjacencyList[fromNode].Add(toNode);
        adjacencyList[toNode].Add(fromNode);
    }

    public List<T> GetNeighbors(T node)
    {
        if (!adjacencyList.ContainsKey(node))
        {
            Debug.Log("Node does not exist in the graph.");
        }
        return adjacencyList[node];
    }

    public void Printgraph()
    {
        foreach (var kv  in adjacencyList)
        {
            Console.WriteLine("Node "+ kv.Key.ToString()  + "is connnected to");

            foreach (var v in kv.Value)
            {
                Debug.Log(v);
            }
        }
    }

    public void BFS(T v)
    {
        List<T> visited = new List<T>();
        Queue<T> queue = new Queue<T>();
        queue.Enqueue(v);
        visited.Add(v);

        while (queue.Count > 0)
        {
            v = queue.Dequeue();
            Debug.Log(v);
            foreach (var neighbor in GetNeighbors(v))
            {
                if (!visited.Contains(neighbor))
                {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                }
            }
        }
    }
}
