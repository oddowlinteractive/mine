using System;
using System.Collections.Generic;
using Managers;
using TMPro;
using Units;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Scripts.Tiles {

    public enum TyleTypes
    {
        Dirt,
        Granite,
        Iron,
        Copper,
        Sand,
    }

    public abstract class NodeBase : MonoBehaviour
    {

        [Header("Managers")]
        public UnitManager _um;
        public GridManager _gm;
        
        [Header("Objects")]
        public GameObject highlightPrefab;
        private GameObject _highlight;
     
        [Header("Data")]
        public int health;
        public bool walkable;
        public bool minable;
        public bool selected;
        public TileBase tile;
        
        [NonSerialized]public UnitMiner miner;
        [NonSerialized]public ICoords Coords;
        
        public static event Action<NodeBase> Selected;
        public static event Action<NodeBase> UnSelected;
        
        public float GetDistance(NodeBase other) => Coords.GetDistance(other.Coords); // Helper to reduce noise in pathfinding

        public virtual void Init(ICoords coords) {
        //     walkable = _walkable;
        //     minable = _minable;
        //     health = _health;
            Coords = coords;
            transform.position= (Vector3)Coords.Pos;
        }

        public void Select()
        {
            selected = true;
            _um.AddMinable(this);
            _highlight = Instantiate(highlightPrefab, new Vector3(Coords.Pos.x+0.5f, Coords.Pos.y+0.5f, 0), Quaternion.identity);
        }
        
        public void UnSelect()
        {
            selected = false;
            _um.RemoveMinable(this);
            Destroy(_highlight);
            if (miner)
            {
                miner.currentAction = MinerActions.None;
                miner.currentState = MinerStates.Idle;
                miner.tileToMine = null;
                miner.currentTile = _gm.GetTile(miner.transform.position);
            }
        }

        public void Mined()
        {
            selected = false;
            _um.RemoveMinable(this);
            Destroy(_highlight);
            walkable = true;
            minable = false;
            _gm.tileMap.SetTile(new Vector3Int((int)Coords.Pos.x, (int)Coords.Pos.y ,0), null );
            
            miner.currentAction = MinerActions.None;
            miner.currentState = MinerStates.Idle;
            miner.tileToMine = null;
            //miner.currentTile = _gm.GetTile(miner.transform.position);
            miner = null;
        }

        #region Pathfinding

        public List<NodeBase> Neighbors { get; protected set; }
        public NodeBase Connection { get; private set; }
        public float G { get; private set; }
        public float H { get; private set; }
        public float F => G + H;

        public abstract void CacheNeighbors();

        public void SetConnection(NodeBase nodeBase) {
            Connection = nodeBase;
        }

        public void SetG(float g) {
            G = g;
        }

        public void SetH(float h) {
            H = h;
        }

        #endregion
    }
}


public interface ICoords {
    public float GetDistance(ICoords other);
    public Vector2 Pos { get; set; }
}