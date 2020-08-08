using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCheckTonyHawkController : MonoBehaviour
{
    [SerializeField] private Renderer myRenderer;
       

    public void RefreshBarLimits(float min, float max)
    {
        myRenderer.material.SetFloat("_Min", min);
        myRenderer.material.SetFloat("_Max", max);
    }


    public void UpdateBarPosition(float position)
    {
        myRenderer.material.SetFloat("_MarkPosition", position);
    }

}
