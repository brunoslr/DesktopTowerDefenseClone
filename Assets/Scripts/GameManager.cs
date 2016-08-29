using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

/// <summary>
/// This singleton class that manages the game states and logic 
/// as well as the enemy management and text GUI
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField]
    GameObject[] enemyPrefab;

    [SerializeField]
    Transform enemyCollector;

    private string sceneName = "mainScene";
    private string enemyTag = "Enemy";

    /// <summary>
    /// Vectors for the initial position and target of the enemies
    /// </summary>
    Vector3 spawnPosition;
    Vector3 targetPosition;

    /// <summary>
    /// Control variables for the waves
    /// </summary>
    private float timeBetweenWaves = 10f;
    private float restartTimeOnGameOver = 5f;
    public float spawnDelay = .3f;
    int currentWave = 0; 
    private float countdown = 2f;
    private int numOfEnemiesPerWave = 5;

    private int numOfEnemiesAlive = 0;    

    /// <summary>
    /// UI Elements
    /// </summary>
    public Slider healthSlider;
    public Text currentGoldText;
    public Text timeToNextWaveText;
    public Text waveOutText;
    public Text gameMessageText;
    public Button startBtn;
    public Button extraWabeBtn;

    /// <summary>
    /// Game State variables
    /// </summary>
    bool isGameOver;
    bool infiniteWaveMode;
    bool isSpawning;
    bool hasStarted;
    int health;
    int gold;
    

    public static GameManager instance;
    
    void Awake()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;
                                
        health = 10;
        gold = 100;
        currentWave = 0;

        infiniteWaveMode = false;
        isSpawning = false;
        isGameOver = false;
        hasStarted = false;

        SyncHealthSlider();
        SyncCurrentGold();
        
    }

   void Start()
    {
        spawnPosition = new Vector3(TileMap.instance.startPositionX, TileMap.instance.startPositionY, 0);
        targetPosition = new Vector3(TileMap.instance.targetPositionX, TileMap.instance.targetPositionY, 0);
       
        //Check the number of enemies alive twice per second
        InvokeRepeating("UpdateNumberOfEnemies", 0f, 0.5f);
    }

    void Update()
    {
        if (!isGameOver&&hasStarted)
        {
            if (currentWave < 5 || infiniteWaveMode)
            {
                if (countdown <= 0f)
                {
                    currentWave++;
                    SpawnWave();
                    countdown = timeBetweenWaves;
                }

                countdown -= Time.deltaTime;
                timeToNextWaveText.text = "Wave em: " + (int)countdown + "s";
            }
            else
            {
                timeToNextWaveText.text = string.Empty;
                if (!isSpawning && numOfEnemiesAlive == 0)
                {   // win state
                    GameOver();
                    isGameOver = true;
                }
            }   
        }
    }

    /// <summary>
    /// Start the game and enable/disable buttons accordingly
    /// </summary>
    public void StartGame()
    {
        hasStarted = true;
        
        //Set
        startBtn.interactable=false;
        extraWabeBtn.interactable = true;
    }

    #region EnemiesManagement

    /// <summary>
    /// Instantiate enemy and set its initial path according to the current Map
    /// </summary>
    /// <param name="enemyUnit"></param>
    public void InstantiateEnemy(int enemyUnit)
    {
        GameObject enemy = (GameObject)Instantiate(enemyPrefab[enemyUnit], 
                                spawnPosition, Quaternion.identity);
        enemy.transform.SetParent(enemyCollector);

        enemy.GetComponent<Enemy>().tileX = (int)enemy.transform.position.x;
        enemy.GetComponent<Enemy>().tileY = (int)enemy.transform.position.y;

        TileMap.instance.GenerateEnemyPathDjikstra((int)targetPosition.x, (int)targetPosition.y, enemy);
        
    }

    /// <summary>
    /// Spawns a wave of enemies and update related UI
    /// </summary>
    void SpawnWave()
    {

        isSpawning = true;

        if (!infiniteWaveMode)
            waveOutText.text = "Wave " + (currentWave) + "/5";
        else
            waveOutText.text = "Wave " + (currentWave);

        for (int i = 0; i < numOfEnemiesPerWave; i++)
        {
            Invoke("SpawnEnemy", spawnDelay * i);
        }

        Invoke("SpawningDone", spawnDelay * numOfEnemiesPerWave);

    }

 
    /// <summary>
    /// Spawn Enemy accordingly to the current wave
    /// </summary>
    private void SpawnEnemy()
    {
        
        InstantiateEnemy(currentWave % enemyPrefab.Length);

    }


    private void SpawningDone()
    {
        isSpawning = false;
    }

    /// <summary>
    /// Check number of enemies alive
    /// </summary>
    internal void UpdateNumberOfEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        numOfEnemiesAlive = enemies.Length;

    }

    /// <summary>
    /// Used on the button Extra Wave
    /// </summary>
    public void SpawnExtraWave()
    {
        if (!isGameOver)
        {
            currentWave++;
            SpawnWave();
        }
    }

    #endregion


    #region UI

    /// <summary>
    /// Update slider and deals damage to the player
    /// </summary>
    public void Damage()
    {
        health -= 1;
        SyncHealthSlider();

        if (health <= 0)
        {
            isGameOver = true;
            GameOver();
 
            return;
        }
    }

    /// <summary>
    /// Show a message on game over and restart the level
    /// </summary>
    private void GameOver()
    {
        if (health <= 0)
        {
            gameMessageText.text = "Game Over! Você perdeu =( \n Jogo reinicia em " 
                                        + restartTimeOnGameOver+ " s";
        }
        else
        {
            gameMessageText.text = "Parabéns, você venceu! \n Jogo reinicia em " 
                                         + restartTimeOnGameOver + " s";
        }

        Invoke("RestartScene", restartTimeOnGameOver);

    }

    private void SyncHealthSlider()
    {
        SyncSlider(healthSlider, health);
    }

      private void SyncSlider(Slider slider, int value)
    {
        slider.value = value;
    }

    private void SyncCurrentGold()
    {
        currentGoldText.text = "Ouro: " + gold;
        
    }

    public int GetCurrentGold()
    {
        return gold;
    }

    public void UpdateCurrentGold(int income)
    {
        gold += income;
        SyncCurrentGold();
    }

    /// <summary>
    /// Toggle infinite wave mode and shows a message
    /// </summary>
    public void ToggleInfiniteWaveMode()
    {
        infiniteWaveMode = !infiniteWaveMode;
        if (infiniteWaveMode)
        {
            waveOutText.text = "Wave " + (currentWave);
            gameMessageText.text = "Modo de Waves Infinitas \n   Ligado!";
        }
        else
        {
            waveOutText.text = "Wave " + (currentWave) + "/5";
            gameMessageText.text = "Modo de Waves Infinitas \n   Desligado!";
        }
              

        Invoke("ClearMessage", 2);

    }

    public void ShowScreenMessage(string message, float durationInSeconds )
    {
        gameMessageText.text = message;
       

        Invoke("ClearMessage", durationInSeconds);

    }

    private void ClearMessage()
    {
        gameMessageText.text = string.Empty;

    }

    public void RestartScene()
    {
       SceneManager.LoadScene(sceneName);
    }

    #endregion
}