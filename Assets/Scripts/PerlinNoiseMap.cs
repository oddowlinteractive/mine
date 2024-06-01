using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/* Danndx 2021 (youtube.com/danndx)
From video: youtu.be/qNZ-0-7WuS8
thanks - delete me! :) */
 
public class PerlinNoiseMap : MonoBehaviour
{
    Dictionary<int, GameObject> tileset;
    Dictionary<int, GameObject> tile_groups;
    public GameObject prefab_plains;
    public GameObject prefab_forest;
    public GameObject prefab_hills;
    public GameObject prefab_mountains;
    public GameObject prefab_sky;
    public GameObject prefab_dirt;
    public Tilemap tilemap;
 
    public int map_width = 128;
    public int map_height = 128;
 
    List<List<int>> noise_grid = new List<List<int>>();
    List<List<GameObject>> tile_grid = new List<List<GameObject>>();
 
    // recommend 4 to 20
    float magnification = 20.0f;
 
    int x_offset = 0; // <- +>
    int y_offset = 0; // v- +^
 
    void Start()
    {
        CreateTileset();
        CreateTileGroups();
        GenerateMap();
        CreateOutside();
    }

    void CreateOutside()
    {
        for (int y = 0; y < 20; y++)
        {
            for (int x = 0; x < 50; x++)
            {
               CreateTile(4, x, map_height + y ); 
            }
        }

        for (int x = 0; x < 50; x++)
        {
            Debug.Log("X: " + x + "Y: " + map_height);
               CreateTile(5, x, map_height - 1); 
        }
    }
 
    void CreateTileset()
    {
        /** Collect and assign ID codes to the tile prefabs, for ease of access.
            Best ordered to match land elevation. **/
 
        tileset = new Dictionary<int, GameObject>();
        tileset.Add(0, prefab_plains);
        tileset.Add(1, prefab_forest);
        tileset.Add(2, prefab_hills);
        tileset.Add(3, prefab_mountains);
        tileset.Add(4, prefab_sky);
        tileset.Add(5, prefab_dirt);
    }
 
    void CreateTileGroups()
    {
        /** Create empty gameobjects for grouping tiles of the same type, ie
            forest tiles **/
 
        tile_groups = new Dictionary<int, GameObject>();
        foreach(KeyValuePair<int, GameObject> prefab_pair in tileset)
        {
            GameObject tile_group = new GameObject(prefab_pair.Value.name);
            tile_group.transform.parent = gameObject.transform;
            tile_group.transform.localPosition = new Vector3(0, 0, 0);
            tile_groups.Add(prefab_pair.Key, tile_group);
        }
    }
 
    void GenerateMap()
    {
        /** Generate a 2D grid using the Perlin noise fuction, storing it as
            both raw ID values and tile gameobjects **/
 
        for(int x = 0; x < map_width; x++)
        {
            noise_grid.Add(new List<int>());
            tile_grid.Add(new List<GameObject>());
 
            for(int y = 0; y < map_height; y++)
            {
                int tile_id = GetIdUsingPerlin(x, y);
                noise_grid[x].Add(tile_id);
                CreateTile(tile_id, x, y);
            }
        }
    }
 
    int GetIdUsingPerlin(int x, int y)
    {
        /** Using a grid coordinate input, generate a Perlin noise value to be
            converted into a tile ID code. Rescale the normalised Perlin value
            to the number of tiles available. **/
 
        float raw_perlin = Mathf.PerlinNoise(
            (x - x_offset) / magnification,
            (y - y_offset) / magnification
        );
        float clamp_perlin = Mathf.Clamp01(raw_perlin); // Thanks: youtu.be/qNZ-0-7WuS8&lc=UgyoLWkYZxyp1nNc4f94AaABAg
        float scaled_perlin = clamp_perlin * 4;
 
        // Replaced 4 with tileset.Count to make adding tiles easier
        if(scaled_perlin == 4)
        {
            scaled_perlin = (3);
        }
        return Mathf.FloorToInt(scaled_perlin);
    }
 
    void CreateTile(int tile_id, int x, int y)
    {
        /** Creates a new tile using the type id code, group it with common
            tiles, set it's position and store the gameobject. **/
 
        GameObject tile_prefab = tileset[tile_id];
        GameObject tile_group = tile_groups[tile_id];
        GameObject tile = Instantiate(tile_prefab, tile_group.transform);
        tile.name= string.Format("tile_x{0}_y{1}", x, y);
        var cell = tile.GetComponent<Cell>();
        cell.x = x;
        cell.y = y;
        tile.transform.localPosition = new Vector3(x, y, 0);
 
        tile_grid[x].Add(tile);
    }
}