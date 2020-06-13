using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform FocusOnObject;

    float zPos;

    // Start is called before the first frame update
    void Awake()
    {
        zPos = transform.position.z;
    }

    void LateUpdate()
    {
        if (FocusOnObject != null)
        {
            transform.position = new Vector3(FocusOnObject.position.x, FocusOnObject.position.y, zPos);
        }
    }
}
