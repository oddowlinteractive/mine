using Managers;
using UnityEngine;

namespace Buildings
{
    public enum BuildingTypes
    {
        Barracks,
        Storage,
        Research,
        MachineGun,
        AntiAir,
        Bulkhead,
        Corridor,
    }

    public abstract class BuildingBase : MonoBehaviour
    {
        [Header("Managers")]
        //FIXME set in init
        public GridManager _gm;
        public UnitManager _um;
        
        [Header("Building Data")]
        public new string name;
        public BuildingTypes type;
        public int health;
        public int attack;

        public SpriteRenderer sr;

    }
}