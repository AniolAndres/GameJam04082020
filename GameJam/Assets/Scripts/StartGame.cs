﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class StartGame : MonoBehaviour
{
    public int maxGoSize = 150;
    public float cameraRotationSpeed = 20f;
    public Transform finalCameraPosition;
    public GameObject gameCamera;
    public GameObject victimPosition;
    public GameObject playerPosition;

    // Camera behavior
    public Vector3 cameraFollowDisplacement = new Vector3(0, 2.5f, -0);
    public float smoothTimeCameraFollow = 0.3F;
    public float cameraLerpDuration = 2.0f;
    public float lerpTimer = 0.0f;
    public float smoothFactorCamera = 10.0f;

    private Text goText;
    private bool isPreviewingVictim;
    private bool isLerpingToPlayer;
    private bool isEnablingPlayerMovement;
    private bool cameraFollowPlayer;
    private Vector3 velocity = Vector3.zero;

    private Vector3 cameraInitialPosition;

    void Start()
    {
        goText = transform.GetComponentInChildren<Text>();
        StartCoroutine("EnableVictimPreview");
        cameraInitialPosition = gameCamera.transform.position;
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
            Vector3 targetPosition = playerPosition.transform.position + cameraFollowDisplacement;
            gameCamera.transform.position = Vector3.Lerp(gameCamera.transform.position, targetPosition, smoothFactorCamera * Time.deltaTime);
        }
    }

    void VictimPreview()
    {
        gameCamera.transform.RotateAround(victimPosition.transform.position, Vector3.up, cameraRotationSpeed * Time.deltaTime);
    }

    void LerpCameraToPlayer()
    {
        lerpTimer += Time.deltaTime;

        float interpolation = lerpTimer / cameraLerpDuration;

        if (interpolation >= 1.0f)
        {
            interpolation = 1.0f;
            isLerpingToPlayer = false;
        }

        Vector3 position = gameCamera.transform.position;
        position.z = Mathf.Lerp(cameraInitialPosition.z, finalCameraPosition.position.z, interpolation);
        position.x = Mathf.Lerp(cameraInitialPosition.x, finalCameraPosition.position.x, interpolation);

        gameCamera.transform.LookAt(playerPosition.transform);
        gameCamera.transform.position = position;
    }

    void StartTextFadingAndUI()
    {
        if(goText.fontSize <= 150)
            goText.fontSize += 2;
    }

    IEnumerator EnableVictimPreview()
    {
        victimPosition.transform.Find("VictimInfo").gameObject.SetActive(true);
        isPreviewingVictim = true;

        while (!Input.anyKey)
            yield return null;

        isPreviewingVictim = false;
        victimPosition.transform.Find("VictimInfo").gameObject.SetActive(false);

        StartCoroutine("EnableLerpCameraToPlayer");
    }

    IEnumerator EnableLerpCameraToPlayer()
    {
        isLerpingToPlayer = true;

        while (isLerpingToPlayer)
            yield return null;

        
        StartCoroutine("EnablePlayerMovement");
        
    }

    IEnumerator EnablePlayerMovement()
    {
        isEnablingPlayerMovement = true;

        while (goText.fontSize < maxGoSize)
            yield return null;

        cameraFollowPlayer = true;
        isEnablingPlayerMovement = false;
        playerPosition.transform.GetComponent<PlayerBehavior>().enabled = true;
    }



}
