using System;
using System.Collections.Generic;
using System.Linq;
using Scripts.Tiles;
using UnityEngine;

namespace Scripts {
    /// <summary>
    /// This algorithm is written for readability. Although it would be perfectly fine in 80% of games, please
    /// don't use this in an RTS without first applying some optimization mentioned in the video: https://youtu.be/i0x5fj4PqP4
    /// If you enjoyed the explanation, be sure to subscribe!
    ///
    /// Also, setting colors and text on each hex affects performance, so removing that will also improve it marginally.
    /// </summary>
    public static class Pathfinding {
        
        public static List<NodeBase> FindPath(NodeBase startNode, NodeBase targetNode) {
            var toSearch = new List<NodeBase>() { startNode };
            var processed = new List<NodeBase>();

            while (toSearch.Any()) {
                var current = toSearch[0];
                foreach (var t in toSearch) 
                    if (t.F < current.F || t.F == current.F && t.H < current.H) current = t;

                processed.Add(current);
                toSearch.Remove(current);
                
                if (current == targetNode) {
                    var currentPathTile = targetNode;
                    var path = new List<NodeBase>();
                    var count = 100;
                    while (currentPathTile != startNode) {
                        path.Add(currentPathTile);
                        currentPathTile = currentPathTile.Connection;
                        count--;
                        if (count < 0) throw new Exception();
                    }
                    
                    Debug.Log("Processed: " + processed.Count);
                    return path;
                }

                foreach (var neighbor in current.Neighbors.Where(t => t.Walkable && !processed.Contains(t))) {
                    var inSearch = toSearch.Contains(neighbor);

                    // Not supporting diagonals so this will always increase by one
                    var costToNeighbor = current.G + 1;

                    if (!inSearch || costToNeighbor < neighbor.G) {
                        neighbor.SetG(costToNeighbor);
                        neighbor.SetConnection(current);

                        if (!inSearch) {
                            neighbor.SetH(neighbor.GetDistance(targetNode));
                            toSearch.Add(neighbor);
                        }
                    }
                }
            }

            Debug.Log("Processed: " + processed.Count);
            return null;
        }
    }
}