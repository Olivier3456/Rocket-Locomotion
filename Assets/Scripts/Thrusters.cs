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
    [SerializeField] private InputActionReference rightThruster;
    [SerializeField] private InputActionReference rightRotateThruster;
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


    private bool isLeftForward = true;
    private bool isRightForward = true;

    private bool canLeftThrust = true;
    private bool canRightThrust = true;


    private ParticleSystem.MainModule leftThrusterMain;
    private ParticleSystem.MainModule rightThrusterMain;

    private Color transparent = new Color(1, 1, 1, 0);
    private Color maxOpacity = new Color(1, 1, 1, 0.4f);


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


    private void Update()
    {
        float absLeftInput = Mathf.Abs(leftInput);
        float absRightInput = Mathf.Abs(rightInput);

        leftThrusterBoostAudioSource.volume = canLeftThrust && playerLife.IsAlive ? absLeftInput : 0;
        rightThrusterBoostAudioSource.volume = canRightThrust && playerLife.IsAlive ? absRightInput : 0;

        Color flameColorLeft = Color.Lerp(transparent, maxOpacity, absLeftInput);
        leftThrusterMain.startColor = canLeftThrust && playerLife.IsAlive ? flameColorLeft : transparent;

        Color flameColorRight = Color.Lerp(transparent, maxOpacity, absRightInput);
        rightThrusterMain.startColor = canRightThrust && playerLife.IsAlive ? flameColorRight : transparent;
    }



    void FixedUpdate()
    {
        if (!playerLife.IsAlive)
        {
            return;
        }

        if (leftInput == 0 && rightInput == 0)
        {
            return;
        }

        Vector3 leftPushVector = canLeftThrust ? leftControllerTransform.forward * leftInput * thrusterForceFactor : Vector3.zero;
        Vector3 rightPushVector = canRightThrust ? rightControllerTransform.forward * rightInput * thrusterForceFactor : Vector3.zero;

        Vector3 totalPushVector = leftPushVector + rightPushVector;

        float clamp = 1.75f;
        Vector3 clampedPushVector = Vector3.ClampMagnitude(totalPushVector, thrusterForceFactor * clamp);

        rb.AddForce(clampedPushVector);
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
