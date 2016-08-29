using UnityEngine;

/// <summary>
/// This class manages the tower to be build on ground tiles 
/// and countains the logic related to placing the towers
/// </summary>
public class BuildManager : MonoBehaviour
{

    public static BuildManager instance;

    public GameObject[] towerPrefabs;

    private GameObject towerToBuild;

    [SerializeField]
    Transform tileCollector;

    private int towerCost;

    private int tileType;

    // Use this for initialization
    void Awake()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;
    }

    void Start()
    {
        towerCost = 10;
        towerToBuild = towerPrefabs[1];
        tileType = 4;//Value defined as tower tile type
    }

    public GameObject GetTowerToBuild()
    {
        return towerToBuild;
    }

    public void SetTower1ToBuild()
    {
        towerToBuild = towerPrefabs[0];
    }

    public void SetTower2ToBuild()
    {
        towerToBuild = towerPrefabs[1];
    }


    public void BuildTowerOnPosition(int x,int y, Vector3 position)
    {
        if (TileMap.instance.CheckViablePath(x,y,tileType)&& !(TileMap.instance.HasEnemiesOnTile(x,y)))
        {
            if (GameManager.instance.GetCurrentGold() >= towerCost)
            {
                GameManager.instance.UpdateCurrentGold(-towerCost);

               
                GameObject go = (GameObject)Instantiate(towerToBuild, position, Quaternion.identity);
                go.transform.SetParent(tileCollector);

                TileMap.instance.UpdateTileInfo(x, y, 4);
                TileMap.instance.UpdateEnemiesPath();
            }
            else
                GameManager.instance.ShowScreenMessage("Ouro insuficiente \n para construir!",1f);
        }
        else
            GameManager.instance.ShowScreenMessage("Local Inválido", 1.5f);
    }

}
