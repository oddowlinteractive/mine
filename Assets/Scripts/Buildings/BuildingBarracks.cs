using System;
using Managers;
using Units;
using UnityEngine;

namespace Buildings
{
    public class BuildingBarracks : BuildingBase
    {
        
        [Header("Units")]
        public UnitMiner unitMiner;
        
        void Update()
        {
        }

        void OnMouseDown()
        {
            Debug.Log("Sprite Clicked");
            GenerateUnit();
        }

        public void GenerateUnit()
        {
            var uLocation = new Vector3(transform.position.x, transform.position.y - .5f , 0.0f);
            var unit = Instantiate(unitMiner, uLocation, Quaternion.identity);
            unit.Init();
            um.AddMiner(unit);
        }
            
    }
}
