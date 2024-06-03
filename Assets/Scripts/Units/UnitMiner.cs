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

        public void Init()
        {
            currentState = MinerStates.Idle;
            currentAction = MinerActions.None;
        }

    }
}
