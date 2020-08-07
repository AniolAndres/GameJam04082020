using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class VictimBehavior : MonoBehaviour
{
    ///////////////// Types
    public enum eAlertState : UInt16
    {
        Safe = 0,               //The victim feels completely safe and does not get scared easily.
        LowAlert,               //The victim has a slight suspicion about what is going on.
        MediumAlert,            //The victim does not seem to trust the robber.
        HighAlert,              //The victim is highly suspicious about the robber.
        Escaping,               //The victim is running from the robber.
        NumberOfStates          //the amount of states
    };

    ///////////////// Vars
    public float VisionDistance;
    public float VisionAngle;
    public float mTimeInProximityToAlertUp;
    public float mTimeInVisionToAlertUp;
    private float mTimeInVision = 0.0f;
    private float mTimeInProximity = 0.0f;

    public eAlertState CurrentState = eAlertState.Safe;

    public GameObject mDetectionWarning;     //visual alert of the current alert state
    public GameObject mRobber;               //reference to the robber

    ///////////////// Methods

    // Start is called before the first frame update
    void Update()
    {
        VisualDetection();
        ProximityDetection();
    }

    void VisualDetection()
    {

    }

    void ProximityDetection()
    {

    }

    IEnumerator TurnAndLook(int seconds)
    {
        mDetectionWarning.SetActive(true);
        Debug.Log("Yo' something happening " + seconds);
        yield return new WaitForSeconds(seconds);
        mDetectionWarning.SetActive(false);
    }
}
