using System;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public int minRoomSize = 10;
    public int overlap = 1;
    public int doorSize = 1;
    public List<RectInt> rooms = new List<RectInt>();
    public List<RectInt> doors = new List<RectInt>();
    private DungeonGraph graph = new DungeonGraph();

    //I removed the Start() because it was generating the rooms after TileMapGenerator
    public void GenerateDungeon()
    {
        Debug.Log("Generating Dungeon...");
        RectInt initialRoom = new RectInt(0, 0, 100, 100);
        
        //Creates the first room and splits it recursively up to the specified limit
        Generator(initialRoom);
        
        //Creates the high level graph with the conetions between doors and rooms
        graph.Creategraph(rooms, doors);

        //check again if is fully connected
        bool connected = graph.IsFullyConnected();
        Debug.Log("Graph fully connected: " + connected);
        graph.Printgraph();

        var assetGenerator = FindFirstObjectByType<AssetGenerator>();
        if (assetGenerator != null)
        {
            assetGenerator.Initialize(rooms, doors);// Pasamos los datos lógicos
            assetGenerator.GenerateAssets();// Le decimos que instancie cosas
        }
    }

    void Generator(RectInt room)
    {
        //Checks if it's posible to create rooms
        if (room.width < minRoomSize * 2 && room.height < minRoomSize * 2)
        {
            rooms.Add(room);
            return;
        }

        bool splitVertically = room.width > room.height; // Vertical split if its width is bigger
        RectInt leftRoom, rightRoom;

        if (splitVertically)
        {
            int splitX = room.x + UnityEngine.Random.Range(minRoomSize, room.width - minRoomSize);
            leftRoom = new RectInt(room.x, room.y, splitX - room.x, room.height);
            rightRoom = new RectInt(splitX - overlap, room.y, room.width - leftRoom.width + overlap, room.height);
        }
        else
        {
            int splitY = room.y + UnityEngine.Random.Range(minRoomSize, room.height - minRoomSize);
            leftRoom = new RectInt(room.x, room.y, room.width, splitY - room.y);
            rightRoom = new RectInt(room.x, splitY - overlap, room.width, room.height - leftRoom.height + overlap);
        }

        // The process repeats due to recursion
        Generator(leftRoom);
        Generator(rightRoom);
        CreateDoor(leftRoom,rightRoom); //After the map is done, the doors are created
    }

    void CreateDoor(RectInt roomA, RectInt roomB)
    {
        int startX = Mathf.Max(roomA.xMin, roomB.xMin);
        int endX = Mathf.Min(roomA.xMax, roomB.xMax);
        int startY = Mathf.Max(roomA.yMin, roomB.yMin);
        int endY = Mathf.Min(roomA.yMax, roomB.yMax);

        if (startX >= endX || startY >= endY)
            return;

        //If it's a vertical wall
        if (endX - startX >= 1 && endY - startY == 1)
        {
            List<int> validX = new List<int>();
            for (int x = startX + 1; x < endX - 1; x++) // avoid corners
            {
                int roomCount = CountRoomsAt(x, startY);//Check if the tile belongs to both rooms
                if (roomCount == 2)
                    validX.Add(x);
            }

            if (validX.Count > 0)//If valid positions, randomly place the door
            {
                int doorX = validX[UnityEngine.Random.Range(0, validX.Count)];
                doors.Add(new RectInt(doorX, startY, 1, 1));
            }
        }
        // if it's an horizontal wall
        else if (endY - startY >= 1 && endX - startX == 1)
        {
            List<int> validY = new List<int>();
            for (int y = startY + 1; y < endY - 1; y++) // avoid corners
            {
                //Check if the tile belongs to both rooms
                int roomCount = CountRoomsAt(startX, y);
                if (roomCount == 2)
                    validY.Add(y);
            }

            //If valid positions, randomly place the door
            if (validY.Count > 0)
            {
                int doorY = validY[UnityEngine.Random.Range(0, validY.Count)];
                doors.Add(new RectInt(startX, doorY, 1, 1));
            }
        }
    }
    //Checks if the are more the two rooms contected
    int CountRoomsAt(int x, int y)
    {
        int count = 0;
        foreach (var room in rooms)
        {
            if (room.Contains(new Vector2Int(x, y)))
                count++;
        }
        return count;
    }


    //This visualizes the high level graph
    void Update()
    {        
        foreach (var room in rooms)//Draws the rooms (yellow)
        {
            AlgorithmsUtils.DebugRectInt(room, Color.yellow);
        }

        foreach (var door in doors)//Draws the doors (blue)
        {
            AlgorithmsUtils.DebugRectInt(door, Color.blue);
        }

        foreach(var room in rooms)//Draws the routes (red)
        {
            foreach (var neighbor in graph.GetNeighbors(room)) //this checks the neighbors
            {
                Vector3 start = new Vector3(room.center.x, 0, room.center.y);
                Vector3 end = new Vector3(neighbor.center.x, 0, neighbor.center.y);
                Debug.DrawLine(start, end, Color.red);
            }
        }
    }
}

/* This creates doors in the middle of the wall
 int startX = Mathf.Max(roomA.xMin, roomB.xMin);
 int endX = Mathf.Min(roomA.xMax, roomB.xMax);
 int startY = Mathf.Max(roomA.yMin, roomB.yMin);
 int endY = Mathf.Min(roomA.yMax, roomB.yMax);

 if (startX >= endX || startY >= endY)
    return;

 Vector2Int center = new Vector2Int(roomA.xMin + roomA.width / 2,roomA.yMin + roomA.height / 2); // o roomB.center

// Searches for the closets point to the rooms centre
 Vector2Int best = new Vector2Int((startX + endX) / 2, (startY + endY) / 2);
 float bestDist = float.MaxValue;

 for (int y = startY; y < endY; y++)
 {
    for (int x = startX; x < endX; x++)
    {
        Vector2Int p = new Vector2Int(x, y);
        int roomCount = CountRoomsAt(x, y);
        if (roomCount == 2)
        {
            float dist = Vector2Int.Distance(center, p);
            if (dist < bestDist)
            {
                bestDist = dist;
                best = p;
            }
        }
    }
 }
 if (bestDist < float.MaxValue)
    doors.Add(new RectInt(best.x, best.y, 1, 1));
 */