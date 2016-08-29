using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This class countains the tilemap, tiletypes configuration and the level generation methods, 
/// as well as the enemy path creation and update logic
/// </summary>
public class TileMap : MonoBehaviour {

    public static TileMap instance;

    [SerializeField]
    TileType[] tileTypes;

    [SerializeField]
    Transform tileCollector;

    public string enemyTag = "Enemy";
    public int[,] tiles;
    Node[,] graph;
    
    int mapSizeX = 15;
    int mapSizeY = 13;

    int maxViableCostPath = 300;


    public int startPositionX, startPositionY;
    public int targetPositionX, targetPositionY;
   

    void Awake()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;

        GenerateMapData();
        GenerateMapVisuals();
        GeneratePathFindingGraph();


    }
    /// <summary>
    /// Generates the grid map loaded with the TileType information
    /// </summary>
    void GenerateMapData()
    {
        tiles = new int[mapSizeX, mapSizeY];

        //Initializes empty map setting the outter nodes type as borders 
        //and the inner nodes as ground tiles
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                if (x == 0 || y == 0 || x == (mapSizeX - 1) || y == (mapSizeY - 1))
                    tiles[x, y] = 0;//Border
                else
                    tiles[x, y] = 1;//Ground
            }
        }

        //Defining start and target point of the map 
        startPositionX = 0; 
        startPositionY = (mapSizeY / 2);
        targetPositionX = (mapSizeX - 1); 
        targetPositionY = (mapSizeY / 2);

        tiles[startPositionX, startPositionY] = 2;//Start Point
        tiles[targetPositionX, targetPositionY] = 3;//Target Point

    }

    /// <summary>
    ///  Update the tile Type information for the grid position
    /// passed as parameter
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="tileType">0 border tile, 1 ground tile, 4 tower tile</param>
    public void UpdateTileInfo(int x, int y, int tileType)
    {
        tiles[x, y] = tileType;

    }

    /// <summary>
    /// Instantiate the corresponding tile prefab on the grid position defined by
    /// </summary>
    void GenerateMapVisuals()
    {
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {

                TileType tt = tileTypes[tiles[x, y]];
                GameObject go = (GameObject)Instantiate(tt.tileVisualPrefab, new Vector3(x, y, 0), Quaternion.identity);
                go.transform.SetParent(tileCollector);

                ClickableTile ct = go.GetComponent<ClickableTile>();
                if (ct != null)
                {
                    ct.tileX = x;
                    ct.tileY = y;
                }

            }
        }
    }
  
    /// <summary>
    /// Returns the cost to enter a tile, 1 if the tile is walkable
    /// infinity if it is not walkable 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    float CostToEnterTile(int x, int y)
    {
        TileType tt = tileTypes[tiles[x, y]];
        if (tt.isWalkable)
            return 1;
        else
            return Mathf.Infinity;
    }

    /// <summary>
    /// Generates the Graph used in the pathfinding algorithm
    /// </summary>
    public void GeneratePathFindingGraph()
    {
        graph = new Node[mapSizeX, mapSizeY];

        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                graph[x, y] = new Node();

            }
        }

        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                graph[x, y].x = x;
                graph[x, y].y = y;

                //add the four possible node movements from each tile 
                //could be changed to 8-way to allow diagonal movement 
                if (x > 0)
                    graph[x, y].neighbours.Add(graph[x - 1, y]);
                if (x < mapSizeX - 1)
                    graph[x, y].neighbours.Add(graph[x + 1, y]);
                if (y > 0)
                    graph[x, y].neighbours.Add(graph[x, y - 1]);
                if (y < mapSizeY - 1)
                    graph[x, y].neighbours.Add(graph[x, y + 1]);

            }
        }
    }

    /// <summary>
    /// Receives 2d grid coordinates and returns a Vector3 position
    /// </summary>
    public Vector3 TileCoordToWorldCoord(int x, int y)
    {
        return new Vector3(x, y, 0);
    }


    /// <summary>
    /// Djikstra algorithm used in pathfinding, 
    /// uses the enemy gameobject current grid positionn passed as parameter 
    /// to define the current position and loads a node path to the enemy 
    /// from his current position to the target x and Y
    /// Based on Wiki's Djikstra algorithm: https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm
    /// </summary>
    /// <param name="x">Target position X of the map</param>
    /// <param name="y">Target position Y of the map</param>
    /// <param name="enemy">GameObject whose path will be generated</param>
    /// <returns></returns>
    public float GenerateEnemyPathDjikstra(int x, int y, GameObject enemy)
    {
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript == null)
            return 0;
        enemy.GetComponent<Enemy>().currentPath = null;

        Dictionary <Node, float> dist = new Dictionary<Node, float>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

        List<Node> unvisited = new List<Node>();

        Node source = graph[
            enemyScript.tileX, enemyScript.tileY
            ];

        Node target = graph[x, y];

        dist[source] = 0;
        prev[source] = null;
        
        //initializing pathfinding algorithm
        foreach(Node v in graph)
        {
            if(v!=source)
            {
                dist[v] = Mathf.Infinity;
                prev[v] = null;
            }

            unvisited.Add(v);

        }

        while(unvisited.Count>0)
        {
            //return the unvisited nove with the smallest distance cost
            Node u = null;
            foreach(Node possibleMin in unvisited)
            {
                if( u== null || dist[possibleMin] < dist[u])
                {
                    u = possibleMin;
                }
            }

            //if found the target node, end the search
            if(u==target)
            {
                break;
            }

            unvisited.Remove(u);

            //calculate distance from current node to each neighbour
            foreach(Node v in u.neighbours)
            {
                float alt = dist[u] + CostToEnterTile(v.x, v.y);
                
                if (alt<dist[v])
                {
                    dist[v] = alt;
                    prev[v] = u;
                }
            }
        }

        //case the target has no previous node, there is no path 
        if(prev[target] == null)
        {   
            return float.MaxValue;
        }

        //create a node path from current position to target position
        List<Node> currentPath = new List<Node>();
        Node curr = target;

        while(curr != null)
        {
            currentPath.Add(curr);
            curr = prev[curr];

        }

        currentPath.Reverse();

        //Update enemy path and return path cost as a float
        enemyScript.currentPath = currentPath;
        return dist[target];

    }

    /// <summary>
    /// Djikstra algorithm used in pathfinding, 
    /// same algorithm as above, with some simplifications 
    /// to verify if there is a valid path from Start Node to Target Node
    /// </summary>
    /// <returns></returns>
    public float PathDjikstraStartToTarget()
    {
        Dictionary<Node, float> dist = new Dictionary<Node, float>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

        List<Node> unvisited = new List<Node>();

        Node source = graph[startPositionX, startPositionY];

        Node target = graph[targetPositionX, targetPositionY];

        dist[source] = 0;
        prev[source] = null;

        //initializing pathfinding algorithm
        foreach (Node v in graph)
        {
            if (v != source)
            {
                dist[v] = Mathf.Infinity;
                prev[v] = null;
            }

            unvisited.Add(v);

        }

        while (unvisited.Count > 0)
        {
            //return the unvisited nove with the smallest distance cost
            Node u = null;
            foreach (Node possibleMin in unvisited)
            {
                if (u == null || dist[possibleMin] < dist[u])
                {
                    u = possibleMin;
                }
            }

            if (u == target)
            {
                break;
            }

            unvisited.Remove(u);

            foreach (Node v in u.neighbours)
            {
                float alt = dist[u] + CostToEnterTile(v.x, v.y);

                if (alt < dist[v])
                {
                    dist[v] = alt;
                    prev[v] = u;
                }
            }
        }

        if (prev[target] == null)
        {
            //no valid path found
            return Mathf.Infinity; ;
        }
        
        return dist[target];

    }

    /// <summary>
    /// Method that return a bool that shows if placing a tile in the position X,Y
    /// there will still be a valid path from start position to target position
    /// </summary>
    /// <param name="x">Position X of the tested tile</param>
    /// <param name="y">Position X of the tested tile</param>
    /// <param name="tileType">New tile type - 1 for ground, 4 for tower</param>
    /// <returns></returns>
    public bool CheckViablePath(int x, int y, int tileType)
    {
        int currentTileType = tiles[x, y];

        UpdateTileInfo(x, y, tileType);

        if(PathDjikstraStartToTarget() < maxViableCostPath) 
            return true;

        else
        {
            UpdateTileInfo(x, y, currentTileType);//Rollback the change
            return false;
        }

    }
    /// <summary>
    /// Method that return a bool that shows there is currently an enemy on the tile[x,y]
    /// </summary>
    /// <param name="x">Position X of the tested tile</param>
    /// <param name="y">Position X of the tested tile</param>
    /// <returns></returns>
    internal bool HasEnemiesOnTile(int x, int y)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        foreach (GameObject enemy in enemies)
        {
            if ((enemy.GetComponent<Enemy>() != null)&&(enemy.GetComponent<Enemy>().tileX == x) && (enemy.GetComponent<Enemy>().tileY == y))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Update path for all the enemies alive
    /// </summary>
    public void UpdateEnemiesPath()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        foreach (GameObject enemy in enemies)
        {
            GenerateEnemyPathDjikstra(targetPositionX, targetPositionY, enemy);

        }
    }
}
