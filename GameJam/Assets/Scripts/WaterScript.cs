using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterScript : MonoBehaviour
{
    public Renderer renderer;

    public Texture[] frames;
        
    int framesPerSecond = 10;

    // Update is called once per frame
    void Update()
    {
        var index = (int)((Time.time * framesPerSecond) % frames.Length);
        renderer.material.mainTexture = frames[index];
    }
}
