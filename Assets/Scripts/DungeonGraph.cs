using System.Collections.Generic;
using UnityEngine;

public class DungeonGraph : Graph<RectInt>
{
    public void Creategraph(List<RectInt> rooms, List<RectInt> doors)
    {
        foreach (RectInt room in rooms)
        {
            AddNode(room);
        }


        foreach (RectInt door in doors)
        {
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
