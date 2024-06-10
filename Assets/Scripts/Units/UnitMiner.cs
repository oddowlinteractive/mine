using System;
using System.Threading.Tasks;
using DG.Tweening;
using Managers;
using Scripts.Tiles;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Units
{
    public enum MinerStates
    {
        Idle,
        Walking,
        Mining,
    }
    
    public enum MinerActions
    {
        Mining,
        None,
    }
    
    public class UnitMiner : UnitsBase
    {
        public NodeBase tileToMine;
        public MinerStates currentState;
        public MinerActions currentAction;
        public int mineTickCount;
        private Int64 _mineTickCountCurrent;

        public void Init(GridManager gm, UnitManager um)
        {
            _gm = gm;
            _um = um;
            type = UnitTypes.Miner;
            currentState = MinerStates.Idle;
            currentAction = MinerActions.None;
            currentTile = _gm.GetTile(transform.position);

            mineTickCount = 2;
            _mineTickCountCurrent = 0;
            health = 20;
            attack = 20;
            carryWeight = 0;
            TickManager.OnTick += StartMiningMine;

        }

        public async Task StartMiningWalkTo(NodeBase _tileToMine, NodeBase _tileToMineFrom)
        {
            tileToMine = _tileToMine;
            tileToMine.miner = this;
            currentAction = MinerActions.Mining;
            currentState = MinerStates.Walking;
            
            var pathParts = _um.GetPathParts(this, _tileToMineFrom);
            foreach (var part in pathParts)
            {
                await transform.DOMove(new Vector3(part.Dst.x + 0.5f, part.Dst.y + 0.5f, 0 ), 4)
                    .SetEase(Ease.Linear)
                    .SetSpeedBased(true)
                    .AsyncWaitForCompletion();
                currentTile = _gm.GetTile(part.Dst);
            }
            _mineTickCountCurrent = 0;
            currentState = MinerStates.Mining;
        }

        private void StartMiningMine(Int64 tick)
        {
            if (currentState != MinerStates.Mining)
            {
                return;
            }
            
            if (_mineTickCountCurrent == 0)
            {
                _mineTickCountCurrent = tick;
                return;
            }

            if (tick - _mineTickCountCurrent >= mineTickCount)
            {
                _mineTickCountCurrent = 0;
                var tile = _gm.GetTile(tileToMine.Coords.Pos);
                tile.transform.DOShakeScale(0.5f, 0.5f); // FIXME: 
                tileToMine.health -= attack;
                if (tileToMine.health <= 0)
                {
                    tileToMine.Mined();
                }
            }
        }
    }
}
