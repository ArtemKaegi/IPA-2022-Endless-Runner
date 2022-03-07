using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinObjectController : MonoBehaviour
{
    public GameObject skin;
    void Update()
    {
        skin.transform.Rotate(new Vector3(0, 30 * Time.deltaTime, 0));
    }
}
