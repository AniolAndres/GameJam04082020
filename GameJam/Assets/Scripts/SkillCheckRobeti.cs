using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCheckRobeti : MonoBehaviour
{
    [SerializeField] private Renderer myRenderer;
    private Transform mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main.transform;
    }

    public void RefreshBarLimits(float min, float max)
    {
        myRenderer.material.SetFloat("_Min", min);
        myRenderer.material.SetFloat("_Max", max);
        transform.LookAt((transform.position + mainCamera.rotation * Vector3.up), (mainCamera.rotation * Vector3.up));
    }


    public void UpdateBarPosition(float position)
    {
        myRenderer.material.SetFloat("_MarkPosition", position);
    }

}
