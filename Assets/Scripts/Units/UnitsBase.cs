using Managers;
using Scripts.Tiles;
using UnityEngine;

namespace Units
{
    public enum UnitTypes
    {
        Miner,
        Solder,
        Courier,
        Builder, // ???
    }

    public abstract class UnitsBase : MonoBehaviour
    {
        protected GridManager _gm; 
        protected UnitManager _um; 
        
        [Header("Unit Data")]
        public new string name;
        public UnitTypes type;
        public int health;
        public int attach;
        public int carryWeight;

        public Sprite unitSprite;
        public SpriteRenderer sr;
        public NodeBase currentTile;


    }
}