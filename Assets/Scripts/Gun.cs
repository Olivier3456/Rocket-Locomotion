using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    [SerializeField] private ControllerManager controllerManager;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] InputActionReference shotInputAction;
    [SerializeField] private float shotLength = 0.25f;
    [Space(20)]
    [SerializeField] private Transform raycastOrigin;
    [SerializeField] private float maxRaycastDistance = 200f;
    [SerializeField] private Transform laserPoint;



    private float shotTimer = 0f;
    private bool isShooting = false;
    private bool hasNewShotInput = false;

    private const string ANIMATOR_SHOT_TRIGGER = "Shot";

    private void OnEnable()
    {
        shotInputAction.action.started += shotInputAction_started;
        shotInputAction.action.canceled += shotInputAction_canceled;
        shotInputAction.action.Enable();
    }
    private void OnDisable()
    {
        shotInputAction.action.Disable();
        shotInputAction.action.started -= shotInputAction_started;
        shotInputAction.action.canceled -= shotInputAction_canceled;
    }

    private void shotInputAction_started(InputAction.CallbackContext obj)
    {
        hasNewShotInput = true;
    }

    private void shotInputAction_canceled(InputAction.CallbackContext obj)
    {
        hasNewShotInput = false;
    }


    private void Update()
    {
        // DEBUG
        //if (Input.GetKeyDown(KeyCode.Space)) { hasNewShotInput = true; }
        //else if (Input.GetKeyUp(KeyCode.Space)) { hasNewShotInput = false; }


        if (!MainManager.Instance.IsSimulationRunning)
        {
            return;
        }

        if (Physics.Raycast(raycastOrigin.position, raycastOrigin.forward, out RaycastHit hit, maxRaycastDistance))
        {
            laserPoint.position = hit.point;
            float scale = 1 + (hit.distance / (maxRaycastDistance * 0.5f));
            Vector3 newLocalScale = new Vector3(scale, scale, scale);
            laserPoint.localScale = newLocalScale;
            laserPoint.forward = hit.normal;
            laserPoint.gameObject.SetActive(true);
        }
        else
        {
            laserPoint.gameObject.SetActive(false);
        }


        if (hasNewShotInput && !isShooting)
        {
            isShooting = true;
            hasNewShotInput = false;
            shotTimer = 0f;
            float pitch = Random.Range(0.8f, 1.1f);
            audioSource.pitch = pitch;
            audioSource.Play();
            animator.SetTrigger(ANIMATOR_SHOT_TRIGGER);
        }

        if (isShooting)
        {
            shotTimer += Time.deltaTime;

            if (shotTimer > shotLength)
            {
                isShooting = false;
            }
        }
    }
}
