using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerBehavior : MonoBehaviour
{
    [Header("Victim references")]

    public VictimBehavior victim;

    [Header("Movement Zone")]

    public float movementSpeed;
    public float sprintMultiplier = 2.0f;
    public float slowMultiplier = 0.5f;


    [Header("Skillcheck zone")]

    public float ignoreInputDelaySkillcheck = 0.7f;
    public bool startSkillCheck;
    private bool inSkillCheck;

    private bool isInGameOver = false;

    [SerializeField] private SkillCheckTonyHawkController scScript;
    [SerializeField] private GameObject barGO;

    [Header("Theft skillcheck")]

    public float    mTheftDuration = 10.0f;
    private float   mCurrentTheftDuration = 0.0f;
    private bool    mMissed = false;
    public bool     mTheftStarted = false;
    private bool    mRobbing = false;

    public float mTheftMarkSpeed = 1;
    private float mMarkPosition = 0.01f;

    [SerializeField] private SkillCheckRobeti mRobScript;
    [SerializeField] private GameObject mRobGO;

    private IEnumerator SkillCheckTonyHawkStyle()
    {
        float timer = 0.0f;
        float currentAcceleration = 0.0f;
        float markPosition = 0.5f;
        float currentSpeed = 0.05f;
        bool skillcheckFailed = false;
        //TODO: (aniol) this has to be retreived from the victim
        float minPercent = 0.15f;
        float maxPercent = 0.85f;
        scScript.RefreshBarLimits(minPercent, maxPercent);
 

        while (inSkillCheck && !skillcheckFailed)
        {
            if(ignoreInputDelaySkillcheck < timer)
            {
                if (Input.GetKey(KeyCode.A))
                {
                    currentAcceleration += 4.0f;
                }
                if (Input.GetKey(KeyCode.D))
                {
                    currentAcceleration -= 4.0f;
                }
            }

            currentAcceleration = Mathf.Clamp(currentAcceleration, -2.0f, 2.0f);

            currentSpeed += currentAcceleration * Time.deltaTime;
            markPosition += currentSpeed * Time.deltaTime;

            markPosition = Mathf.Clamp(markPosition, 0.0f, 1.0f);
            scScript.UpdateBarPosition(markPosition);

            if(markPosition < minPercent || markPosition > maxPercent)
            {
                skillcheckFailed = true;
                StartCoroutine("GameOverRoutine");
            }


            timer += Time.deltaTime;

            yield return null;
        }

        barGO.SetActive(false);

    }

    public void StartStealing()
    {
        mTheftStarted = true;
        mRobbing = true;

        mRobGO.SetActive(true);

        mRobScript.RefreshBarLimits(0.4f, 0.6f); //yes xd

    }

    private IEnumerator GameOverRoutine()
    {
        isInGameOver = true;

        yield return new WaitForSeconds(5.0f);

        SceneManager.LoadScene("GrayBox");
    }

    public void ReceiveSkillCheckNotification(bool enable)
    {
        startSkillCheck = enable;
        inSkillCheck = enable;
    }

    private void UpdateRobSkillcheck()
    {
        if(mCurrentTheftDuration >= mTheftDuration || mMissed)
        {
            //TODO: End game, lost
            mTheftStarted = false;
            mRobbing = false;
            mRobGO.SetActive(false);
            Debug.Log("PIERDES PUTO TONTO");
            return;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            if(0.4f <= mMarkPosition && mMarkPosition <= 0.6f)
            {
                //TODO: WIN
                Debug.Log("GANAS PUTO TONTO");
                mTheftStarted = false;
                mRobbing = false;
                mRobGO.SetActive(false);
                return;
            }
            else
            {
                //TODO: End game, lost
                mTheftStarted = false;
                mRobbing = false;
                mRobGO.SetActive(false);
                Debug.Log("PIERDES PUTO TONTO");
                return;
            }
        }
        mMarkPosition += mTheftMarkSpeed * Time.deltaTime;

        mMarkPosition = Mathf.Clamp(mMarkPosition, 0.0f, 1.0f);
        mRobScript.UpdateBarPosition(mMarkPosition);

    }

    private void Update()
    {
        mRobGO.SetActive(true);
        if (startSkillCheck)
        {
            inSkillCheck = true;
            barGO.SetActive(true);
            StartCoroutine("SkillCheckTonyHawkStyle");
            startSkillCheck = false;
        }
        if(mRobbing)
        {
            mCurrentTheftDuration += Time.deltaTime;
            UpdateRobSkillcheck();
        }
    }

    void FixedUpdate()
    {
        if (inSkillCheck || isInGameOver) return;
        Vector3 movementDirection;

        movementDirection.x = Input.GetAxisRaw("Horizontal");
        movementDirection.z = Input.GetAxisRaw("Vertical");
        movementDirection.y = 0;

        float sprintSpeedMultiplier = Input.GetKey(KeyCode.LeftShift) ? sprintMultiplier : 1.0f;
        float slowSpeedMultiplier = Input.GetKey(KeyCode.LeftControl) ? slowMultiplier : 1.0f;

        transform.rotation = Quaternion.LookRotation(movementDirection);
        transform.position += movementDirection * movementSpeed * Time.deltaTime * sprintSpeedMultiplier * slowSpeedMultiplier;
    }

}
