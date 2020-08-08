using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBehavior : MonoBehaviour
{
    public bool buttonEnter;
    public bool buttonExit;

    public GameObject victim;
    public GameObject thief;

    // Update is called once per frame
    void Update()
    {
        if(buttonEnter)
        {
            Quaternion currentRotation = victim.transform.rotation;
            Quaternion wantedRotation = Quaternion.Euler(0, 55, 0);
            victim.transform.rotation = Quaternion.RotateTowards(currentRotation, wantedRotation, Time.deltaTime * 5000.0f);
        }

        if(buttonExit)
        {
            Quaternion currentRotation = victim.transform.rotation;
            Quaternion wantedRotation = Quaternion.Euler(0, 210, 0);
            victim.transform.rotation = Quaternion.RotateTowards(currentRotation, wantedRotation, Time.deltaTime * 5000.0f);
        }
    }

    public void EnterButton()
    {
        buttonEnter = true;
        buttonExit = false;
    }

    public void ExitButton()
    {
        buttonEnter = false;
        buttonExit = true;
    }

}
