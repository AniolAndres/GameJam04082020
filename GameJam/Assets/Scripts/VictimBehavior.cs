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
    private Animator mAnim;

    public float mSkillCheckDelay = 1.0f;

    public List<PairFloatString> mAlertTexts = new List<PairFloatString>((int)eAlertState.NumberOfStates);

    public eAlertState CurrentState = eAlertState.Safe;

    public GameObject mDetectionWarning;     //visual alert of the current alert state
    public PlayerBehavior mRobber;           //reference to the robber

    public float mSkillcheckCoolDown = 5;    //Cooldown Between skillchecks
    private float mTimeSinceLastSkillCheck = 0;
    public float mTimeBetweenRandomSkillChecksTry = 5.0f;
    private float mTimeSinceLastRandomSkillCheckTry = 0;

    private AudioSource mAlertSound;

    ///////////////// Methods
    private void Start()
    {
        CurrentState = (eAlertState)0;
        mDetectionWarning.GetComponent<TextMesh>().text = mAlertTexts[0].mStringValue;
        mAnim = gameObject.GetComponent<Animator>();
        mAlertSound = gameObject.GetComponent<AudioSource>();
    }

    private void CheckPlayRandomSkillChecks()
    {
        float Dist = Vector3.Distance(transform.position, mRobber.transform.position);
        //if we are in range of robeti skill check, leave
        if(Dist < mAlertTexts[0].mFloatValue)
        {
            return;
        }
        if (mTimeSinceLastRandomSkillCheckTry >= mTimeBetweenRandomSkillChecksTry)
        {
            //if a random between the closest skillcheck and the furthest is bigger thatn the current distance, we call to trigger a skillcheck
            //keep in mind that there is a cooldown, so probably it will not be activated too often
            if (UnityEngine.Random.Range(mAlertTexts[0].mFloatValue, mAlertTexts[(int)eAlertState.NumberOfStates - 1].mFloatValue*0.9f) > Dist)
            {
                TriggerSkillcheckAlert();
            }
            mTimeSinceLastRandomSkillCheckTry = 0.0f;
        }
    }

    private void FixedUpdate()
    {
        mTimeSinceLastRandomSkillCheckTry += Time.deltaTime;
        CheckPlayRandomSkillChecks();
    }

    void Update()
    {
        mTimeSinceLastSkillCheck += Time.deltaTime;
        ProximityDetection();
        mAnim.SetBool("Aware", mFollowingPlayer);
        if (mFollowingPlayer)
        {
            VisualFollowing();
            mTimeLooking += Time.deltaTime;
        } 
        mAnim.SetBool("Aware", mFollowingPlayer);
    }

    //stops a skillcheck
    void StopSkillCheck()
    {
        if(mFollowingPlayer)
        {
            mTimeLooking = 0f;
            mFollowingPlayer = false;
        }
    }

    //Triggers a skillcheck
    void TriggerSkillcheckAlert()
    {
        //cooldown
        if (mTimeSinceLastSkillCheck >= mSkillcheckCoolDown)
        {
            mAlertSound.Play();
            mDetectionWarning.SetActive(true);
            mFollowingPlayer = true;
            mTimeSinceLastSkillCheck = 0.0f;
            Invoke("PlaySkillCheck", mSkillCheckDelay);
        }
    }

    void PlaySkillCheck()
    {
        mTimeLooking = 0;
        mTimeSinceLastSkillCheck = 0;
        mRobber.ReceiveSkillCheckNotification(true);
    }

    void VisualFollowing()
    {
        //TODO(@Roger): move this logics to the player
        //if time's up, we stop looking at player
        if (mTimeLooking >= mFocusTime)
        {
            StopSkillCheck();
            mRobber.ReceiveSkillCheckNotification(false);
            mDetectionWarning.SetActive(false);
            return;
        }
       
        // transform.LookAt(mRobber.transform.position);

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
                    if(iterator > 0)
                    {
                        TriggerSkillcheckAlert();
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

    public void RobbedSuccessfuly()
    {
        mAnim.SetBool("Robbed".Length, true);
    }

}
