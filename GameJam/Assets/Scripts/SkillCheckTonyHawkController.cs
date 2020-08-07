using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCheckTonyHawkController : MonoBehaviour
{
    [SerializeField] private Renderer myRenderer;
        
    public void Start()
    {
        myRenderer = gameObject.GetComponent<Renderer>();
    }


    public void UpdateBarPosition(float position)
    {
        myRenderer.material.SetFloat("_MarkPosition", position);
    }

}
