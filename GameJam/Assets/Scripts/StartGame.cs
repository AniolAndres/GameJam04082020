using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class StartGame : MonoBehaviour
{
    public int maxGoSize = 150;
    public float cameraRotationSpeed = 2.5f;
    public GameObject gameCamera;
    public GameObject victimPosition;
    public GameObject playerPosition;

    // Camera behavior
    public Vector3 cameraFollowDisplacement = new Vector3(0, 2.5f, -0);
    public float smoothTimeCameraFollow = 0.3F;

    private Text goText;
    private bool isPreviewingVictim;
    private bool isLerpingToPlayer;
    private bool isEnablingPlayerMovement;
    private bool cameraFollowPlayer;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        goText = transform.GetComponentInChildren<Text>();
        StartCoroutine("EnableVictimPreview");
    }

    private void FixedUpdate()
    {
        if(isPreviewingVictim) 
        { 
            VictimPreview();
        }

        if (isLerpingToPlayer)
        {
            LerpCameraToPlayer();
        }

        if(isEnablingPlayerMovement)
        {
            StartTextFadingAndUI();
        }

        if(cameraFollowPlayer)
        {
            Vector3 targetPosition = playerPosition.transform.TransformPoint(cameraFollowDisplacement);
            gameCamera.transform.position = Vector3.SmoothDamp(gameCamera.transform.position, targetPosition, ref velocity, smoothTimeCameraFollow);
        }
    }

    void VictimPreview()
    {  
        gameCamera.transform.LookAt(victimPosition.transform);
        gameCamera.transform.Translate(Vector3.right * cameraRotationSpeed * Time.deltaTime);
    }

    void LerpCameraToPlayer()
    {
        float interpolation = 0.75f * Time.deltaTime;

        Vector3 position = gameCamera.transform.position;
        position.z = Mathf.Lerp(gameCamera.transform.position.z, playerPosition.transform.position.z, interpolation);
        position.x = Mathf.Lerp(gameCamera.transform.position.x, playerPosition.transform.position.x, interpolation);

        gameCamera.transform.LookAt(playerPosition.transform.parent);
        gameCamera.transform.position = position;
    }

    void StartTextFadingAndUI()
    {
        if(goText.fontSize <= 150)
            goText.fontSize += 2;
    }

    IEnumerator EnableVictimPreview()
    {
        victimPosition.transform.parent.Find("VictimInfo").gameObject.SetActive(true);
        isPreviewingVictim = true;

        while (!Input.anyKey)
            yield return null;

        isPreviewingVictim = false;
        victimPosition.transform.parent.Find("VictimInfo").gameObject.SetActive(false);

        StartCoroutine("EnableLerpCameraToPlayer");
    }

    IEnumerator EnableLerpCameraToPlayer()
    {
        isLerpingToPlayer = true;

        while (gameCamera.transform.position.z >= playerPosition.transform.position.z + 1)
            yield return null;

        isLerpingToPlayer = false;

        StartCoroutine("EnablePlayerMovement");
        
    }

    IEnumerator EnablePlayerMovement()
    {
        isEnablingPlayerMovement = true;

        while (goText.fontSize < maxGoSize)
            yield return null;

        cameraFollowPlayer = true;
        isEnablingPlayerMovement = false;
        playerPosition.transform.parent.GetComponent<PlayerBehavior>().enabled = true;
    }



}
