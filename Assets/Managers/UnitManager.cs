using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public TickManager tm;
        
        //[Header("Units")]
        //public UnitMiner unitMiner;
        
        public List<NodeBase> ToMine { get; private set; }
        public List<UnitMiner> Miners { get; private set; }
        public List<UnitSolder> Solders { get; private set; }

        void Start()
        {
            TickManager.OnTick += OnTickUpdate;
            ToMine = new List<NodeBase>();
            Miners = new List<UnitMiner>();
        }

        private void OnTickUpdate(Int64 tick)
        {
            var taskM = ActionMiners();
            //var taskS = ActionSolders();
            Task.WhenAll(taskM);
        }

        private void SortTilesByDistance(List<NodeBase> tiles, NodeBase src)
        {
             tiles.Sort((x, y) =>
            {
                var xDst = (x.Coords.Pos - src.Coords.Pos).SqrMagnitude();
                var yDst = (y.Coords.Pos - src.Coords.Pos).SqrMagnitude();
                var so = xDst.CompareTo(yDst);
                return so;
            });
        }
        
        private async Task ActionMiners()
        {
            var tasks = new List<Task>();
            foreach (var miner in Miners)
            {
                if(miner.currentAction == MinerActions.None)
                {
                    SortTilesByDistance(ToMine, miner.currentTile);
                    //ToMine.Reverse();
                    foreach (var tile in ToMine)
                    {
                        SortTilesByDistance(tile.Neighbors, miner.currentTile);
                        foreach (var neighbor in tile.Neighbors)
                        {
                            if (neighbor.walkable) // and Make sure its the closest one.
                            {
                                tasks.Add(miner.StartMiningWalkTo(tile, neighbor));
                                goto DoneWithMiner;
                            }
                        }
                    }
                }
                DoneWithMiner: ;
            }

            await Task.WhenAll(tasks);
        }

        public List<SimplePath> GetPathParts(UnitsBase unit, NodeBase dest)
        {
            var paths = Pathfinding.FindPath(unit.currentTile, dest);
            // FindPath does not include the tile the unit is on.
            paths.Add(unit.currentTile);
            
            List<SimplePath> parts = new List<SimplePath>();
            Vector2 src = new Vector2();
            Vector2 dst = new Vector2();
            for(var i = paths.Count - 1; i >= 0; i--)
            {
                if (i == paths.Count-1)
                {
                    src = paths[i].Coords.Pos;
                    dst = src;
                    continue;
                }
                
                if (Mathf.Approximately(src.x, dst.x) && Mathf.Approximately(dst.x, paths[i].Coords.Pos.x))
                {
                    dst = paths[i].Coords.Pos;
                }
                else if (Mathf.Approximately(src.y, dst.y) && Mathf.Approximately(dst.y, paths[i].Coords.Pos.y))
                {
                    dst = paths[i].Coords.Pos;
                }
                else
                {
                    parts.Add(new SimplePath{Src = src, Dst = dst});
                    src = dst;
                    dst = paths[i].Coords.Pos;
                }
                if (i == 0)
                {
                    parts.Add(new SimplePath{Src = src, Dst = dst});
                }
                // FIXME: if next to dest finish.
                
            }
            return parts;
        }
        
        public void AddMinable(NodeBase tile)
        {
            if(!ToMine.Exists(x => x.Coords.Pos == tile.Coords.Pos))
            {
                ToMine.Add(tile);
            }
        }
        
        public void RemoveMinable(NodeBase tile)
        {
            ToMine.Remove(tile);
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