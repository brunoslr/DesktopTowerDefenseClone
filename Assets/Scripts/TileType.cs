using UnityEngine;
using System.Collections;

/// <summary>
/// Class countains the declaration of TileType used on 
/// Djikstra pathfinding to calculate the cost to move
/// </summary>
[System.Serializable]
public class TileType {

    public string name;
    public GameObject tileVisualPrefab;

    public bool isWalkable;


}
