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

    [SerializeField] private positionCluster[] horizontalPositions;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float laneSwitchingSpeed;
    [SerializeField] private float jumpTime;
    [SerializeField] private float duckTime;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    private Vector3 startPosition;
    private Rigidbody rb;
    public Transform currentTransform;
    public int currentHorizontalPosition = 1;
    public int currentVerticalPosition = 1;
    private Vector3 firstPosition;
    private Vector3 lastPosition;
    private float dragDistance;
    private bool isDucking = false;
    private bool isJumping = false;
    private float countdown = 0;
    void Start()
    {
        currentTransform = horizontalPositions[currentHorizontalPosition].verticalPositions[currentVerticalPosition]
            .transform;
        dragDistance = Screen.height * 5 / 100;
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
    }

    void FixedUpdate()
    {
        if (rb.velocity.magnitude < maxSpeed)
        {
            rb.AddForce(0, 0, acceleration);
        }
        playerTransform.position = Vector3.Lerp(playerTransform.position, currentTransform.position,
            laneSwitchingSpeed * Time.deltaTime);
        CheckInput();

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
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    firstPosition = touch.position;
                    lastPosition = touch.position;
                    break;
                case TouchPhase.Ended:
                {
                    lastPosition = touch.position;

                    if (Mathf.Abs(lastPosition.x - firstPosition.x) > dragDistance ||
                        Mathf.Abs(lastPosition.y - firstPosition.y) > dragDistance)
                    {
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

                    break;
                }
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
}