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
}
