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
    private float currentTileGenerationZ;
    private float currentObstacleGenerationZ;
    private ArrayList generatedTiles = new ArrayList();
    private ArrayList generatedObstacles = new ArrayList();
    private ArrayList generatedCoins = new ArrayList();
    [SerializeField] private GameObject coin;
    [SerializeField] private float coinDistance;
    [SerializeField] private int currentCoins;
    private int allCoins;
    private TextMeshProUGUI coinText;
    private Transform lastExit;
    private GameObject player;
    private GameObject menu;
    private GameObject skinSelector;
    private GameObject gameOverUi;
    private GameObject gameUi;
    private TextMeshProUGUI scoreText;
    private CameraController camera;
    private GameObject cameraDeathPosition;
    [SerializeField] private float cameraMovementSpeed;
    [SerializeField] private float cameraRotationSpeed;
    [SerializeField] private GameObject cameraDefaultPosition;
    [SerializeField] private GameObject cameraPlayerPosition;
    [SerializeField] private GameObject cameraSkinsPosition;
    [SerializeField] private SkinsController skinsController;
    private bool GameStarted = false;
    private float score;
    [SerializeField]private GameObject defaultSkin;
    public GameObject skin;

    [Serializable]
    public struct Tile
    {
        public float length;
        public GameObject tileObject;
    }

    [Serializable]
    public struct Obstacle
    {
        public float length;
        public GameObject obstacleObject;
        public Transform[] exits;
    }

    [SerializeField] private Tile[] tiles;
    [SerializeField] private Obstacle[] obstacles;
    [SerializeField] private float firstObstacleDistance;
    [SerializeField] private int lastObstacle;

    private void Start()
    {
        allCoins = PlayerPrefs.GetInt("Coins");
        player = GameObject.FindWithTag("Player");
        menu = GameObject.FindWithTag("Menu");
        skinSelector = GameObject.FindWithTag("SkinSelector");
        gameOverUi = GameObject.FindWithTag("GameOver");
        gameUi = GameObject.FindWithTag("GameUI");
        coinText = GameObject.FindWithTag("coinText").GetComponent<TextMeshProUGUI>();
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        camera.currentCameraPosition = cameraDefaultPosition;
        camera.cameraMovementSpeed = cameraMovementSpeed;
        camera.cameraRotationSpeed = cameraRotationSpeed;
        player.SetActive(false);
        skinSelector.SetActive(false);
        gameOverUi.SetActive(false);
        gameUi.SetActive(false);
        scoreText = gameUi.GetComponentInChildren<TextMeshProUGUI>();
    }

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
            if (player.GetComponent<PlayerController>().currentHorizontalPosition == 1)
            {
                score += 1;
            }
            score += 1;
            scoreText.text = score.ToString();
        }
    }

    private void GenerateCoins(int i)
    {
        int amountOfCoins = (int) (obstacles[i].length / coinDistance);
        if (generatedObstacles.Count > 2)
        {
            if (lastExit == null)
            {
                lastExit = obstacles[lastObstacle].exits[Random.Range(0, obstacles[lastObstacle].exits.Length)];
            }

            Transform currentExit = obstacles[i].exits[Random.Range(0, obstacles[i].exits.Length)];
            if (1 <= Mathf.Abs(lastExit.position.y - currentExit.position.y))
            {
                if (1 <= Mathf.Abs(lastExit.position.x - currentExit.position.x))
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

        lastObstacle = i;
    }

    public void AddCoins(int amount)
    {
        currentCoins += amount;
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
    }

    private void ResetGame()
    {
        currentCoins = 0;
        player.SetActive(true);
        player.transform.position = player.GetComponent<PlayerController>().startPosition;
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

    public void ToMenuFromGameOver()
    {
        ResetGame();
        gameOverUi.SetActive(false);
        menu.SetActive(true);
        camera.currentCameraPosition = cameraDefaultPosition;
    }

    public int GetCoins()
    {
        return allCoins;
        Debug.Log("Get Coins: " + allCoins);
    }

    public void RemoveCoins(int coins)
    {
        allCoins -= coins;
        PlayerPrefs.SetInt("Coins", allCoins);
    }
}