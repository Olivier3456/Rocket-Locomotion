using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Rot2_Thruster : MonoBehaviour
{
    [SerializeField] private Transform xrRigMainTransform;
    [Space(20)]
    [SerializeField] private Rot_PlayerLife playerLife;
    [Space(20)]
    [SerializeField] private Rot2_Thrusters_Forces_Mixer forcesMixer;
    //[Space(20)]
    //[SerializeField] private Rot_BoostManager thrusterBoostManager;
    [Space(20)]
    [SerializeField] private Transform controllerTransform;
    [Space(20)]
    [SerializeField] private Transform thrusterVisual;
    [Space(20)]
    [SerializeField] private InputActionReference thrust;
    [SerializeField] private InputActionReference rotate;
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

    private bool isForward = true;
    public bool IsForward { get { return isForward; } }

    private float thrustInput = 0f;
    //private float boostInput = 0f;

    private float thrustValue = 0f;
    //private float boostValue = 0f;

    private float rotateInput = 0f;

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

        rotate.action.performed += Rotation_Action_performed;
        rotate.action.canceled += Rotation_Action_canceled;
        rotate.action.Enable();


        //boost.action.performed += Boost_Action_performed;
        //boost.action.canceled += Boost_Action_canceled;
        //boost.action.Enable();

        particleMainModule = thrusterParticle.main;

        mainAudioSource.time = Random.Range(0, mainAudioSource.clip.length);
    }


    private void OnDestroy()
    {
        thrust.action.Disable();
        thrust.action.performed -= Thrust_Action_performed;
        thrust.action.canceled -= Thrust_Action_canceled;

        thrusterRotation.action.Disable();
        thrusterRotation.action.started -= ThrusterRotation_Action_started;

        rotate.action.Disable();
        rotate.action.performed -= Rotation_Action_performed;
        rotate.action.canceled -= Rotation_Action_canceled;


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


    private void Rotation_Action_performed(InputAction.CallbackContext obj)
    {
        rotateInput = obj.ReadValue<float>();
    }


    private void Rotation_Action_canceled(InputAction.CallbackContext obj)
    {
        rotateInput = 0f;
    }


    private void Update()
    {
        //UpdateThrustersValues();

        //thrusterBoostManager.Boost(boostValue);
        float normalizedTorqueMagnitude = torqueMagnitude / rotationSensibility;

        mainAudioSource.volume = Mathf.Max(thrustValue, normalizedTorqueMagnitude);
        //mainAudioSource.pitch = 1 + boostValue;

        //Color flameColor = boostValue == 0 ? Color.Lerp(transparentThrust, thrustColor, thrustValue) : Color.Lerp(transparentBoost, boostColor, (boostValue + thrustValue) * 0.5f);


        Color flameColor = Color.Lerp(Color.Lerp(transparentThrust, thrustColor, thrustValue), Color.Lerp(transparentRotate, rotateColor, normalizedTorqueMagnitude), normalizedTorqueMagnitude);
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
            torqueMagnitude = 0f;
            return;
        }

        Vector3 gravityCenter = Camera.main.transform.position + (xrRigMainTransform.up * gravityCenterYOffsetFromCamera);  // Gravity center is set arbitrary at a point vertical to the camera.

        Vector3 from_GravityCenter_To_Controller = controllerTransform.position - gravityCenter;

        Vector3 from_GravityCenter_To_Controller_Direction = from_GravityCenter_To_Controller.normalized;

        Vector3 torqueAxis = Vector3.Cross(from_GravityCenter_To_Controller_Direction, controllerTransform.forward).normalized;

        float dot = Vector3.Dot(from_GravityCenter_To_Controller_Direction, controllerTransform.forward);

        float distance_from_gravityCenter_normalized_clamped01 = Mathf.Clamp01(from_GravityCenter_To_Controller.magnitude / normalDistanceFromGravityCenter);

        float positionRotationFactors = dot > 0 ?    // Higher if controller forward is orthogonal to the axis controller-gravityCenter, and if distance controller-gravityCenter is higher than a minimum value.
            (1 - dot) * distance_from_gravityCenter_normalized_clamped01 :
            (1 + dot) * distance_from_gravityCenter_normalized_clamped01;

        torqueMagnitude = rotateInput * positionRotationFactors * rotationSensibility;

        Vector3 torque = torqueAxis * torqueMagnitude;

        forcesMixer.Rotate(torque, 1 - positionRotationFactors, this);
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
