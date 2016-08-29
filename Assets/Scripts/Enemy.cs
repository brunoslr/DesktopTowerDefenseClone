using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

/// <summary>
/// This class contains the enemy information and behavior , methods to move, receive damage
/// </summary>
public class Enemy : MonoBehaviour {


    public int tileX;
    public int tileY;
    public int health = 20;
    public int goldValue = 2;

    public GameObject healthPrefab;
    private Slider healthSlider;

    private Vector3 healthBarOffset;
    public float speed = 0.1f ;

    public List<Node> currentPath = null;
    
    void Start()
    {
        setupHealthSlider();
    }

    void Update()
    {
        MoveEnemy();
        
        if (currentPath == null && gameObject != null)
        {
            DealDamagetoPlayer();
            DestroyEnemy();
        }
    }

    private void MoveEnemy()
    {
        if (Vector3.Distance(transform.position, TileMap.instance.TileCoordToWorldCoord(tileX, tileY)) < 0.05f)
            AdvanceToNextTile();

        transform.position = Vector3.Lerp(transform.position, TileMap.instance.TileCoordToWorldCoord(tileX, tileY), speed * Time.deltaTime);
    }

    public void AdvanceToNextTile()
    {
        if (currentPath == null)
            return;
           
        tileX = currentPath[1].x;
        tileY = currentPath[1].y;
        
        currentPath.RemoveAt(0);

        if (currentPath.Count == 1)
        {
            currentPath = null;
        }
        
    }

    /// <summary>
    /// Setup the slider by instantiation the healthCanvas prefab and 
    /// setting the slider as the current max value of the enemy
    /// </summary>
    private void setupHealthSlider()
    {
        
        GameObject healthCanvas = (GameObject)Instantiate(healthPrefab, 
                                    transform.position, Quaternion.identity);
        healthCanvas.transform.SetParent(transform, false);
        healthCanvas.transform.position = transform.position;
        healthSlider = GetComponentInChildren<Slider>();
        if (healthSlider != null)
        {
            healthSlider.maxValue = health;
            healthSlider.value = health;
        }
    }

    private void SyncHealthSlider()
    {
        if (healthSlider != null)
        {
            healthSlider.value = health;
        };
    }

    internal void ReceiveDamage(int damage)
    {
        health -= damage;
        SyncHealthSlider();

        if (health <= 0)
        {
            GameManager.instance.UpdateCurrentGold(goldValue);
            DestroyEnemy();
        };
    }

    private void DealDamagetoPlayer()
    {
        GameManager.instance.Damage();
    }


    private void DestroyEnemy()
    {
        
        Destroy(gameObject);

    }

    /// <summary>
    /// Test function to check the found path
    /// </summary>
    public void  DrawPathGizmo()
    {
        if (currentPath != null)
        {
            
            //Desenha o caminho do ponto atual ate o destino
            int currNode = 0;
            while(currNode < currentPath.Count-1)
            {
                Vector3 start = TileMap.instance.TileCoordToWorldCoord(currentPath[currNode].x, currentPath[currNode].y) + new Vector3(0,0,-1);
                Vector3 end = TileMap.instance.TileCoordToWorldCoord(currentPath[currNode+1].x, currentPath[currNode+1].y) +  new Vector3(0, 0, -1);
                
                Debug.DrawLine(start, end, Color.red);
                currNode++;

            }
         
        }

    }

}
