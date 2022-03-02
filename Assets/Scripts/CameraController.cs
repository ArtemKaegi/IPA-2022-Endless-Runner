using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject currentCameraPosition;
    public float cameraMovementSpeed;
    public float cameraRotationSpeed;
    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, currentCameraPosition.transform.position, cameraMovementSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, currentCameraPosition.transform.rotation,
            cameraRotationSpeed * Time.deltaTime);
    }

}
