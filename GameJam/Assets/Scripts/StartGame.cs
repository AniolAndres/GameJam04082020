using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    public float timePlayer = 3.5f;

    public GameObject gameCamera;
    public GameObject victimPosition;
    public GameObject playerPosition;

    private bool isPreviewingVictim;

    void Start()
    {
        EnableVictimPreview();
        // Move to player
        // Enable player movement
    }

    private void FixedUpdate()
    {
        
        //if(victimTime) { 
        //    VictimPreview();
        //}
    }

    void VictimPreview()
    {
        gameCamera.transform.LookAt(victimPosition.transform);
        gameCamera.transform.Translate(Vector3.right * Time.deltaTime);
    }

    IEnumerator EnableVictimPreview()
    {
        isPreviewingVictim = true;
        yield return new WaitForSeconds(3.5f);
    }

}
