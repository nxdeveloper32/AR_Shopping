using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayProduct : MonoBehaviour
{
    public float sensitivity;
    void OnMouseDrag()
    {
        float rotX = Input.GetAxis("Mouse X") * sensitivity * Mathf.Deg2Rad;
        float rotY = Input.GetAxis("Mouse Y") * sensitivity * Mathf.Deg2Rad;
        transform.Rotate(Vector3.forward, rotY);
        transform.Rotate(Vector3.right, rotX);
    }
}
