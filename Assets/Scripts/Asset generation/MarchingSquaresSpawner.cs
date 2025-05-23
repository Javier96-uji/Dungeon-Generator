using UnityEngine;

public class MarchingSquaresSpawner : MonoBehaviour
{
    [SerializeField] private TileMapGenerator tileMapGenerator;
    [SerializeField] private GameObject[] casePrefabs = new GameObject[16];

    public void SpawnFromTileMap()
    {
        int[,] map = tileMapGenerator.GetTileMap();
        int rows = map.GetLength(0);
        int cols = map.GetLength(1);

        RectInt bounds = tileMapGenerator.GetDungeonBounds();

        for (int y = 0; y < rows - 1; y++)
        {
            for (int x = 0; x < cols - 1; x++)
            {
                //Checks if there is a door
                if (map[y, x] == 2 || map[y, x + 1] == 2 || map[y + 1, x] == 2 || map[y + 1, x + 1] == 2)
                {
                    continue;
                }

                int bl = map[y, x] == 1 ? 1 : 0;
                int br = map[y, x + 1] == 1 ? 1 : 0;
                int tr = map[y + 1, x + 1] == 1 ? 1 : 0;
                int tl = map[y + 1, x] == 1 ? 1 : 0;

                if (bl == 0 && br == 0 && tr == 0 && tl == 0)
                    continue;

                int caseIndex = (br) | (tr << 1) | (tl << 2) | (bl << 3);

                if (casePrefabs[caseIndex] != null)
                {
                    float worldX = bounds.xMin + x + 1f;
                    float worldZ = bounds.yMin + y + 1f;
                    Vector3 spawnPos = new Vector3(worldX, 0f, worldZ);

                    Instantiate(casePrefabs[caseIndex], spawnPos, Quaternion.identity, transform);
                }
            }
        }
    }
}