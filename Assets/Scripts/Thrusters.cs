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
    [SerializeField] private float thrusterForceFactor = 700f;
    [Space(20)]
    [SerializeField] private Rigidbody rb;
    [Space(20)]
    [SerializeField] private AudioSource leftThrusterBoostAudioSource;
    [SerializeField] private AudioSource leftThrusterRotateAudioSource;
    [SerializeField] private AudioSource rightThrusterBoostAudioSource;
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


    private float leftInput = 0;
    private float rightInput = 0;

    private float leftBoostInput = 0;
    private float rightBoostInput = 0;


    private bool isLeftForward = true;
    private bool isRightForward = true;

    private bool canLeftThrust = true;
    private bool canRightThrust = true;


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
        leftInput = isLeftForward ? obj.ReadValue<float>() : -obj.ReadValue<float>();
    }
    private void LeftThruster_Action_canceled(InputAction.CallbackContext obj)
    {
        leftInput = 0f;
    }
    private void RightThruster_Action_performed(InputAction.CallbackContext obj)
    {
        rightInput = isRightForward ? obj.ReadValue<float>() : -obj.ReadValue<float>();
    }
    private void RightThruster_Action_canceled(InputAction.CallbackContext obj)
    {
        rightInput = 0f;
    }
    private void LeftRotate_Action_started(InputAction.CallbackContext obj)
    {
        if (leftInput == 0 && canLeftThrust)
        {
            isLeftForward = !isLeftForward;
            StartCoroutine(RotateThruster(leftThrusterVisual));
        }
    }
    private void RightRotate_Action_started(InputAction.CallbackContext obj)
    {
        if (rightInput == 0 && canRightThrust)
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
        float absLeftInput = Mathf.Abs(leftInput);
        float absRightInput = Mathf.Abs(rightInput);

        leftThrusterBoostAudioSource.volume = canLeftThrust && playerLife.IsAlive ? absLeftInput + leftBoostInput : 0;
        leftThrusterBoostAudioSource.pitch = 1 + leftBoostInput;

        rightThrusterBoostAudioSource.volume = canRightThrust && playerLife.IsAlive ? absRightInput + rightBoostInput : 0;
        rightThrusterBoostAudioSource.pitch = 1 + rightBoostInput;

        Color flameColorLeft = Color.Lerp(transparent, maxOpacity_NoBoost, absLeftInput);
        flameColorLeft = Color.Lerp(flameColorLeft, boostColor, leftBoostInput);
        leftThrusterMain.startColor = canLeftThrust && playerLife.IsAlive ? flameColorLeft : transparent;

        Color flameColorRight = Color.Lerp(transparent, maxOpacity_NoBoost, absRightInput);
        flameColorRight = Color.Lerp(flameColorRight, boostColor, rightBoostInput);
        rightThrusterMain.startColor = canRightThrust && playerLife.IsAlive ? flameColorRight : transparent;
    }



    void FixedUpdate()
    {
        if (!playerLife.IsAlive)
        {
            return;
        }

        if (leftInput == 0 && rightInput == 0 && leftBoostInput == 0 && rightBoostInput == 0)
        {
            return;
        }

        Vector3 leftPushVector = canLeftThrust ? leftControllerTransform.forward * leftInput * (1 + leftBoostInput) * thrusterForceFactor : Vector3.zero;
        Vector3 rightPushVector = canRightThrust ? rightControllerTransform.forward * rightInput * (1 + rightBoostInput) * thrusterForceFactor : Vector3.zero;

        Vector3 totalPushVector = leftPushVector + rightPushVector;

        if (leftBoostInput == 0 && rightBoostInput == 0)
        {
            float clamp = 1.75f;
            totalPushVector = Vector3.ClampMagnitude(totalPushVector, thrusterForceFactor * clamp);
        }

        rb.AddForce(totalPushVector);
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
