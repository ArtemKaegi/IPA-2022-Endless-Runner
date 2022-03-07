using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraController : MonoBehaviour
{
    public GameObject currentCameraPosition;
    public float cameraMovementSpeed;
    public float cameraRotationSpeed;
    public float elapsed = 0;
    public float duration = 0.05f;
    public float magnitude = 0;
    private void FixedUpdate()
    {
        
        transform.position = Vector3.Lerp(transform.position, currentCameraPosition.transform.position, cameraMovementSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, currentCameraPosition.transform.rotation,
            cameraRotationSpeed * Time.deltaTime);
    }

    private void Update()
    {
        if (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.position += new Vector3(x, y);

            elapsed += Time.deltaTime;
            Debug.Log("Shaking");

        }
    }

    public void Shake(float elapsed, float magnitude)
    {
        this.elapsed = elapsed;
        this.magnitude = magnitude;
    }

}
