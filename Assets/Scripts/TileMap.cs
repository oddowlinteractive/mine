using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

/* Danndx 2021 (youtube.com/danndx)
From video: youtu.be/qNZ-0-7WuS8
thanks - delete me! :) */
 
public class TileMap : MonoBehaviour
{
    private Dictionary<int, TileBase> _tileSet;
    Dictionary<int, GameObject> tile_groups;
    public TileBase plains;
    public TileBase forest;
    public TileBase hills;
    public TileBase mountains;
    public TileBase sky;
    public TileBase dirt;
    public Tilemap tilemap;
    public GameObject backGround;
    public GameObject unit;
 
    public int map_width = 128;
    public int map_height = 128;

    private List<List<int>> _noiseGrid = new List<List<int>>();
    private List<List<GameObject>> _tileGrid = new List<List<GameObject>>();
 
    // recommend 4 to 20
    private float _magnification = 20.0f;

    private int xOffset = 0; // <- +>
    private int yOffset = 0; // v- +^
 
    void Start()
    {
        CreateTileset();
        CreateTileGroups();
        GenerateMap();
        //CreateOutside();
        var bg_x = backGround.transform.position.x;

        backGround.transform.position = new Vector3(0,(float)(127.0 + 9.5),0);
        unit.transform.position = new Vector3(50, (float)(127.0 + 9), 0);
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
        // Collect and assign ID codes to the tile prefabs, for ease of access. Best ordered to match land elevation.
 
        _tileSet = new Dictionary<int, TileBase>();
        _tileSet.Add(0, plains);
        _tileSet.Add(1, forest);
        _tileSet.Add(2, hills);
        _tileSet.Add(3, mountains);
        _tileSet.Add(4, sky);
        _tileSet.Add(5, dirt);
    }
 
    void CreateTileGroups()
    {
        /*
         * Create empty gameobjects for grouping tiles of the same type, ie
         * forest tiles
        */
 
        tile_groups = new Dictionary<int, GameObject>();
        foreach(KeyValuePair<int, TileBase> prefabPair in _tileSet)
        {
            GameObject tileGroup = new GameObject(prefabPair.Value.name);
            tileGroup.transform.parent = gameObject.transform;
            tileGroup.transform.localPosition = new Vector3(0, 0, 0);
            tile_groups.Add(prefabPair.Key, tileGroup);
        }
    }
 
    void GenerateMap()
    {
        /* Generate a 2D grid using the Perlin noise fuction, storing it as
            both raw ID values and tile gameobjects */
 
        for(int x = 0; x < map_width; x++)
        {
            _noiseGrid.Add(new List<int>());
            _tileGrid.Add(new List<GameObject>());
 
            for(int y = 0; y < map_height; y++)
            {
                int tile_id = GetIdUsingPerlin(x, y);
                _noiseGrid[x].Add(tile_id);
                CreateTile(tile_id, x, y);
            }
        }

        for (int x = 0; x < 100; x++)
        {
            CreateTile(5, x, map_width - 1);
            
        }
    }
 
    int GetIdUsingPerlin(int x, int y)
    {
        /* Using a grid coordinate input, generate a Perlin noise value to be
            converted into a tile ID code. Rescale the normalised Perlin value
            to the number of tiles available. */
 
        float raw_perlin = Mathf.PerlinNoise(
            (x - xOffset) / _magnification,
            (y - yOffset) / _magnification
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
        /* Creates a new tile using the type id code, group it with common
            tiles, set it's position and store the gameobject. */
        //TileBase tile = tileset[tile_id];
        //GameObject tile_group = tile_groups[tile_id];
        //GameObject tile = Instantiate(tile, tile_group.transform);
        //tile.name= string.Format("tile_x{0}_y{1}", x, y);
        //var cell = tile.GetComponent<Cell>();
        //cell.x = x;
        //cell.y = y;
        //tile.transform.localPosition = new Vector3(x, y, 0);
 
        //tile_grid[x].Add(tile);
        
        tilemap.SetTile(new Vector3Int(x, y ,0), _tileSet[tile_id] );
    }
}