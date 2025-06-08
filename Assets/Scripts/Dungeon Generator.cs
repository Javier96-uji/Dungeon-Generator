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
        
        //Creates the graph with the conetions between doors and rooms
        graph.Creategraph(rooms, doors);

        Debug.Log("Grafo generado con " + rooms.Count + " habitaciones y " + doors.Count + " puertas.");
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

        //Determines whether the shared wall is horizontal or vertical and places a random door within a non shared wall
        
        if ((endX - startX) > (endY - startY)) // Horizontal wall
        {
            int width = endX - startX;
            if (width < 1) return;

            int doorX = (width == 1) ? startX : UnityEngine.Random.Range(startX + 1, endX - 1);
            int doorY = startY;
            if (IsValid(roomA, roomB, doorX, doorY))
            {
                doors.Add(new RectInt(doorX, doorY, doorSize, doorSize));
            }
        }
        else // Vertical wall
        {
            int height = endY - startY;
            if (height < 1) return;

            int doorX = startX;
            int doorY = (height == 1) ? startY : UnityEngine.Random.Range(startY + 1, endY - 1);
            if (IsValid(roomA, roomB, doorX, doorY))
            {
                doors.Add(new RectInt(doorX, doorY, doorSize, doorSize));
            }
        }
        bool IsCornerOf(RectInt room, int x, int y)
        {
            return (x == room.xMin || x == room.xMax - 1) &&
                   (y == room.yMin || y == room.yMax - 1);
        }

        bool IsValid(RectInt roomA, RectInt roomB, int x, int y)
        {
            // Door must not be in the corner of either room
            bool isCorner = (IsCornerOf(roomA, x, y) || IsCornerOf(roomB, x, y));
            return !isCorner;
        }

    }

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