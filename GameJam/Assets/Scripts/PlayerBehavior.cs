using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerBehavior : MonoBehaviour
{
    [Header("Victim references")]

    public VictimBehavior victim;
    public GameObject victimGO;
    public float stealDistance = 10.0f;

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

    public float mTheftDuration = 10.0f;
    private float mCurrentTheftDuration = 0.0f;
    private bool mMissed = false;
    public bool mTheftStarted = false;
    private bool mRobbing = false;

    public float mTheftMarkSpeed = 1;
    private float mMarkPosition = 0.01f;
    public GameObject fButtonGO; 

    [Header("Theft animations")]

    private Animator anim;

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

        anim.SetBool("skillCheck", true);

        while (inSkillCheck && !skillcheckFailed)
        {
            if (ignoreInputDelaySkillcheck < timer)
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

        if(!skillcheckFailed)
            anim.SetBool("skillCheck", false);

        barGO.SetActive(false);

    }

    public void StartStealing()
    {
        mRobGO.SetActive(true);
        mTheftStarted = true;
        mRobbing = true;
        mMarkPosition = 0.75f;

        mRobScript.RefreshBarLimits(0.4f, 0.6f); //yes xd

    }

    private IEnumerator GameOverRoutine()
    {
        isInGameOver = true;
        anim.SetBool("failedSkillCheck", true);

        yield return new WaitForSeconds(3.0f);

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
            StartCoroutine("GameOverRoutine");
            return;
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            if(0.4f <= mMarkPosition && mMarkPosition <= 0.6f)
            {
                //TODO: WIN
                victim.RobbedSuccessfuly();
                StartCoroutine("GameOverRoutine");
                mTheftStarted = false;
                mRobGO.SetActive(false);
                return;
            }
            else
            {
                //TODO: End game, lost
                mTheftStarted = false;
                mRobbing = false;
                mRobGO.SetActive(false);
                StartCoroutine("GameOverRoutine");
                return;
            }
        }
        mMarkPosition += mTheftMarkSpeed * Time.deltaTime;
        mMarkPosition = mMarkPosition % 1;

        mMarkPosition = Mathf.Clamp(mMarkPosition, 0.0f, 1.0f);
        mRobScript.UpdateBarPosition(mMarkPosition);

    }

    private void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        if (mRobbing)
        {
            // TODO: Start robbing animation
            mCurrentTheftDuration += Time.deltaTime;
            UpdateRobSkillcheck();
        }

        float distance = Vector3.Distance(transform.position, victimGO.transform.position);
        if (distance < stealDistance)
        {
            if (!mRobbing)
            {
                fButtonGO.SetActive(true);
            }
            else
            {
                fButtonGO.SetActive(false);
            }

            if (!mRobbing && Input.GetKeyDown(KeyCode.F))
            {
                StartStealing();
            }
        }
        else
        {
            fButtonGO.SetActive(false);
        }

        if (startSkillCheck)
        {
            inSkillCheck = true;
            barGO.SetActive(true);
            StartCoroutine("SkillCheckTonyHawkStyle");
            startSkillCheck = false;

        }
    }

    void FixedUpdate()
    {
        if (inSkillCheck || isInGameOver) return;
        Vector3 movementDirection;

        movementDirection.x = Input.GetAxisRaw("Horizontal");
        movementDirection.z = Input.GetAxisRaw("Vertical");
        movementDirection.y = 0;

        float speedMultiplier = 1.0f;

        if(Input.GetKey(KeyCode.LeftShift))
        {
            speedMultiplier = sprintMultiplier;
        } 
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            speedMultiplier = slowMultiplier;
        }

        transform.rotation = Quaternion.LookRotation(movementDirection);
        transform.position += movementDirection * movementSpeed * Time.deltaTime * speedMultiplier;
        anim.SetBool("Idle", movementDirection == Vector3.zero);
        anim.SetFloat("Speed", speedMultiplier);
    }

}
