using UnityEngine;

namespace System.Runtime.CompilerServices
{
    // this is needed to enable the record feature in .NET framework and .NET core <= 3.1 projects
    internal static class IsExternalInit { }
}

namespace Managers
{
    
    public readonly struct MouseData<T>{
        public MouseData(T cur, T prev)
        {
            Pos = cur;
            PrevPos = prev;
        }
        
        public T Pos { get; init; }
        public T PrevPos { get; init; }
    
    }
    public class MouseManager : MonoBehaviour
    {
        public Grid grid;
        public Vector3 WorldPos { get; private set; }
        public Vector3 WorldPrevPos { get; private set; }
        public Vector3Int GridPos { get; private set; }
        public Vector3Int GridPrevPos { get; private set; }
        
        void Start()
        {
            // Need to fill this variable befor GetMousePosition is run.
            WorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GridPos = grid.WorldToCell(WorldPos);
            
        }
        void Update()
        {
            GetMousePosition();
        }
        private void GetMousePosition ()
        {
            WorldPrevPos = WorldPos;
            WorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            GridPrevPos = GridPos;
            GridPos = grid.WorldToCell(WorldPos);
        }

        public MouseData<Vector3> GetWorldPos()
        {
            return new MouseData<Vector3>(WorldPos, WorldPrevPos);
        }
        
        public MouseData<Vector3Int> GetGridPos()
        {
            return new MouseData<Vector3Int>(GridPos, GridPrevPos);
        }
    }
}