using System.Collections.Generic;
using System;
using Buildings;
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
        [Header("Managers")]
        public MouseManager mm;
        public UnitManager um;
        
        [Header("Game Objects")]
        public Tilemap tileMap;
        public Grid grid;
        
        
        [Header("Tiles")]
        public TileBase plains;
        public TileBase forest;
        public TileBase hills;
        public TileBase mountains;
        public TileBase sky;
        public TileBase dirt;

        
        [Header("Data")]
        [SerializeField] public int mapWidth = 128;
        [SerializeField] public int mapHeight = 128; 
        [SerializeField] private float magnification = 20;
        [SerializeField] private int xOffset = 0; // <- +>
        [SerializeField] private int yOffset = 0; // v- +^
        [SerializeField] private NodeBase nodeBasePrefab; 
        
        private Dictionary<int, TileBase> _tileSet;
        public static GridManager Instance;
       
        public Dictionary<Vector2, NodeBase> Tiles { get; private set; }
        
        private List<List<int>> _noiseGrid = new List<List<int>>();
        private List<List<GameObject>> _tileGrid = new List<List<GameObject>>();
        
        [Header("Temorary Objects")]
        public GameObject unitPrefab;
        public GameObject barracksPrefab;
        
        
        private void Awake()
        {
            Instance = this;
        }

        // Start is called before the first frame update
        void Start() {
            CreateTileset();
            GenerateMap();
            foreach (var tile in Tiles.Values) tile.CacheNeighbors();
            
            //unit.transform.position = new Vector3(5, (float)(127.0 + 9), 0);
            var bSprite = barracksPrefab.GetComponent<SpriteRenderer>();
            //var bLocation = new Vector3(80 - (bSprite.size.x / 2.0f), mapHeight - (bSprite.size.y / 2.0f), 0.0f);
            var bLocation = new Vector3(80 + 0.5f, mapHeight + 1, 0.0f);
            var iBarracks = Instantiate(barracksPrefab, bLocation, Quaternion.identity);
            var b = iBarracks.GetComponent<BuildingBarracks>();
            b.um = um;
            b.gm = this;
        }

        // Update is called once per frame
        void Update() {
            // Mouse over -> highlight tile
            var mData = mm.GetGridPos();
            if(!mData.Pos.Equals(mData.PrevPos)){
                //interactiveMap.SetTile(previousMousePos, null); // Remove old hoverTile
                //interactiveMap.SetTile(mousePos, hoverTile);
                NodeBase uPos;
                bool walking = false;
                if (Tiles.TryGetValue(new Vector2((float)mData.Pos.x, (float)mData.Pos.y), out uPos))
                {
                    walking = uPos.walkable;
                }
                Debug.Log("TILE: " + mData.Pos.x + " " + mData.Pos.y + " " + walking);
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (OnGrid(mData))
                {
                    NodeBase dPos;
                    NodeBase uPos;

                    var clickedTile = GetTile(mData);
                    if (clickedTile.minable)
                    {
                        if (clickedTile.selected)
                        {
                            clickedTile.UnSelect();
                        }
                        else
                        {
                            clickedTile.Select();
                        }
                    }
                    
                    /*
                    if (Tiles.TryGetValue(new Vector2((float)mData.Pos.x, (float)mData.Pos.y), out dPos))
                    {
                             
                        var upos = tileMap.WorldToCell(unitPrefab.transform.position);
                        if (dPos.walkable && Tiles.TryGetValue(new Vector2((float)upos.x, (float)upos.y), out uPos))
                        {
                            unitPrefab.transform.position = new Vector3(mData.Pos.x + 0.5f, mData.Pos.y + 0.2f, 0);
                            var paths = Pathfinding.FindPath(uPos, dPos);
                            Debug.Log("Path length " + paths.Count);
                        }
                    }
                    */
                }
                //var tile = tileMap.GetTile(tpos);
            }
        }

        public NodeBase GetTile(MouseData<Vector3Int> mData)
        {
            NodeBase retVal;
            if (Tiles.TryGetValue(new Vector2((float)mData.Pos.x, (float)mData.Pos.y), out retVal))
            {
                return retVal;
            }
            return null;
        }
        
        public bool OnGrid(MouseData<Vector3Int> mData)
        {
            return (mData.Pos.x > 0 && mData.Pos.x < mapWidth && mData.Pos.y > 0 && mData.Pos.y < mapHeight + 1);
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
                    
                    var tile = Instantiate(nodeBasePrefab, grid.transform);
                    tile.Init(false, true, new SquareCoords{Pos = new Vector2(x, y)});
                    tile.um = um;
                    Tiles.Add(new Vector2(x,y),tile);
                }
            }

            for (int x = 0; x < mapWidth; x++)
            {
                var tile = Instantiate(nodeBasePrefab,grid.transform);
                tile.Init(true, false, new SquareCoords{Pos = new Vector2(x, mapHeight)});
                tile.um = um;
                Tiles.Add(new Vector2(x, mapHeight),tile);
                
            }
                
            for (int x = 0; x < 100; x++)
            {
                tileMap.SetTile(new Vector3Int(x, mapWidth - 1 ,0), _tileSet[5] );
                NodeBase uPos;
                if (Tiles.TryGetValue(new Vector2((float)x, (float)mapWidth - 1), out uPos))
                    uPos.walkable = false;
            }
        }
     
        int GetIdUsingPerlin(int x, int y)
        {
            /* Using a grid coordinate input, generate a Perlin noise value to be
                converted into a tile ID code. Rescale the normalised Perlin value
                to the number of tiles available.
            */
     
            float rawPerlin = Mathf.PerlinNoise(
                (x - xOffset) / magnification,
                (y - yOffset) / magnification
            );
            float clampPerlin = Mathf.Clamp01(rawPerlin); // Thanks: youtu.be/qNZ-0-7WuS8&lc=UgyoLWkYZxyp1nNc4f94AaABAg
            float scaledPerlin = clampPerlin * 4;
     
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