using System.Collections.Generic;
using System.Linq;
using Scripts.Tiles;
using Managers;
using UnityEngine;

namespace Tiles
{
    public class NodeSquare: NodeBase
    {
        private static readonly List<Vector2> Dirs = new List<Vector2>() {
            new Vector2(0, 1), new Vector2(-1, 0), new Vector2(0, -1), new Vector2(1, 0),
            // Don't want diagonal movements
            //new Vector2(1, 1), new Vector2(1, -1), new Vector2(-1, -1), new Vector2(-1, 1)
        };

        public override void CacheNeighbors() {
            Neighbors = new List<NodeBase>();

            foreach (var tile in Dirs.Select(dir => GridManager.Instance.GetTileAtPosition(Coords.Pos + dir)).Where(tile => tile != null)) {
                Neighbors.Add(tile);
            }
        }

        public override void Init(bool walkable, bool minable, ICoords coords) {
            base.Init(walkable, minable, coords);
        
            //_renderer.transform.rotation = Quaternion.Euler(0, 0, 90 * Random.Range(0, 4));
        }
    }

    public struct SquareCoords : ICoords {

        public float GetDistance(ICoords other) {
            var dist = new Vector2Int(Mathf.Abs((int)Pos.x - (int)other.Pos.x), Mathf.Abs((int)Pos.y - (int)other.Pos.y));
            return dist.x + dist.y;
        }

        public Vector2 Pos { get; set; }
    }
}