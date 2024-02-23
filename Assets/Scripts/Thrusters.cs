using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Thrusters : MonoBehaviour
{
    [SerializeField] private PlayerLife playerLife;
    [Space(20)]
    [SerializeField] private Transform leftControllerTransform;
    [SerializeField] private Transform rightControllerTransform;
    [Space(20)]
    [SerializeField] private Transform leftThrusterVisual;
    [SerializeField] private Transform rightThrusterVisual;
    [Space(20)]
    [SerializeField] private InputActionReference leftThruster;
    [SerializeField] private InputActionReference leftRotateThruster;
    [SerializeField] private InputActionReference leftBoost;
    [SerializeField] private InputActionReference rightThruster;
    [SerializeField] private InputActionReference rightRotateThruster;
    [SerializeField] private InputActionReference rightBoost;
    [Space(20)]
    [SerializeField] private float thrusterBaseForceFactor = 700f;
    [SerializeField] private float thrusterBoostForceFactor = 700f;
    [Space(20)]
    [SerializeField] private Rigidbody rb;
    [Space(20)]
    [SerializeField] private AudioSource leftThrusterAudioSource;
    [SerializeField] private AudioSource leftThrusterRotateAudioSource;
    [SerializeField] private AudioSource rightThrusterAudioSource;
    [SerializeField] private AudioSource rightThrusterRotateAudioSource;
    [Space(20)]
    [SerializeField] private ParticleSystem leftThrusterParticle;
    [SerializeField] private ParticleSystem rightThrusterParticle;
    [Space(20)]
    [SerializeField] private AnimationCurve thrusterRotationAnimCurve;
    [SerializeField] private AnimationCurve thrusterAudioAnimCurve;
    [Space(40)]
    public bool debug;
    public Canvas canvas;
    public TextMeshProUGUI rightText;
    public TextMeshProUGUI leftText;


    private float leftBaseInput = 0;
    private float rightBaseInput = 0;
    private float leftBoostInput = 0;
    private float rightBoostInput = 0;

    private float leftBaseValue = 0;
    private float rightBaseValue = 0;
    private float leftBoostValue = 0;
    private float rightBoostValue = 0;

    private bool isLeftForward = true;
    private bool isRightForward = true;

    private bool canLeftThrust = true;
    private bool canRightThrust = true;

    private bool canLeftBoost = true;
    private bool canRightBoost = true;


    private ParticleSystem.MainModule leftThrusterMain;
    private ParticleSystem.MainModule rightThrusterMain;

    private Color transparent = new Color(1, 1, 1, 0);
    private Color maxOpacity_NoBoost = new Color(1, 1, 1, 0.2f);
    private Color boostColor = new Color(0.9f, 0.1f, 0, 0.75f);    //Red


    void Start()
    {
        leftThruster.action.performed += LeftThruster_Action_performed;
        leftThruster.action.canceled += LeftThruster_Action_canceled;
        leftThruster.action.Enable();

        rightThruster.action.performed += RightThruster_Action_performed;
        rightThruster.action.canceled += RightThruster_Action_canceled;
        rightThruster.action.Enable();

        leftRotateThruster.action.started += LeftRotate_Action_started;
        leftRotateThruster.action.Enable();

        rightRotateThruster.action.started += RightRotate_Action_started;
        rightRotateThruster.action.Enable();

        leftBoost.action.performed += LeftBoost_Action_performed;
        leftBoost.action.canceled += LeftBoost_Action_canceled;
        leftBoost.action.Enable();

        rightBoost.action.performed += RightBoost_Action_performed;
        rightBoost.action.canceled += RightBoost_Action_canceled;
        rightBoost.action.Enable();


        leftThrusterMain = leftThrusterParticle.main;
        rightThrusterMain = rightThrusterParticle.main;


        if (debug) canvas.gameObject.SetActive(true);
        else canvas.gameObject.SetActive(false);

        leftThrusterAudioSource.time = Random.Range(0, leftThrusterAudioSource.clip.length);
        rightThrusterAudioSource.time = Random.Range(0, rightThrusterAudioSource.clip.length);
    }


    private void OnDestroy()
    {
        leftThruster.action.Disable();
        leftThruster.action.performed -= LeftThruster_Action_performed;
        leftThruster.action.canceled -= LeftThruster_Action_canceled;

        rightThruster.action.Disable();
        rightThruster.action.performed -= RightThruster_Action_performed;
        rightThruster.action.canceled -= RightThruster_Action_canceled;

        leftRotateThruster.action.Disable();
        leftRotateThruster.action.started -= LeftRotate_Action_started;

        rightRotateThruster.action.Disable();
        rightRotateThruster.action.started -= RightRotate_Action_started;

        leftBoost.action.Disable();
        leftBoost.action.performed -= LeftBoost_Action_performed;
        leftBoost.action.canceled -= LeftBoost_Action_canceled;

        rightBoost.action.Disable();
        rightBoost.action.performed -= RightBoost_Action_performed;
        rightBoost.action.canceled -= RightBoost_Action_canceled;
    }


    private void LeftThruster_Action_performed(InputAction.CallbackContext obj)
    {
        leftBaseInput = obj.ReadValue<float>();
    }
    private void LeftThruster_Action_canceled(InputAction.CallbackContext obj)
    {
        leftBaseInput = 0f;
    }
    private void RightThruster_Action_performed(InputAction.CallbackContext obj)
    {
        rightBaseInput = obj.ReadValue<float>();
    }
    private void RightThruster_Action_canceled(InputAction.CallbackContext obj)
    {
        rightBaseInput = 0f;
    }
    private void LeftRotate_Action_started(InputAction.CallbackContext obj)
    {
        if (canLeftThrust)
        {
            isLeftForward = !isLeftForward;
            StartCoroutine(RotateThruster(leftThrusterVisual));
        }
    }
    private void RightRotate_Action_started(InputAction.CallbackContext obj)
    {
        if (canRightThrust)
        {
            isRightForward = !isRightForward;
            StartCoroutine(RotateThruster(rightThrusterVisual));
        }
    }
    private void LeftBoost_Action_performed(InputAction.CallbackContext obj)
    {
        leftBoostInput = obj.ReadValue<float>();
    }
    private void LeftBoost_Action_canceled(InputAction.CallbackContext obj)
    {
        leftBoostInput = 0f;
    }
    private void RightBoost_Action_performed(InputAction.CallbackContext obj)
    {
        rightBoostInput = obj.ReadValue<float>();
    }
    private void RightBoost_Action_canceled(InputAction.CallbackContext obj)
    {
        rightBoostInput = 0f;
    }


    private void Update()
    {
        UpdateThrustersValues();

        leftThrusterAudioSource.volume = leftBaseValue;
        leftThrusterAudioSource.pitch = 1 + leftBoostValue;

        rightThrusterAudioSource.volume = rightBaseValue;
        rightThrusterAudioSource.pitch = 1 + rightBoostValue;

        Color flameColorLeft = Color.Lerp(transparent, maxOpacity_NoBoost, leftBaseValue);
        flameColorLeft = Color.Lerp(flameColorLeft, boostColor, leftBoostValue);
        leftThrusterMain.startColor = flameColorLeft;

        Color flameColorRight = Color.Lerp(transparent, maxOpacity_NoBoost, rightBaseValue);
        flameColorRight = Color.Lerp(flameColorRight, boostColor, rightBoostValue);
        rightThrusterMain.startColor = flameColorRight;
    }



    void FixedUpdate()
    {
        if (!playerLife.IsAlive)
        {
            return;
        }

        UpdateThrustersValues();

        if (leftBaseValue == 0 && rightBaseValue == 0 && leftBoostValue == 0 && rightBoostValue == 0)
        {
            return;
        }


        Vector3 leftThrusterVector;
        Vector3 leftBoostVector;
        Vector3 rightThrusterVector;
        Vector3 rightBoostVector;


        leftThrusterVector = leftControllerTransform.forward * leftBaseValue * thrusterBaseForceFactor;
        leftBoostVector = leftControllerTransform.forward * leftBoostValue * thrusterBoostForceFactor;

        if (!isLeftForward)
        {
            leftThrusterVector *= -1;
            leftBoostVector *= -1;
        }


        rightThrusterVector = rightControllerTransform.forward * rightBaseValue * thrusterBaseForceFactor;
        rightBoostVector = rightControllerTransform.forward * rightBoostValue * thrusterBoostForceFactor;

        if (!isRightForward)
        {
            rightThrusterVector *= -1;
            rightBoostVector *= -1;
        }


        float clamp = 1.75f;
        Vector3 totalBaseVector = Vector3.ClampMagnitude(leftThrusterVector + rightThrusterVector, thrusterBaseForceFactor * clamp);
        Vector3 totalBoostVector = leftBoostVector + rightBoostVector;
        Vector3 finalVector = totalBaseVector + totalBoostVector;

        rb.AddForce(finalVector);
    }


    private void UpdateThrustersValues()
    {
        if (!playerLife.IsAlive)
        {
            leftBaseValue = 0f;
            rightBaseValue = 0f;
            leftBoostValue = 0f;
            rightBoostValue = 0f;
            return;
        }

        leftBoostValue = canLeftThrust && canLeftBoost ? leftBoostInput : 0f;
        rightBoostValue = canRightThrust && canRightBoost ? rightBoostInput : 0f;

        if (leftBoostValue == 0)
        {
            leftBaseValue = canLeftThrust ? leftBaseInput : 0f;
        }
        else // If the thruster boosts, even a little, its base force vector is at max.
        {
            leftBaseValue = 1;
        }

        if (rightBoostValue == 0)
        {
            rightBaseValue = canRightThrust ? rightBaseInput : 0f;
        }
        else // If the thruster boosts, even a little, its base force vector is at max.
        {
            rightBaseValue = 1;
        }
    }


    private IEnumerator RotateThruster(Transform transformToRotate)
    {
        Transform hand;
        bool rotatesToForward;
        if (transformToRotate == leftThrusterVisual)
        {
            canLeftThrust = false;
            hand = leftControllerTransform;
            rotatesToForward = isLeftForward;
            leftThrusterRotateAudioSource.Play();
        }
        else
        {
            canRightThrust = false;
            hand = rightControllerTransform;
            rotatesToForward = isRightForward;
            rightThrusterRotateAudioSource.Play();
        }

        float rotationTime = 0.5f;
        float progress = 0f;

        while (progress < 1)
        {
            yield return null;
            progress += Time.deltaTime / rotationTime;
            float value = thrusterRotationAnimCurve.Evaluate(progress);
            transformToRotate.forward = rotatesToForward ? Vector3.Slerp(-hand.forward, hand.forward, value) : Vector3.Slerp(hand.forward, -hand.forward, value);

            if (transformToRotate == leftThrusterVisual)
            {
                leftThrusterRotateAudioSource.pitch = 1 + thrusterAudioAnimCurve.Evaluate(progress);
            }
            else
            {
                rightThrusterRotateAudioSource.pitch = 1 + thrusterAudioAnimCurve.Evaluate(progress);
            }
        }

        transformToRotate.forward = rotatesToForward ? hand.forward : -hand.forward;

        if (transformToRotate == leftThrusterVisual)
        {
            canLeftThrust = true;
            leftThrusterRotateAudioSource.Stop();
        }
        else
        {
            canRightThrust = true;
            rightThrusterRotateAudioSource.Stop();
        }
    }
}
