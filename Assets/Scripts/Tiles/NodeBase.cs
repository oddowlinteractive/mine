using System;
using System.Collections.Generic;
using Managers;
using TMPro;
using Units;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Scripts.Tiles {
    public abstract class NodeBase : MonoBehaviour
    {

        [Header("Managers")]
        public UnitManager um;
        public GridManager gm;
        
        [Header("References")]
        [SerializeField] private Color obstacleColor;

        [SerializeField] private Gradient walkableColor;
        [SerializeField] protected SpriteRenderer _renderer;
        public GameObject highlightPrefab;
        public GameObject highlight;
     
        public ICoords Coords;
        public bool walkable;
        public bool minable;
        public bool selected ;
        public UnitMiner miner;
        private Color _defaultColor;
        
        public static event Action<NodeBase> Selected;
        public static event Action<NodeBase> UnSelected;
        
        public float GetDistance(NodeBase other) => Coords.GetDistance(other.Coords); // Helper to reduce noise in pathfinding

        public virtual void Init(bool _walkable, bool _minable, ICoords coords) {
            this.walkable = _walkable;
            this.minable = _minable;
            Coords = coords;

            //_renderer.color = walkable ? walkableColor.Evaluate(Random.Range(0f, 1f)) : obstacleColor;
            //_defaultColor = _renderer.color;


            transform.position = Coords.Pos;
        }

        public void Select()
        {
            selected = true;
            um.AddMinable(this);
            highlight = Instantiate(highlightPrefab, new Vector3(Coords.Pos.x+0.5f, Coords.Pos.y+0.5f, 0), Quaternion.identity);
        }
        
        public void UnSelect()
        {
            selected = false;
            um.RemoveMinable(this);
            Destroy(highlight);
            miner.currentAction = MinerActions.None;
            miner.currentState = MinerStates.Idle;
            miner.tileToMine = null;
            miner.currentTile = gm.GetTile(miner.transform.position);
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