using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public int minRoomSize = 20;
    public int overlap = 5;
    private List<RectInt> rooms = new List<RectInt>();

    void Start()
    {
        RectInt initialRoom = new RectInt(0, 0, 100, 50);
        Generator(initialRoom);
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
            int splitX = room.x + Random.Range(minRoomSize, room.width - minRoomSize);
            leftRoom = new RectInt(room.x, room.y, splitX - room.x, room.height);
            rightRoom = new RectInt(splitX, room.y, room.width - leftRoom.width, room.height);
        }
        else
        {
            int splitY = room.y + Random.Range(minRoomSize, room.height - minRoomSize);
            leftRoom = new RectInt(room.x, room.y, room.width, splitY - room.y);
            rightRoom = new RectInt(room.x, splitY, room.width, room.height - leftRoom.height);
        }

        // Recursión
            Generator(leftRoom);
            Generator(rightRoom);
    }

    void Update()
    {
        foreach (var room in rooms)
        {
            AlgorithmsUtils.DebugRectInt(room, Color.yellow);
        }
    }
}

//Para la puerta puedo usar intersect o algo así para sacar el rectangulo de la pared y ahi hago un cuadrado que indica la puerta
//De esa forma tengo las coordenadas para coger la puerta cuando tenga que hacer los nodos de la ruta