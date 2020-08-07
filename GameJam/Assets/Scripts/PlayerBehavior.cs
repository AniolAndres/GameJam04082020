using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    public float movementSpeed;
    public float sprintMultiplier = 2.0f;
    public float slowMultiplier = 0.5f;

    void FixedUpdate()
    {
        Vector3 movementDirection;

        movementDirection.x = Input.GetAxisRaw("Horizontal");
        movementDirection.z = Input.GetAxisRaw("Vertical");
        movementDirection.y = 0;

        float sprintSpeedMultiplier = Input.GetKey(KeyCode.LeftShift) ? sprintMultiplier : 1.0f;
        float slowSpeedMultiplier = Input.GetKey(KeyCode.LeftControl) ? slowMultiplier : 1.0f;


        transform.position += movementDirection * movementSpeed * Time.deltaTime * sprintSpeedMultiplier * slowSpeedMultiplier;
    }

}
