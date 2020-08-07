using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    [Header("Movement Zone")]

    public float movementSpeed;
    public float sprintMultiplier = 2.0f;
    public float slowMultiplier = 0.5f;


    [Header("Skillcheck zone")]

    public float skillCheckDuration = 10.0f;
    public bool startSkillCheck;
    private bool inSkillCheck;

    public float markPosition = 0.0f;
    public float currentSpeed = 0.001f;
    public float currentAcceleration = 0.0f;

    private IEnumerator SkillCheckTonyHawkStyle()
    {
        //TODO: (aniol) MAKE THIS VARIABLES GREAT (and local) AGAIN
        float duration = skillCheckDuration;
        //float markPosition = 0.0f;
        //float currentSpeed = 1.0f;
        while(duration >= 0.0f)
        {
            //float currentAcceleration = 0.0f;
            currentAcceleration = 0.0f;

            if (Input.GetKey(KeyCode.A))
            {
                currentAcceleration -= 1.0f;
            }
            if (Input.GetKey(KeyCode.D))
            {
                currentAcceleration += 1.0f;
            }

            currentSpeed += currentAcceleration * Time.deltaTime;
            markPosition += currentSpeed * Time.deltaTime * 0.01f;

            markPosition = Mathf.Clamp(markPosition, -100.0f, 100.0f);

            duration -= Time.deltaTime;
            yield return null;
        }
        inSkillCheck = false;


    }

    private void Update()
    {
        if (startSkillCheck)
        {
            inSkillCheck = true;
            StartCoroutine("SkillCheckTonyHawkStyle");
            startSkillCheck = false;

        }
    }

    void FixedUpdate()
    {
        if (inSkillCheck) return;
        Vector3 movementDirection;

        movementDirection.x = Input.GetAxisRaw("Horizontal");
        movementDirection.z = Input.GetAxisRaw("Vertical");
        movementDirection.y = 0;

        float sprintSpeedMultiplier = Input.GetKey(KeyCode.LeftShift) ? sprintMultiplier : 1.0f;
        float slowSpeedMultiplier = Input.GetKey(KeyCode.LeftControl) ? slowMultiplier : 1.0f;


        transform.position += movementDirection * movementSpeed * Time.deltaTime * sprintSpeedMultiplier * slowSpeedMultiplier;
    }

}
