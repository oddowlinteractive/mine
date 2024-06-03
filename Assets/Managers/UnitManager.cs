using System;
using System.Collections.Generic;
using Scripts.Tiles;
using Units;
using Unity.VisualScripting;
using UnityEngine;

namespace Managers
{

    public class UnitManager : MonoBehaviour
    {
        
        [Header("Managers")]
        public GridManager gm;
        
        //[Header("Units")]
        //public UnitMiner unitMiner;
        
        public Dictionary<Vector2, NodeBase> ToMine { get; private set; }
        public List<UnitMiner> Miners { get; private set; }

        void Start()
        {
            ToMine = new Dictionary<Vector2, NodeBase>();
            Miners = new List<UnitMiner>();
        }

        private void Update()
        {
            foreach (var miner in Miners)
            {
                if (miner.currentAction == MinerActions.None)
                {
                    foreach (var item in ToMine)
                    {
                        foreach (var neighbor in item.Value.Neighbors)
                        {
                            if (neighbor.walkable)
                            {
                                var pos = neighbor.transform.position;
                                miner.transform.position = new Vector3(pos.x + 0.5f , miner.transform.position.y, 0);
                            }
                        }
                    }
                }
            }
        }

        public void AddMinable(NodeBase tile)
        {
            ToMine.Add(tile.Coords.Pos, tile);
        }
        
        public void RemoveMinable(NodeBase tile)
        {
            ToMine.Remove(tile.Coords.Pos);
        }

        public void AddMiner(UnitMiner miner)
        {
            Miners.Add(miner);
        }
    }
}