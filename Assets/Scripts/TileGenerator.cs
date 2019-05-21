using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public static class TileEnum {
    public const float DefalutTile = 0f;
    public const float ObstacleTile = 1f;
    public const float RedTile = 2f;
    public const float BlueTile = 3f;
}

public class TileGenerator : MonoBehaviour
{
    public GameObject tilePrefab;
    public GameObject RedTilePrefab;
    public GameObject BlueTilePrefab;
    public GameObject ObstacleTilePrefab;

    public static bool tileMap_isCreated = false;
    public static float[,,] tileMapInfo;
    private GameObject[,] tileMap;

    private int mapWidth;
    private int mapHeight;

    [SerializeField]
    float tileOffset = 0.45f;

    public void DrawMiniMapTiles() {
        for (int x = 0; x < mapWidth; x++) {
            for (int z = 0; z < mapHeight; z++) {
                if (tileMap[x, z] != null) {
                    Destroy((tileMap[x, z] as GameObject).gameObject);
                }
            }
        }

        if (MapGenerator.mapCreated) {
            mapWidth = MapGenerator.currentMapSize.x;
            mapHeight = MapGenerator.currentMapSize.y;

            tileMap = new GameObject[mapHeight, mapWidth];
            tileMapInfo = new float[BallAgent.agents.Length, mapHeight, mapWidth];

            for (int i = 0; i < BallAgent.agents.Length; i++)
            {
                if (BallAgent.agents[i] != null) {
                    int x_pos = 0;
                    int z_pos = 0;
                    float nearestRedTileDist = (float)(mapWidth * mapHeight);

                    for (int x = 0; x < mapWidth; x++)
                    {
                        for (int z = 0; z < mapHeight; z++)
                        {
                            if (nearestRedTileDist > Vector3.Distance(BallAgent.agents[i].position, MapGenerator.tilePositions[x, z]))
                            {
                                nearestRedTileDist = Vector3.Distance(BallAgent.agents[i].position, MapGenerator.tilePositions[x, z]);
                                x_pos = x;
                                z_pos = z;
                            }
                        }
                    }
                    tileMap[x_pos, z_pos] = Instantiate(RedTilePrefab);
                    tileMapInfo[i, x_pos, z_pos] = TileEnum.RedTile;
                }
            }
            if (BallSight.visible_targets != null) {
                foreach (Transform tf in BallSight.visible_targets)
                {
                    if (tf != null)
                    {
                        int x_pos = 0;
                        int z_pos = 0;
                        float nearestBlueTileDist = (float)(mapWidth * mapHeight);

                        for (int x = 0; x < mapWidth; x++)
                        {
                            for (int z = 0; z < mapHeight; z++)
                            {
                                if (nearestBlueTileDist > Vector3.Distance(tf.position, MapGenerator.tilePositions[x, z]))
                                {
                                    nearestBlueTileDist = Vector3.Distance(tf.position, MapGenerator.tilePositions[x, z]);
                                    x_pos = x;
                                    z_pos = z;
                                }
                            }
                        }
                        tileMap[x_pos, z_pos] = Instantiate(BlueTilePrefab);
                        for (int i = 0; i < BallAgent.agents.Length; i++) {
                            tileMapInfo[i, x_pos, z_pos] = TileEnum.BlueTile;
                        }
                    }
                }
            }
            if (BallSight.visible_obstacles != null)
            {
                foreach (Transform tf in BallSight.visible_obstacles)
                {
                    if (tf != null)
                    {
                        int x_pos = 0;
                        int z_pos = 0;
                        float nearestObstacleTileDist = (float)(mapWidth * mapHeight);

                        for (int x = 0; x < mapWidth; x++)
                        {
                            for (int z = 0; z < mapHeight; z++)
                            {
                                if (nearestObstacleTileDist > Vector3.Distance(tf.position, MapGenerator.tilePositions[x, z]))
                                {
                                    nearestObstacleTileDist = Vector3.Distance(tf.position, MapGenerator.tilePositions[x, z]);
                                    x_pos = x;
                                    z_pos = z;
                                }
                            }
                        }
                        tileMap[x_pos, z_pos] = Instantiate(ObstacleTilePrefab);
                        for (int i = 0; i < BallAgent.agents.Length; i++) {
                            tileMapInfo[i, x_pos, z_pos] = TileEnum.ObstacleTile;
                        }
                    }
                }
            }
            

            for (int x = 0; x < mapWidth; x++)
            {
                for (int z = 0; z < mapHeight; z++)
                {
                    float x_pos = x * tileOffset + MapGenerator.currentMapSize.x;
                    float z_pos = z * tileOffset - MapGenerator.currentMapSize.y / 3;
                    if (tileMap[x, z] == null)
                    {
                        tileMap[x, z] = Instantiate(tilePrefab);
                        for (int i = 0; i < BallAgent.agents.Length; i++) {
                            tileMapInfo[i, x, z] = TileEnum.DefalutTile;
                        }
                    }
                    tileMap[x, z].transform.position = new Vector3(x_pos, 0, z_pos);
                }
            }
            TileGenerator.tileMap_isCreated = true;
        }
    }
}
