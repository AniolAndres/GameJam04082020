using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using UnityEngine;

[Serializable]
public class PairFloatString
{
    public float mFloatValue;
    public String mStringValue;
}

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
    public float mFocusTime;
    private float mTimeLooking = 0.0f;
    private bool mFollowingPlayer = false;
    private float mTimeInProximity = 0.0f;

    public List<PairFloatString> mAlertTexts = new List<PairFloatString>((int)eAlertState.NumberOfStates);

    public eAlertState CurrentState = eAlertState.Safe;

    public GameObject mDetectionWarning;     //visual alert of the current alert state
    public PlayerBehavior mRobber;               //reference to the robber

    //TODO(@Roger): each time  
    ///////////////// Methods
    private void Start()
    {
        mDetectionWarning.SetActive(true);
        CurrentState = (eAlertState)0;
        mDetectionWarning.GetComponent<TextMesh>().text = mAlertTexts[0].mStringValue;
    }

    void Update()
    {
        ProximityDetection();
        if(mFollowingPlayer)
        {
            VisualFollowing();
            mTimeLooking += Time.deltaTime;
        }
    }

    void VisualFollowing()
    {
        //if time's up, we stop looking at player
        if(mTimeLooking >= mFocusTime)
        {
            mTimeLooking = 0;
            mFollowingPlayer = false;
            float distractedX = transform.position.x - transform.forward.x;
            float distractedY = transform.position.y;
            float distractedZ = transform.position.z - transform.forward.z;
            Vector3 LookAtPos = new Vector3(distractedX, distractedY, distractedZ);
            transform.LookAt(LookAtPos);
            mRobber.ReceiveSkillCheckNotification(false);
            return;
        }
        transform.LookAt(mRobber.transform.position);
        
    }

    void ProximityDetection()
    {
        //check distance to mRobber, in the editor they have to be from closer to further
        float dist = Vector3.Distance(transform.position, mRobber.transform.position);
        int iterator = 0;
        foreach (var Dist in mAlertTexts)
        {
            if(dist < Dist.mFloatValue)
            {
                //now we look at player if he got closer
                if((int)CurrentState > iterator)
                {
                    mTimeLooking = 0;
                    mFollowingPlayer = true;
                    mRobber.ReceiveSkillCheckNotification(true);
                }

                //set vars up
                CurrentState = (eAlertState)iterator;
                mDetectionWarning.GetComponent<TextMesh>().text = Dist.mStringValue;

                return;
            }
            ++iterator;
        }
    }

}
