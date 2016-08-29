using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Countais the declaration of the node class used in tileMap to build the grid nodes
/// </summary>
public class Node
{
    public List<Node> neighbours;
    public int x;
    public int y;

    public Node()
    {
        neighbours = new List<Node>();
    }

    public float DistanceTo(Node n)
    {
        return Vector2.Distance(
            new Vector2(x, y),
            new Vector2(n.x, n.y));
    }

}

