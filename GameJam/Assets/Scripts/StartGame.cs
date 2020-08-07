using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    public float cameraRotationSpeed = 2.5f;
    public GameObject gameCamera;
    public GameObject victimPosition;
    public GameObject playerPosition;

    private bool isPreviewingVictim;
    private bool isLerpingToPlayer;

    void Start()
    {
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

    IEnumerator EnableLerpCameraToPlayer()
    {
        isLerpingToPlayer = true;

        while (gameCamera.transform.position.z >= playerPosition.transform.position.z + 1)
            yield return null;

        isLerpingToPlayer = false;
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

}
