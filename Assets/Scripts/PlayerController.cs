using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Serializable]
    public struct positionCluster
    {
        public GameObject[] verticalPositions;
    }

    [SerializeField] private GameManager gm;
    [SerializeField] private GameObject explosionObject;
    [SerializeField] private positionCluster[] horizontalPositions;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float laneSwitchingSpeed;
    [SerializeField] private float jumpTime;
    [SerializeField] private float duckTime;
    [SerializeField] public float maxSpeed;
    [SerializeField] private float acceleration;
    public Vector3 startPosition;
    private Rigidbody rb;
    public Transform currentTransform;
    public int currentHorizontalPosition = 1;
    public int currentVerticalPosition = 1;
    public GameObject selectedSkin;
    private GameObject currentSkin;
    private Vector3 firstPosition;
    private Vector3 lastPosition;
    private float dragDistance;
    private bool isDucking = false;
    private bool isJumping = false;
    private bool moved = false;
    private float countdown = 0;

    void Start()
    {
        currentTransform = horizontalPositions[currentHorizontalPosition].verticalPositions[currentVerticalPosition]
            .transform;
        dragDistance = Screen.height * 3 / 100;
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
    }

    private void OnEnable()
    {
        Destroy(currentSkin);
        currentSkin = Instantiate(selectedSkin, playerTransform.position, playerTransform.rotation, playerTransform);
    }

    void Update()
    {
        if (rb.velocity.magnitude < maxSpeed)
        {
            rb.AddForce(0, 0, acceleration);
        }

        CheckInput();

        playerTransform.position = Vector3.Lerp(playerTransform.position, currentTransform.position,
            laneSwitchingSpeed * Time.deltaTime);

        if (isJumping || isDucking)
        {
            countdown -= Time.deltaTime;
            if (countdown <= 0)
            {
                isDucking = false;
                isJumping = false;
                currentVerticalPosition = 1;
                currentTransform = horizontalPositions[currentHorizontalPosition]
                    .verticalPositions[currentVerticalPosition].transform;
            }
        }
    }

    void CheckInput()
    {
        if (Input.touchCount >= 1)
        {
            CheckTouch(Input.GetTouch(0));
        }
    }

    void CheckTouch(Touch touch)
    {
        switch (touch.phase)
        {
            case TouchPhase.Began:
                firstPosition = touch.position;
                lastPosition = touch.position;
                break;
            case TouchPhase.Moved:

                if (!moved)
                {
                    lastPosition = touch.position;

                    if (Mathf.Abs(lastPosition.x - firstPosition.x) + 20 > dragDistance ||
                        Mathf.Abs(lastPosition.y - firstPosition.y) > dragDistance)
                    {
                        moved = true;
                        if (Mathf.Abs(lastPosition.x - firstPosition.x) > Mathf.Abs(lastPosition.y - firstPosition.y))
                        {
                            if ((lastPosition.x > firstPosition.x))
                            {
                                if (currentHorizontalPosition < horizontalPositions.Length - 1)
                                {
                                    currentHorizontalPosition++;
                                }
                            }
                            else
                            {
                                if (currentHorizontalPosition > 0)
                                {
                                    currentHorizontalPosition--;
                                }
                            }
                        }
                        else
                        {
                            if (lastPosition.y > firstPosition.y)
                            {
                                if (!isJumping && !isDucking)
                                {
                                    StartJump();
                                }
                            }
                            else
                            {
                                if (!isJumping && !isDucking)
                                {
                                    StartDucking();
                                }
                            }
                        }

                        currentTransform = horizontalPositions[currentHorizontalPosition]
                            .verticalPositions[currentVerticalPosition].transform;
                    }
                }

                break;
            case TouchPhase.Ended:
            {
                moved = false;
                break;
            }
        }
    }

    void StartJump()
    {
        isJumping = true;
        countdown = jumpTime;
        currentVerticalPosition = 2;
    }

    void StartDucking()
    {
        isDucking = true;
        countdown = duckTime;
        currentVerticalPosition = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ObstacleTrigger"))
        {
            gm.GenerateRandomObstacle();
        }
        else if (other.CompareTag("TileTrigger"))
        {
            gm.GenerateRandomTile();
        }
        else if (other.gameObject.CompareTag("Coin"))
        {
            other.gameObject.GetComponent<CoinController>().Pop();
            Debug.Log("CoinsAdd");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Die();
            Debug.Log(collision.gameObject.transform.position);
        }
    }

    private void Die()
    {
        gm.GameEnd();
        GameObject currentExplosion =
            Instantiate(explosionObject, playerTransform.position, playerTransform.rotation, null);
        rb.velocity = new Vector3(0, 0, 0);
        Destroy(currentExplosion, 10);
        playerTransform.position = horizontalPositions[1].verticalPositions[1].transform.position;
        currentTransform = horizontalPositions[1].verticalPositions[1].transform;
        transform.position = startPosition;
        currentHorizontalPosition = 1;
        currentVerticalPosition = 1;
        gameObject.SetActive(false);
    }

}