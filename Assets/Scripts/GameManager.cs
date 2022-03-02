using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private float currentGenerationZ;
    private GameObject[] generatedTiles = Array.Empty<GameObject>();
    private GameObject player;
    private GameObject menu;
    private GameObject skinSelector;
    private CameraController camera;
    [SerializeField] private float cameraMovementSpeed;
    [SerializeField] private float cameraRotationSpeed;
    [SerializeField] private GameObject cameraDefaultPosition;
    [SerializeField] private GameObject cameraPlayerPosition;
    [SerializeField] private GameObject cameraSkinsPosition;
    public bool GameStarted = false;

    [Serializable]
    public struct Tile
    {
        public float length;
        public GameObject tileObject;
    }

    [SerializeField] private Tile[] tiles;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        menu = GameObject.FindWithTag("Menu");
        skinSelector = GameObject.FindWithTag("SkinSelector");
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        camera.currentCameraPosition = cameraDefaultPosition;
        camera.cameraMovementSpeed = cameraMovementSpeed;
        camera.cameraRotationSpeed = cameraRotationSpeed;
        player.SetActive(false);
        skinSelector.SetActive(false);
    }

    public void ToSkinsFromMenu()
    {
        camera.currentCameraPosition = cameraSkinsPosition;
        menu.SetActive(false);
        skinSelector.SetActive(true);
    }
    
    public void ToMenuFromSkins()
    {
        camera.currentCameraPosition = cameraDefaultPosition;
        menu.SetActive(true);
        skinSelector.SetActive(false);
    }

    public void StartGame()
    {
        GameStarted = true;
        menu.SetActive(false);
        player.SetActive(true);
        camera.currentCameraPosition = cameraPlayerPosition;
        GenerateStart();
    }

    public void GenerateRandomTile()
    {
        int i = Random.Range(0, tiles.Length - 1);
        GenerateTile(0);
    }

    private void GenerateTile(int i)
    {
        generatedTiles.Append(Instantiate(tiles[i].tileObject,
            transform.position + Vector3.forward * currentGenerationZ, transform.rotation,
            null));
        currentGenerationZ += tiles[i].length;
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
    }
}