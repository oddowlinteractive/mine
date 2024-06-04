using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using Scripts;
using Scripts.Tiles;
using Units;
using Unity.VisualScripting;
using UnityEngine;



namespace Managers
{

    public enum PathDirection
    {
        Horizontal,
        Virtical
    }

    public class UnitManager : MonoBehaviour
    {
        
        [Header("Managers")]
        public GridManager gm;
        
        //[Header("Units")]
        //public UnitMiner unitMiner;
        
        public Dictionary<Vector2, NodeBase> ToMine { get; private set; }
        public List<UnitMiner> Miners { get; private set; }
        public List<UnitSolder> Solders { get; private set; }

        void Start()
        {
            ToMine = new Dictionary<Vector2, NodeBase>();
            Miners = new List<UnitMiner>();
        }

        private async void Update()
        {
            foreach (var miner in Miners)
            {
                if(miner.currentAction == MinerActions.None)
                {
                    foreach (var item in ToMine.ToArray())
                    {
                        foreach (var neighbor in item.Value.Neighbors)
                        {
                            if (neighbor.walkable)
                            {
                                var pos = neighbor.transform.position;
                                //miner.transform.position = new Vector3(pos.x + 0.5f , miner.transform.position.y, 0);
                                item.Value.miner = miner;
                                miner.currentAction = MinerActions.Mining;
                                miner.currentState = MinerStates.Walking;
                                miner.tileToMine = neighbor;
                                var pathParts = GetPathParts(miner, neighbor);
                                foreach (var part in pathParts)
                                {
                                    await miner.transform.DOMove(new Vector3(part.Dst.x + 0.5f, part.Dst.y + 0.5f, 0 ), 4)
                                        .SetEase(Ease.Linear)
                                        .SetSpeedBased(true)
                                        .AsyncWaitForCompletion();
                                }
                            }
                        }
                    }
                }
                else if (miner.currentAction == MinerActions.Mining)
                {
                    //if (!miner.tileToMine.minable)
                    //{
                    //    miner.currentAction = MinerActions.None;
                    //    miner.currentState = MinerStates.Idle;
                    //    miner.tileToMine = null;
                    //    miner.currentTile = gm.GetTile(miner.transform.position);
                    //}
                }
            }
        }

        public List<SimplePath> GetPathParts(UnitsBase unit, NodeBase dest)
        {
            var paths = Pathfinding.FindPath(unit.currentTile, dest);
            List<SimplePath> parts = new List<SimplePath>();
            Vector2 src = new Vector2();
            Vector2 dst = new Vector2();
            PathDirection dir = PathDirection.Horizontal;
            for(var i = paths.Count - 1; i >= 0; i--)
            {
                if (i == paths.Count-1)
                {
                    src = paths[i].Coords.Pos;
                    continue;
                }

                PathDirection nextDir;
                if (Mathf.Approximately(src.x, paths[i].Coords.Pos.x))
                {
                    nextDir = PathDirection.Virtical;
                }
                else if (Mathf.Approximately(src.y, paths[i].Coords.Pos.y))
                {
                    nextDir = PathDirection.Horizontal;
                }
                else
                {
                    // FIXME: this should never happen... right?
                    nextDir = dir;
                }

                if (i == paths.Count-2)
                {
                    dir = nextDir;
                }

                if (nextDir == dir)
                {
                    dst = paths[i].Coords.Pos;
                }
                else
                {
                    parts.Add(new SimplePath{Src = src, Dst = dst});
                    src = paths[i].Coords.Pos;
                }

                if (i == 0)
                {
                    parts.Add(new SimplePath{Src = src, Dst = dst});
                }
                
            }

            return parts;
        }
        
        public void AddMinable(NodeBase tile)
        {
            ToMine.Add(tile.Coords.Pos, tile);
        }
        
        public void RemoveMinable(NodeBase tile)
        {
            ToMine.Remove(tile.Coords.Pos);
        }

        public void AddUnit(UnitMiner unit)
        {
            Miners.Add(unit);
        }
        
        public void AddUnit(UnitSolder unit)
        {
            Solders.Add(unit);
        }
    }
    public struct SimplePath
    {
        public Vector2 Src { get; set; }
        public Vector2 Dst { get; set; }
    }
}