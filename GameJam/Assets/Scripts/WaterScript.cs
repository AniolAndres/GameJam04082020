using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterScript : MonoBehaviour
{
    public Renderer myRenderer;

    public Texture[] frames;
        
    public int framesPerSecond = 10;

    void Update()
    {
        var index = (int)((Time.time * framesPerSecond) % frames.Length);
        myRenderer.material.mainTexture = frames[index];
    }
}
