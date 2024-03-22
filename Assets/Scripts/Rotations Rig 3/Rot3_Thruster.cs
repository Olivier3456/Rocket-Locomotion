using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Rot3_Thruster : MonoBehaviour
{
    [SerializeField] private Transform xrRigMainTransform;
    [Space(20)]
    [SerializeField] private Rot_PlayerLife playerLife;
    [Space(20)]
    [SerializeField] private Rot3_Thrusters_Forces_Mixer forcesMixer;
    //[Space(20)]
    //[SerializeField] private Rot_BoostManager thrusterBoostManager;
    [Space(20)]
    [SerializeField] private Transform controllerTransform;
    [Space(20)]
    [SerializeField] private Transform thrusterVisual;
    [Space(20)]
    [SerializeField] private InputActionReference thrust;

    // https://sadhikamaja.blogspot.com/2022/12/yaw-pitch-roll.html

    [SerializeField] private InputActionReference yaw;
    [SerializeField] private InputActionReference pitch;
    [SerializeField] private InputActionReference roll;
    //[SerializeField] private InputActionReference boost;
    [SerializeField] private InputActionReference thrusterRotation;
    [Space(20)]
    [SerializeField] private float thrusterBaseForceFactor = 700f;
    //[SerializeField] private float thrusterBoostForceFactor = 700f;
    [Space(20)]
    [SerializeField] private float rotationSensibility = 2f;
    [SerializeField] private float gravityCenterYOffsetFromCamera = 0f;
    [Space(20)]
    [SerializeField] private AudioSource mainAudioSource;
    [SerializeField] private AudioSource rotateAudioSource;
    [Space(20)]
    [SerializeField] private ParticleSystem thrusterParticle;
    [Space(20)]
    [SerializeField] private AnimationCurve thrusterRotationAnimCurve;
    [SerializeField] private AnimationCurve thrusterAudioAnimCurve;
    [Space(20)]
    [SerializeField] private Rigidbody rb;

    private bool isForward = true;
    public bool IsForward { get { return isForward; } }

    private float thrustInput = 0f;
    //private float boostInput = 0f;

    private float thrustValue = 0f;
    //private float boostValue = 0f;

    private float yawInput = 0f;
    private float pitchInput = 0f;
    private float rollInput = 0f;

    [SerializeField] private float yawForceFactor = 1f;
    [SerializeField] private float pitchForceFactor = 1f;
    [SerializeField] private float rollForceFactor = 1f;




    //private float rotateInput = 0f;

    private bool canThrust = true;
    //private bool canBoost = true;

    private ParticleSystem.MainModule particleMainModule;

    private Color thrustColor = new Color(1, 1, 1, 0.75f);              // White.
    private Color transparentThrust = new Color(1, 1, 1, 0);
    private Color rotateColor = new Color(0.9f, 0.1f, 0, 0.75f);        //Red with a bit of orange.
    private Color transparentRotate = new Color(0.9f, 0.1f, 0, 0f);

    //private Color transparentBoost = new Color(0.9f, 0.1f, 0, 0f);
    //private Color boostColor = new Color(0.9f, 0.1f, 0, 0.75f);       //Red with a bit of orange.

    public float ThrusterBaseForceFactor { get { return thrusterBaseForceFactor; } }
    //public float BoostValue { get { return boostValue; } }

    private float normalDistanceFromGravityCenter = 0.1f;   // Reduces torque force if controller is very near gravity center.

    private float torqueMagnitude;


    void Start()
    {
        thrust.action.performed += Thrust_Action_performed;
        thrust.action.canceled += Thrust_Action_canceled;
        thrust.action.Enable();

        thrusterRotation.action.started += ThrusterRotation_Action_started;
        thrusterRotation.action.Enable();



        yaw.action.performed += Yaw_Action_performed;
        yaw.action.canceled += Yaw_Action_canceled;
        yaw.action.Enable();

        pitch.action.performed += Pitch_Action_performed;
        pitch.action.canceled += Pitch_Action_canceled;
        pitch.action.Enable();

        roll.action.performed += Roll_Action_performed;
        roll.action.canceled += Roll_Action_canceled;
        roll.action.Enable();



        //boost.action.performed += Boost_Action_performed;
        //boost.action.canceled += Boost_Action_canceled;
        //boost.action.Enable();

        particleMainModule = thrusterParticle.main;

        mainAudioSource.time = Random.Range(0, mainAudioSource.clip.length);
    }



    private void Yaw_Action_performed(InputAction.CallbackContext obj)
    {
        yawInput = obj.ReadValue<Vector2>().x;
    }
    private void Yaw_Action_canceled(InputAction.CallbackContext obj)
    {
        yawInput = 0f;
    }

    private void Pitch_Action_performed(InputAction.CallbackContext obj)
    {
        pitchInput = obj.ReadValue<Vector2>().y;
    }
    private void Pitch_Action_canceled(InputAction.CallbackContext obj)
    {
        pitchInput = 0f;
    }

    private void Roll_Action_performed(InputAction.CallbackContext obj)
    {
        rollInput = obj.ReadValue<Vector2>().x;
    }
    private void Roll_Action_canceled(InputAction.CallbackContext obj)
    {
        rollInput = 0f;
    }



    private void OnDestroy()
    {
        thrust.action.Disable();
        thrust.action.performed -= Thrust_Action_performed;
        thrust.action.canceled -= Thrust_Action_canceled;

        thrusterRotation.action.Disable();
        thrusterRotation.action.started -= ThrusterRotation_Action_started;


        yaw.action.Disable();
        yaw.action.performed -= Yaw_Action_performed;
        yaw.action.canceled -= Yaw_Action_canceled;

        pitch.action.Disable();
        pitch.action.performed -= Pitch_Action_performed;
        pitch.action.canceled -= Pitch_Action_canceled;

        roll.action.Disable();
        roll.action.performed -= Roll_Action_performed;
        roll.action.canceled -= Roll_Action_canceled;



        //boost.action.Disable();
        //boost.action.performed -= Boost_Action_performed;
        //boost.action.canceled -= Boost_Action_canceled;
    }


    private void Thrust_Action_performed(InputAction.CallbackContext obj)
    {
        thrustInput = obj.ReadValue<float>();
    }
    private void Thrust_Action_canceled(InputAction.CallbackContext obj)
    {
        thrustInput = 0f;
    }

    private void ThrusterRotation_Action_started(InputAction.CallbackContext obj)
    {
        if (canThrust)
        {
            isForward = !isForward;
            StartCoroutine(ThrusterRotation());
        }
    }

    //private void Boost_Action_performed(InputAction.CallbackContext obj)
    //{
    //    boostInput = obj.ReadValue<float>();
    //}
    //private void Boost_Action_canceled(InputAction.CallbackContext obj)
    //{
    //    boostInput = 0f;
    //}



    private void Update()
    {
        //UpdateThrustersValues();

        //thrusterBoostManager.Boost(boostValue);
        //float normalizedTorqueMagnitude = torqueMagnitude / rotationSensibility;

        mainAudioSource.volume = thrustValue;
        //mainAudioSource.pitch = 1 + boostValue;

        //Color flameColor = boostValue == 0 ? Color.Lerp(transparentThrust, thrustColor, thrustValue) : Color.Lerp(transparentBoost, boostColor, (boostValue + thrustValue) * 0.5f);


        Color flameColor = Color.Lerp(transparentThrust, thrustColor, thrustValue);
        particleMainModule.startColor = flameColor;
    }


    void FixedUpdate()
    {
        //if (!playerLife.IsAlive)
        //{
        //    return;
        //}

        Rotation();
        Thrust();
    }



    private void Thrust()
    {
        if (!playerLife.IsAlive)
        {
            thrustValue = 0f;
            return;
        }

        //UpdateThrustersValues();

        Vector3 thrustVector;
        //Vector3 boostVector;

        thrustValue = canThrust ? thrustInput : 0f;

        thrustVector = controllerTransform.forward * thrustValue * thrusterBaseForceFactor;
        //boostVector = controllerTransform.forward * boostValue * thrusterBoostForceFactor;

        if (!isForward)
        {
            thrustVector *= -1;
            //boostVector *= -1;
        }

        Vector3 totalVector = thrustVector; // + boostVector;

        //rb.AddForce(totalVector);
        forcesMixer.Thrust(totalVector, this);
    }


    private void Rotation()
    {
        if (!playerLife.IsAlive)
        {
            return;
        }

        if (yawInput != 0f) rb.AddTorque(xrRigMainTransform.up * yawInput * yawForceFactor);
        if (pitchInput != 0f) rb.AddTorque(xrRigMainTransform.right * -pitchInput * pitchForceFactor);
        if (rollInput != 0f) rb.AddTorque(xrRigMainTransform.forward * -rollInput * rollForceFactor);
    }


    //private void UpdateThrustersValues()
    //{
    //    if (!playerLife.IsAlive)
    //    {
    //        thrustValue = 0f;
    //        //boostValue = 0f;
    //        return;
    //    }

    //    //boostValue = isRotating && canBoost ? boostInput : 0f;

    //    //if (boostValue == 0)
    //    //{
    //    thrustValue = isRotating ? thrustInput : 0f;
    //    //}
    //    //else // If the thruster boosts, even a little, its base force vector is at max.
    //    //{
    //    //    thrustValue = 1;
    //    //}
    //}


    private IEnumerator ThrusterRotation()
    {
        canThrust = false;
        rotateAudioSource.Play();

        float rotationTime = 0.5f;
        float progress = 0f;

        while (progress < 1)
        {
            yield return null;
            progress += Time.deltaTime / rotationTime;
            float value = thrusterRotationAnimCurve.Evaluate(progress);
            thrusterVisual.forward = isForward ? Vector3.Slerp(-controllerTransform.forward, controllerTransform.forward, value) : Vector3.Slerp(controllerTransform.forward, -controllerTransform.forward, value);

            rotateAudioSource.pitch = 1 + thrusterAudioAnimCurve.Evaluate(progress);
        }

        thrusterVisual.forward = isForward ? controllerTransform.forward : -controllerTransform.forward;

        canThrust = true;
        rotateAudioSource.Stop();
    }


    //public void AuthoriseBoost(bool canBoost)
    //{
    //    this.canBoost = canBoost;
    //}    
}
