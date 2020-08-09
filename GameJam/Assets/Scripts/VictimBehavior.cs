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
    public float mDistanceThreshold;
    public float mFocusTime;
    private float mTimeLooking = 0.0f;
    private bool mFollowingPlayer = false;
    private float mTimeInProximity = 0.0f;

    public List<PairFloatString> mAlertTexts = new List<PairFloatString>((int)eAlertState.NumberOfStates);

    public eAlertState CurrentState = eAlertState.Safe;

    public GameObject mDetectionWarning;     //visual alert of the current alert state
    public PlayerBehavior mRobber;           //reference to the robber

    public float mSkillcheckCoolDown = 5;    //Cooldown Between skillchecks
    private float mTimeSinceLastSkillCheck = 0;

    ///////////////// Methods
    private void Start()
    {
        mDetectionWarning.SetActive(true);
        CurrentState = (eAlertState)0;
        mDetectionWarning.GetComponent<TextMesh>().text = mAlertTexts[0].mStringValue;
    }

    void Update()
    {
        mTimeSinceLastSkillCheck += Time.deltaTime;
        ProximityDetection();
        if(mFollowingPlayer)
        {
            VisualFollowing();
            mTimeLooking += Time.deltaTime;
        }
        //if a random between the closest skillcheck and the furthest is bigger thatn the current distance, we call to trigger a skillcheck
        //keep in mind that there is a cooldown, so probably it will not be activated too often
        if(UnityEngine.Random.Range(mAlertTexts[0].mFloatValue, mAlertTexts[(int)eAlertState.NumberOfStates].mFloatValue) > Vector3.Distance(transform.position, mRobber.transform.position))
        {
            TriggerSkillcheck();
        }
    }

    //stops a skillcheck
    void StopSkillCheck()
    {
        if(mFollowingPlayer)
        {
            mTimeLooking = 0;
            mFollowingPlayer = false;
            float distractedX = transform.position.x - transform.forward.x;
            float distractedY = transform.position.y;
            float distractedZ = transform.position.z - transform.forward.z;
            Vector3 LookAtPos = new Vector3(distractedX, distractedY, distractedZ);
            transform.LookAt(LookAtPos);
        }
    }

    //Triggers a skillcheck
    void TriggerSkillcheck()
    {
        //cooldown
        if(mTimeSinceLastSkillCheck >= mSkillcheckCoolDown)
        {
            mTimeLooking = 0;
            mFollowingPlayer = true;
            mTimeSinceLastSkillCheck = 0;
            mRobber.ReceiveSkillCheckNotification(true);
        }
    }

    void VisualFollowing()
    {
        //TODO(@Roger): move this logics to the player
        //if time's up, we stop looking at player
        if(mTimeLooking >= mFocusTime)
        {
            StopSkillCheck();
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
            float CurrentThreshold = mDistanceThreshold;
            //if we are not in this state, our threshold is negative
            if ((int)CurrentState != iterator)
            {
                CurrentThreshold *= -1;
            }
            if (dist < (Dist.mFloatValue + CurrentThreshold))
            {
                //if closest distance, we just rob and end
                if ((int)CurrentState > iterator)
                {
                    if(iterator == 0)
                    {
                        mRobber.StartStealing();
                    }
                    else
                    {
                        TriggerSkillcheck();
                    }
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
