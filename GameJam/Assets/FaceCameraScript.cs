using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCameraScript : MonoBehaviour
{
    private Transform mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt((transform.position + mainCamera.rotation * Vector3.up), (mainCamera.rotation * Vector3.up));
    }
}
