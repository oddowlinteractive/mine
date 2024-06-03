using Managers;
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
        [Header("Managers")]
        public UnitManager um; 
        
        [Header("Unit Data")]
        public new string name;
        public UnitTypes type;
        public int health;
        public int attach;
        public int carryWeight;

        public Sprite unitSprite;
        public SpriteRenderer sr;


    }
}