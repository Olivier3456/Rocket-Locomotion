using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;


public class Thruster : MonoBehaviour
{
    [SerializeField] private PlayerLife playerLife;
    [Space(20)]
    [SerializeField] private ApplyThrustersForceToRigidbody applyThrustersForceToRigidbody;
    [Space(20)]
    [SerializeField] private ThrustersBoostManager thrustersBoostManager;
    [Space(20)]
    [SerializeField] private Transform controllerTransform;
    [Space(20)]
    [SerializeField] private Transform thrusterVisual;
    [Space(20)]
    [SerializeField] private InputActionReference thrust;
    [SerializeField] private InputActionReference boost;
    [SerializeField] private InputActionReference rotate;
    [Space(20)]
    [SerializeField] private float thrusterBaseForceFactor = 700f;
    [SerializeField] private float thrusterBoostForceFactor = 700f;
    [Space(20)]
    [SerializeField] private AudioSource mainAudioSource;
    [SerializeField] private AudioSource rotateAudioSource;
    [Space(20)]
    [SerializeField] private ParticleSystem thrusterParticle;
    [Space(20)]
    [SerializeField] private AnimationCurve thrusterRotationAnimCurve;
    [SerializeField] private AnimationCurve thrusterAudioAnimCurve;

    private bool isForward = true;

    private float thrustInput = 0;
    private float boostInput = 0;

    private float thrustValue = 0;
    private float boostValue = 0;

    private bool canThrust = true;
    private bool canBoost = true;

    private ParticleSystem.MainModule particleMainModule;

    private Color transparentThrust = new Color(1, 1, 1, 0);
    private Color thrustColor = new Color(1, 1, 1, 0.2f);          // White.
    private Color transparentBoost = new Color(0.9f, 0.1f, 0, 0f);
    private Color boostColor = new Color(0.9f, 0.1f, 0, 0.75f);    //Red with a bit of orange.

    public float ThrusterBaseForceFactor { get { return thrusterBaseForceFactor; } }
    public float BoostValue { get { return boostValue; } }


    void Start()
    {
        thrust.action.performed += Thrust_Action_performed;
        thrust.action.canceled += Thrust_Action_canceled;
        thrust.action.Enable();

        rotate.action.started += Rotate_Action_started;
        rotate.action.Enable();

        boost.action.performed += Boost_Action_performed;
        boost.action.canceled += Boost_Action_canceled;
        boost.action.Enable();

        particleMainModule = thrusterParticle.main;

        mainAudioSource.time = Random.Range(0, mainAudioSource.clip.length);
    }


    private void OnDestroy()
    {
        thrust.action.Disable();
        thrust.action.performed -= Thrust_Action_performed;
        thrust.action.canceled -= Thrust_Action_canceled;

        rotate.action.Disable();
        rotate.action.started -= Rotate_Action_started;


        boost.action.Disable();
        boost.action.performed -= Boost_Action_performed;
        boost.action.canceled -= Boost_Action_canceled;
    }


    private void Thrust_Action_performed(InputAction.CallbackContext obj)
    {
        thrustInput = obj.ReadValue<float>();
    }
    private void Thrust_Action_canceled(InputAction.CallbackContext obj)
    {
        thrustInput = 0f;
    }

    private void Rotate_Action_started(InputAction.CallbackContext obj)
    {
        if (canThrust)
        {
            isForward = !isForward;
            StartCoroutine(Rotate());
        }
    }

    private void Boost_Action_performed(InputAction.CallbackContext obj)
    {
        boostInput = obj.ReadValue<float>();
    }
    private void Boost_Action_canceled(InputAction.CallbackContext obj)
    {
        boostInput = 0f;
    }


    private void Update()
    {
        UpdateThrustersValues();

        thrustersBoostManager.Boost(this, boostValue);

        mainAudioSource.volume = thrustValue;
        mainAudioSource.pitch = 1 + boostValue;

        //Color flameColor = Color.Lerp(transparentThrust, thrustColor, thrustValue);
        //flameColor = Color.Lerp(flameColor, boostColor, boostValue);

        Color flameColor = boostValue == 0 ? Color.Lerp(transparentThrust, thrustColor, thrustValue) : Color.Lerp(transparentBoost, boostColor, (boostValue + thrustValue) * 0.5f);
        particleMainModule.startColor = flameColor;
    }


    void FixedUpdate()
    {
        if (!playerLife.IsAlive)
        {
            return;
        }

        UpdateThrustersValues();
                
        Vector3 thrustVector;
        Vector3 boostVector;

        thrustVector = controllerTransform.forward * thrustValue * thrusterBaseForceFactor;
        boostVector = controllerTransform.forward * boostValue * thrusterBoostForceFactor;

        if (!isForward)
        {
            thrustVector *= -1;
            boostVector *= -1;
        }

        applyThrustersForceToRigidbody.SetForceVector(this, thrustVector, boostVector);
    }


    private void UpdateThrustersValues()
    {
        if (!playerLife.IsAlive)
        {
            thrustValue = 0f;
            boostValue = 0f;
            return;
        }

        boostValue = canThrust && canBoost ? boostInput : 0f;

        if (boostValue == 0)
        {
            thrustValue = canThrust ? thrustInput : 0f;
        }
        else // If the thruster boosts, even a little, its base force vector is at max.
        {
            thrustValue = 1;
        }
    }


    private IEnumerator Rotate()
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


    public void AuthoriseBoost(bool canBoost)
    {
        this.canBoost = canBoost;
    }
}
