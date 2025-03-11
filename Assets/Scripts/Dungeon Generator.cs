using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public int minRoomSize = 10;
    public int overlap = 1;
    public int doorSize = 1;
    private List<RectInt> rooms = new List<RectInt>();
    private List<RectInt> doors = new List<RectInt>();
    private DungeonGraph graph = new DungeonGraph();

    void Start()
    {
        RectInt initialRoom = new RectInt(0, 0, 100, 100);
        Generator(initialRoom);
        graph.Creategraph(rooms, doors);
    }

    void Generator(RectInt room)
    {
        if (room.width < minRoomSize * 2 && room.height < minRoomSize * 2)
        {
            rooms.Add(room);
            return;
        }

        bool splitVertically = room.width > room.height; // División vertical si es más ancho
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

        // Recursión
        Generator(leftRoom);
        Generator(rightRoom);
        CreateDoor(leftRoom,rightRoom);
    }

    void CreateDoor(RectInt roomA, RectInt roomB)
    {
        int doorX, doorY;

        int startX = Mathf.Max(roomA.xMin, roomB.xMin);
        int endX = Mathf.Min(roomA.xMax, roomB.xMax);
        int startY = Mathf.Max(roomA.yMin, roomB.yMin);
        int endY = Mathf.Min(roomA.yMax, roomB.yMax);

        if ((endX - startX) > (endY - startY)) // Pasillo horizontal
        {
            doorX = UnityEngine.Random.Range(startX + doorSize, endX - doorSize);
            doorY = startY; // Mantener alineacion
        }
        else // Pasillo vertical
        {
            doorX = startX; // Mantener alineacion
            doorY = UnityEngine.Random.Range(startY + doorSize, endY - doorSize);
        }

        RectInt door = new RectInt(doorX, doorY, doorSize, doorSize);
        doors.Add(door);


        /*Codigo de prueba
         * RectInt intersetion = RectInt.Intersect(roomA, roomB);
        if (intersetion.width > 0 && intersetion.height > 0)
        {
            RectInt door;
            if (intersetion.height < intersetion.width)
            {
                int doorX = intersetion.x + intersetion.width + doorSize;
                door = new RectInt(doorX, intersetion.x, doorSize, intersetion.height);
            }
            else
            {
                int doorY = intersetion.y + intersetion.height + doorSize;
                door = new RectInt(intersetion.x, doorY, intersetion.width, doorSize);
            }
            doors.Add(door);
        }*/
    }

    void Update()
    {
        foreach (var room in rooms)
        {
            AlgorithmsUtils.DebugRectInt(room, Color.yellow);
        }

        foreach (var door in doors)
        {
            AlgorithmsUtils.DebugRectInt(door, Color.blue);
        }

        foreach(var room in rooms)
        {
            foreach (var neighbor in graph.GetNeighbors(room))
            {
                Vector3 start = new Vector3(room.center.x, room.center.y, 0);
                Vector3 end = new Vector3(neighbor.center.x, neighbor.center.y, 0);
                Debug.DrawLine(start, end, Color.red);
            }
        }
    }
}