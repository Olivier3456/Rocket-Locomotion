using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private float shotLength = 0.25f;

    [SerializeField] InputActionReference shotInputAction;

    private float shotTimer = 0f;
    private bool isShooting = false;
    private bool isShotInput = false;

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
        isShotInput = true;
    }

    private void shotInputAction_canceled(InputAction.CallbackContext obj)
    {
        isShotInput = false;
    }


    private void Update()
    {
        // DEBUG
        if (Input.GetKeyDown(KeyCode.Space)) { isShotInput = true; }
        else if (Input.GetKeyUp(KeyCode.Space)) { isShotInput = false; }

        if (isShotInput && !isShooting)
        {
            isShooting = true;
            shotTimer = 0f;
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
