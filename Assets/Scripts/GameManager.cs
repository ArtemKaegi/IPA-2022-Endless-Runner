using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    #region misc

    private TextMeshProUGUI coinText;
    private Transform lastExit;
    private GameObject player;
    private TextMeshProUGUI scoreText;
    private float defaultMaxSpeed = 20;
    private bool GameStarted = false;
    private int score;
    private BackendConnector connector;

    #endregion

    #region Menu

    private GameObject menu;
    private GameObject skinSelector;
    private GameObject gameOverUi;
    private GameObject gameUi;
    private GameObject newPlayerUi;
    private GameObject highscoreUi;
    private GameObject highscoreUiContent;
    [SerializeField] private TextMeshProUGUI playerNameText;

    #endregion

    #region Camera

    private CameraController camera;
    private GameObject cameraDeathPosition;
    [SerializeField] private float cameraMovementSpeed;
    [SerializeField] private float cameraRotationSpeed;
    [SerializeField] private GameObject cameraDefaultPosition;
    [SerializeField] private GameObject cameraPlayerPosition;
    [SerializeField] private GameObject cameraSkinsPosition;

    #endregion

    #region Tiles

    private float currentTileGenerationZ;

    [Serializable]
    public struct Tile
    {
        public float length;
        public GameObject tileObject;
    }


    [SerializeField] private Tile[] tiles;

    private ArrayList generatedTiles = new ArrayList();

    #endregion

    #region Obstacles

    private float currentObstacleGenerationZ;

    [Serializable]
    public struct Obstacle
    {
        public float length;
        public GameObject obstacleObject;
        public Transform[] exits;
    }


    [SerializeField] private Obstacle[] obstacles;
    [SerializeField] private float firstObstacleDistance;
    [SerializeField] private int lastObstacle;

    private ArrayList generatedObstacles = new ArrayList();

    #endregion

    #region Coins

    private ArrayList generatedCoins = new ArrayList();
    [SerializeField] private GameObject coin;
    [SerializeField] private float coinDistance;
    [SerializeField] private int currentCoins;
    private int allCoins;

    #endregion

    #region Skins

    [SerializeField] private SkinsController skinsController;
    [SerializeField] private GameObject defaultSkin;
    public GameObject skin;

    #endregion

    private void Start()
    {
        allCoins = PlayerPrefs.GetInt("Coins");
        player = GameObject.FindWithTag("Player");
        menu = GameObject.FindWithTag("Menu");
        skinSelector = GameObject.FindWithTag("SkinSelector");
        gameOverUi = GameObject.FindWithTag("GameOver");
        gameUi = GameObject.FindWithTag("GameUI");
        newPlayerUi = GameObject.FindWithTag("NewPlayer");
        highscoreUi = GameObject.FindWithTag("Highscore");
        highscoreUiContent = GameObject.FindWithTag("HighscoreContent");
        coinText = GameObject.FindWithTag("coinText").GetComponent<TextMeshProUGUI>();
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        camera.currentCameraPosition = cameraDefaultPosition;
        camera.cameraMovementSpeed = cameraMovementSpeed;
        camera.cameraRotationSpeed = cameraRotationSpeed;
        player.SetActive(false);
        skinSelector.SetActive(false);
        gameOverUi.SetActive(false);
        gameUi.SetActive(false);
        highscoreUi.SetActive(false);
        scoreText = gameUi.GetComponentInChildren<TextMeshProUGUI>();
        connector = GameObject.FindWithTag("BackendConnector").GetComponent<BackendConnector>();
        if (PlayerPrefs.GetString("PlayerName") == "")
        {
            menu.SetActive(false);
        }
        else
        {
            newPlayerUi.SetActive(false);
        }
        
    }

    #region Backend

    public void CreateNewPlayerBackend()
    {
        string playerName = playerNameText.text;
        string deviceId = SystemInfo.deviceUniqueIdentifier;
        
        PlayerPrefs.SetString("PlayerName", playerName);
        
        connector.AddPlayer(playerName, deviceId);
    }

    public void SetNewHighscore(int highscore)
    {
        string deviceId = SystemInfo.deviceUniqueIdentifier;
        
        connector.setHighscore(deviceId, highscore.ToString());
    }
    
    #endregion

    #region Game Control

    public void StartGame()
    {
        currentObstacleGenerationZ = firstObstacleDistance;
        menu.SetActive(false);
        player.GetComponent<PlayerController>().selectedSkin = skin != null ? skin : defaultSkin;
        player.SetActive(true);
        camera.currentCameraPosition = cameraPlayerPosition;
        GenerateStart();
        GameStarted = true;
        gameUi.SetActive(true);
    }

    private void GenerateStart()
    {
        GenerateRandomTile();
        GenerateRandomTile();
        GenerateRandomTile();
        GenerateRandomTile();
        GenerateRandomTile();
        GenerateRandomTile();
        GenerateRandomTile();
        GenerateRandomTile();
        GenerateRandomTile();
        GenerateRandomTile();
        GenerateRandomTile();
        GenerateRandomTile();
        GenerateRandomTile();
        GenerateRandomTile();
        GenerateRandomTile();
        GenerateRandomTile();
        GenerateRandomTile();
        GenerateRandomTile();
        GenerateRandomTile();
        GenerateRandomTile();
        GenerateRandomObstacle();
        GenerateRandomObstacle();
        GenerateRandomObstacle();
        GenerateRandomObstacle();
        GenerateRandomObstacle();
        GenerateRandomObstacle();
        GenerateRandomObstacle();
        GenerateRandomObstacle();
        GenerateRandomObstacle();
        GenerateRandomObstacle();
        GenerateRandomObstacle();
        GenerateRandomObstacle();
        GenerateRandomObstacle();
        GenerateRandomObstacle();
        GenerateRandomObstacle();
        GenerateRandomObstacle();
        GenerateRandomObstacle();
        GenerateRandomObstacle();
        GenerateRandomObstacle();
    }

    public void GameEnd()
    {
        allCoins += currentCoins;
        PlayerPrefs.SetInt("Coins", allCoins);
        Destroy(cameraDeathPosition);
        cameraDeathPosition = new GameObject();
        var dathPos = cameraDeathPosition.transform.position;
        dathPos = camera.currentCameraPosition.transform.position;
        cameraDeathPosition.transform.rotation = camera.currentCameraPosition.transform.rotation;
        dathPos = new Vector3(dathPos.x, dathPos.y, dathPos.z - 5);
        cameraDeathPosition.transform.position = dathPos;
        camera.currentCameraPosition = cameraDeathPosition;
        gameOverUi.SetActive(true);
        coinText.text = currentCoins.ToString();
        if (GameStarted)
        {
            camera.Shake(-1, 0.3f);
        }
        SetNewHighscore(score);
    }

    private void ResetGame()
    {
        currentCoins = 0;
        player.SetActive(true);
        player.transform.position = player.GetComponent<PlayerController>().startPosition;
        player.GetComponent<PlayerController>().maxSpeed = defaultMaxSpeed;
        player.SetActive(false);
        gameOverUi.SetActive(false);
        gameUi.SetActive(false);
        foreach (GameObject x in generatedTiles)
        {
            Destroy(x);
        }

        foreach (GameObject x in generatedObstacles)
        {
            Destroy(x);
        }

        foreach (GameObject x in generatedCoins)
        {
            Destroy(x);
        }

        generatedTiles = new ArrayList();
        generatedObstacles = new ArrayList();
        generatedCoins = new ArrayList();
        currentObstacleGenerationZ = 0;
        currentTileGenerationZ = 0;
        score = 0;
        scoreText.text = score.ToString();
        GameStarted = false;
    }

    public void Restart()
    {
        ResetGame();
        StartGame();
    }

    #endregion

    #region Level Generation

    public void GenerateRandomTile()
    {
        int i = Random.Range(0, tiles.Length);
        GenerateTile(i);
    }

    private void GenerateTile(int i)
    {
        GameObject x = Instantiate(tiles[i].tileObject,
            transform.position + Vector3.forward * currentTileGenerationZ, transform.rotation,
            null);
        generatedTiles.Add(x);
        currentTileGenerationZ += tiles[i].length;
    }

    public void GenerateRandomObstacle()
    {
        int i = Random.Range(0, obstacles.Length);
        GenerateObstacle(i);
    }

    public void GenerateObstacle(int i)
    {
        GameObject x = Instantiate(obstacles[i].obstacleObject,
            transform.position + Vector3.forward * currentObstacleGenerationZ, transform.rotation,
            null);
        generatedObstacles.Add(x);

        GenerateCoins(i);

        currentObstacleGenerationZ += obstacles[i].length;
        if (GameStarted)
        {
            player.GetComponent<PlayerController>().maxSpeed += 0.1f;

            score += 1;
            scoreText.text = score.ToString();
        }
    }

    private void GenerateCoins(int currentObstacle)
    {
        int amountOfCoins = (int) (obstacles[currentObstacle].length / coinDistance);
        if (generatedObstacles.Count > 2)
        {
            if (lastExit == null)
            {
                lastExit = obstacles[lastObstacle].exits[Random.Range(0, obstacles[lastObstacle].exits.Length)];
            }

            Transform currentExit = obstacles[currentObstacle]
                .exits[Random.Range(0, obstacles[currentObstacle].exits.Length)];
            if (1 <= Mathf.Abs(lastExit.position.y - currentExit.position.y))
            {
                for (int currentCoin = 0; currentCoin < amountOfCoins / 2; currentCoin++)
                {
                    generatedCoins.Add(Instantiate(coin,
                        new Vector3(lastExit.position.x, lastExit.position.y,
                            (currentObstacleGenerationZ - obstacles[lastObstacle].length) +
                            coinDistance * currentCoin), transform.rotation, null));
                }

                for (int currentCoin = 0; currentCoin < amountOfCoins / 2; currentCoin++)
                {
                    generatedCoins.Add(Instantiate(coin,
                        new Vector3(currentExit.position.x, currentExit.position.y,
                            (currentObstacleGenerationZ - obstacles[lastObstacle].length) +
                            coinDistance * (currentCoin + amountOfCoins / 2)),
                        transform.rotation, null));
                }
            }
            else
            {
                for (int currentCoin = 0; currentCoin < amountOfCoins / 3; currentCoin++)
                {
                    generatedCoins.Add(Instantiate(coin,
                        new Vector3(lastExit.position.x, lastExit.position.y,
                            (currentObstacleGenerationZ - obstacles[lastObstacle].length) + coinDistance * currentCoin),
                        transform.rotation, null));
                }

                if (1 < Mathf.Abs(lastExit.position.x - currentExit.position.x))
                {
                    for (int currentCoin = 0; currentCoin < amountOfCoins / 3; currentCoin++)
                    {
                        generatedCoins.Add(Instantiate(coin,
                            new Vector3(lastExit.position.x, lastExit.position.y,
                                (currentObstacleGenerationZ - obstacles[lastObstacle].length) +
                                coinDistance * (currentCoin + amountOfCoins / 3)), transform.rotation, null));
                    }
                }
                else
                {
                    for (int currentCoin = 0; currentCoin < amountOfCoins / 3; currentCoin++)
                    {
                        generatedCoins.Add(Instantiate(coin,
                            new Vector3(lastExit.position.x,
                                player.GetComponent<PlayerController>().startPosition.y + 1.5f,
                                (currentObstacleGenerationZ - obstacles[lastObstacle].length) +
                                coinDistance * (currentCoin + amountOfCoins / 3)), transform.rotation, null));
                    }
                }

                for (int currentCoin = 0; currentCoin < amountOfCoins / 3 + 1; currentCoin++)
                {
                    generatedCoins.Add(Instantiate(coin,
                        new Vector3(currentExit.position.x, currentExit.position.y,
                            (currentObstacleGenerationZ - obstacles[lastObstacle].length) +
                            coinDistance * (currentCoin + (amountOfCoins / 3) * 2)), transform.rotation, null));
                }
            }

            lastExit = currentExit;
        }

        lastObstacle = currentObstacle;
    }

    #endregion

    #region Menu Navigation

    public void ToSkinsFromMenu()
    {
        camera.currentCameraPosition = cameraSkinsPosition;
        menu.SetActive(false);
        skinSelector.SetActive(true);
        skinsController.UpdateCoinsText();
        skinsController.UpdateBuyEquipText();
    }

    public void ToMenuFromSkins()
    {
        camera.currentCameraPosition = cameraDefaultPosition;
        menu.SetActive(true);
        skinSelector.SetActive(false);
    }

    public void ToMenuFromGameOver()
    {
        ResetGame();
        gameOverUi.SetActive(false);
        menu.SetActive(true);
        camera.currentCameraPosition = cameraDefaultPosition;
    }

    public void ToHighscoreFromMenu()
    {
        highscoreUi.SetActive(true);
        menu.SetActive(false);
        connector.getHighscores(highscoreUiContent);
    }

    public void ToMenuFromHighscore()
    {
        highscoreUi.SetActive(false);
        menu.SetActive(true);
    }
    #endregion

    #region Coin Controll

    public void AddCoins(int amount)
    {
        currentCoins += amount;
    }

    public int GetCoins()
    {
        return allCoins;
    }

    public void RemoveCoins(int coins)
    {
        allCoins -= coins;
        PlayerPrefs.SetInt("Coins", allCoins);
    }

    #endregion
}