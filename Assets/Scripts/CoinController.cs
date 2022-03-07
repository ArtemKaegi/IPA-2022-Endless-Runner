using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    private GameObject coin;
    [SerializeField] private GameObject death;
    private GameManager gm;
    [SerializeField] private int value;

    void Start()
    {
        gm = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        coin = GetComponentInChildren<Transform>().gameObject;
    }

    void Update()
    {
        coin.transform.Rotate(0, 60 * Time.deltaTime, 0);
    }

    public void Pop()
    {
        gm.AddCoins(value);
        GameObject currentDeath =Instantiate(death, transform.position, transform.rotation, null);
        Destroy(currentDeath, 2);
        Destroy(gameObject);
    }
}