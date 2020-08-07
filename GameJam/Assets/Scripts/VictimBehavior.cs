using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictimBehavior : MonoBehaviour
{
    public float minCheckTimer = 2.0f;

    public GameObject detectionWarning;

    // Start is called before the first frame update
    void Update()
    {
        RandomDetection();  
    }

    void RandomDetection()
    {
        if (Random.Range(0, 10) > 5 && detectionWarning.activeSelf)
        {
            StartCoroutine(TurnAndLook(Random.Range(2, 6))); //This could be linked to difficulty
        } 
        else
        {
            Debug.Log("Nothing to see");
        }
    }

    // Update is called once per frame
     IEnumerator TurnAndLook(int seconds)
    {
        detectionWarning.SetActive(true);
        Debug.Log("Yo' something happening " + seconds);
        yield return new WaitForSeconds(seconds);
        detectionWarning.SetActive(false);
    }
}
