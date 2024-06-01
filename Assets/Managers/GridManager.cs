using System.Collections.Generic;
using System;
using Scripts;
using Scripts.Tiles;
using Tiles;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace Managers
{
    public class GridManager : MonoBehaviour
    {
        private Dictionary<int, TileBase> _tileSet;
        public static GridManager Instance;
        private Grid grid;
        
        [SerializeField] private Tilemap interactiveMap = null;
        public TileBase plains;
        public TileBase forest;
        public TileBase hills;
        public TileBase mountains;
        public TileBase sky;
        public TileBase dirt;
        public Tilemap tileMap;
        public GameObject tilesParent;
        public GameObject backGround;
        public GameObject unit;
        public int mapWidth = 128;
        public int mapHeight = 128; 
        [SerializeField] protected NodeBase nodeBasePrefab; 
       
        public Dictionary<Vector2, NodeBase> Tiles { get; private set; }
        
        private List<List<int>> _noiseGrid = new List<List<int>>();
        private List<List<GameObject>> _tileGrid = new List<List<GameObject>>();
        
        private Vector3Int previousMousePos = new Vector3Int();
        
        // recommend 4 to 20
        [SerializeField] private float magnification = .20f;

        [SerializeField] private int xOffset = 0; // <- +>
        [SerializeField] private int yOffset = 0; // v- +^
        
        private void Awake()
        {
            Instance = this;
        }

        // Start is called before the first frame update
        void Start() {
            grid = gameObject.GetComponent<Grid>();
            CreateTileset();
            GenerateMap();
            foreach (var tile in Tiles.Values) tile.CacheNeighbors();
            backGround.transform.position = new Vector3(0,(float)(127.0 + 9.5),0);
            unit.transform.position = new Vector3(5, (float)(127.0 + 9), 0);
        }

        // Update is called once per frame
        void Update() {
            // Mouse over -> highlight tile
            Vector3Int mousePos = GetMousePosition();
            if (!mousePos.Equals(previousMousePos)) {
                //interactiveMap.SetTile(previousMousePos, null); // Remove old hoverTile
                //interactiveMap.SetTile(mousePos, hoverTile);
                NodeBase uPos;
                bool walking = false;
                if (Tiles.TryGetValue(new Vector2((float)mousePos.x, (float)mousePos.y), out uPos))
                {
                    walking = uPos.Walkable;
                }
                previousMousePos = mousePos;
                Debug.Log("TILE: " + mousePos.x + " " + mousePos.y + " " + walking);
            }

            // Left mouse click -> add path tile
            //if (Input.GetMouseButton(0)) {
            //    pathMap.SetTile(mousePos, pathTile);
            //}

            // Right mouse click -> remove path tile
            //if (Input.GetMouseButton(1)) {
            //    pathMap.SetTile(mousePos, null);
            //}
            if (Input.GetMouseButtonDown(0))
            {
                var worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                var tpos = tileMap.WorldToCell(worldPoint);

                // Try to get a tile from cell position
                if (tpos.x > 0 && tpos.x < mapWidth && tpos.y > 0 && tpos.y < mapHeight + 1)
                {
                    Debug.Log("IM IN MAP");
                    NodeBase dPos;
                    NodeBase uPos;
                    if (Tiles.TryGetValue(new Vector2((float)tpos.x, (float)tpos.y), out dPos))
                    {
                             
                        var upos = tileMap.WorldToCell(unit.transform.position);
                        if (dPos.Walkable && Tiles.TryGetValue(new Vector2((float)upos.x, (float)upos.y), out uPos))
                        {
                            unit.transform.position = new Vector3(tpos.x+ 0.5f, tpos.y + 0.2f, 0);
                            var paths = Pathfinding.FindPath(uPos, dPos);
                            Debug.Log("Path length " + paths.Count);
                            //foreach (var path in paths)
                            //{
                            //    unit.transform.position = new Vector3(path.Coords.Pos.x, path.Coords.Pos.y, 0);
                            //    //System.Threading.Thread.Sleep(500);
                            //}
                                
                        }
                    }
                }
                var tile = tileMap.GetTile(tpos);
            }
        }

        Vector3Int GetMousePosition () {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return grid.WorldToCell(mouseWorldPos);
        }

        public NodeBase GetTileAtPosition(Vector2 pos) => Tiles.TryGetValue(pos, out var tile) ? tile : null;
        
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
        void GenerateMap()
        {
            /* Generate a 2D grid using the Perlin noise fuction, storing it as
                both raw ID values and tile gameobjects */
            Tiles = new Dictionary<Vector2, NodeBase>(); 
            for(int x = 0; x < mapWidth; x++)
            {
                _noiseGrid.Add(new List<int>());
                _tileGrid.Add(new List<GameObject>());
     
                for(int y = 0; y < mapHeight; y++)
                {
                    int tileID = GetIdUsingPerlin(x, y);
                    _noiseGrid[x].Add(tileID);
                    tileMap.SetTile(new Vector3Int(x, y ,0), _tileSet[tileID] );
                    
                    var tile = Instantiate(nodeBasePrefab,grid.transform);
                    tile.Init(true, new SquareCoords{Pos = new Vector2(x, y)});
                    Tiles.Add(new Vector2(x,y),tile);
                }
            }

            for (int x = 0; x < mapWidth; x++)
            {
                var tile = Instantiate(nodeBasePrefab,grid.transform);
                tile.Init(true, new SquareCoords{Pos = new Vector2(x, mapHeight)});
                Tiles.Add(new Vector2(x, mapHeight),tile);
                
            }
                
            for (int x = 0; x < 100; x++)
            {
                tileMap.SetTile(new Vector3Int(x, mapWidth - 1 ,0), _tileSet[5] );
                NodeBase uPos;
                if (Tiles.TryGetValue(new Vector2((float)x, (float)mapWidth - 1), out uPos))
                    uPos.Walkable = false;
            }
        }
     
        int GetIdUsingPerlin(int x, int y)
        {
            /* Using a grid coordinate input, generate a Perlin noise value to be
                converted into a tile ID code. Rescale the normalised Perlin value
                to the number of tiles available. */
     
            float rawPerlin = Mathf.PerlinNoise(
                (x - xOffset) / magnification,
                (y - yOffset) / magnification
            );
            float clampPerlin = Mathf.Clamp01(rawPerlin); // Thanks: youtu.be/qNZ-0-7WuS8&lc=UgyoLWkYZxyp1nNc4f94AaABAg
            float scaledPerlin = clampPerlin * 4;
     
            // Replaced 4 with tileset.Count to make adding tiles easier
            if(Mathf.Approximately(scaledPerlin, 4))
            {
                scaledPerlin = (3);
            }
            return Mathf.FloorToInt(scaledPerlin);
        }
        private void OnDrawGizmos() {
            if (!Application.isPlaying) return;
            Gizmos.color = Color.red;
            foreach (var tile in Tiles) {
                if (tile.Value.Connection == null) continue;
                Gizmos.DrawLine((Vector3)tile.Key + new Vector3(0.5f, 0.5f, -1), (Vector3)tile.Value.Connection.Coords.Pos + new Vector3(0.5f, 0.5f, -1));
            }
        }
    }
}