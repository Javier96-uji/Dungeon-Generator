using System.Collections.Generic;
using UnityEngine;

public class DungeonGraph : Graph<RectInt>
{
    private Graph<RectInt> DunGraph = new Graph<RectInt>();

    public void Creategraph(List<RectInt> rooms, List<RectInt> doors)
    {
        foreach (RectInt room in rooms)
        {
            DunGraph.AddNode(room);
        }

        foreach (RectInt door in doors)
        {
            foreach (RectInt room in rooms)
            {
                if (room.Overlaps(door))
                {
                    DunGraph.AddNode(door);
                    DunGraph.AddEdge(room,door);
                }
            }
        }
    }
}
