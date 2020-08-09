using System.Collections;
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

    enum CameraState
    {
        VictimPreview,
        FollowingPlayer
    }
    CameraState mCurrentCameraState = CameraState.VictimPreview;

    void Start()
    {
        mCurrentCameraState = CameraState.VictimPreview;
        victimPosition.transform.Find("VictimInfo").gameObject.SetActive(true);

        goText = transform.GetComponentInChildren<Text>();
        cameraInitialPosition = gameCamera.transform.position;
        
    }

    private void FixedUpdate()
    {
        switch (mCurrentCameraState)
        {
            case CameraState.VictimPreview:
            {
                VictimPreview();
                if (Input.anyKey)
                {
                    victimPosition.transform.Find("VictimInfo").gameObject.SetActive(false);
                    mCurrentCameraState = CameraState.FollowingPlayer;
                    playerPosition.transform.GetComponent<PlayerBehavior>().enabled = true;
                }
            }
            break;
            case CameraState.FollowingPlayer:
            {                
                Vector3 targetPosition = playerPosition.transform.position + cameraFollowDisplacement;
                gameCamera.transform.position = Vector3.Lerp(gameCamera.transform.position, targetPosition, smoothFactorCamera * Time.deltaTime);

                    Vector3 CenterPositionLookat = (playerPosition.transform.position + victimPosition.transform.position) * 0.5f;

                Vector3 targetLookatPosition = playerPosition.transform.position + (Vector3.up * 50);
                gameCamera.transform.rotation = Quaternion.RotateTowards(gameCamera.transform.rotation, Quaternion.LookRotation(CenterPositionLookat - gameCamera.transform.position), Time.time * 0.1f);
            }
            break;
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
}
