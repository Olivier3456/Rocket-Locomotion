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
    [SerializeField] private InputActionReference leftForwardThruster;
    [SerializeField] private InputActionReference leftBackwardThruster;
    [SerializeField] private InputActionReference rightForwardThruster;
    [SerializeField] private InputActionReference rightBackwardThruster;
    [Space(20)]
    [SerializeField] private float thrusterForceFactor = 700f;
    [Space(20)]
    [SerializeField] private Rigidbody rb;
    [Space(20)]
    [SerializeField] private AudioSource leftThrusterAudioSource;
    [SerializeField] private AudioSource rightThrusterAudioSource;
    [Space(20)]
    [SerializeField] private ParticleSystem leftThrusterParticle;
    [SerializeField] private ParticleSystem rightThrusterParticle;
    [Space(20)]
    [SerializeField] private AnimationCurve thrusterRotationAnimCurve;
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
    private ParticleSystem.EmissionModule leftThrusterEm;
    private ParticleSystem.EmissionModule rightThrusterEm;


    void Start()
    {
        leftForwardThruster.action.started += LeftForwardThruster_Action_started;
        leftForwardThruster.action.performed += LeftForwardThruster_Action_performed;
        leftForwardThruster.action.canceled += LeftForwardThruster_Action_canceled;
        leftForwardThruster.action.Enable();

        rightForwardThruster.action.started += RightForwardThruster_Action_started;
        rightForwardThruster.action.performed += RightForwardThruster_Action_performed;
        rightForwardThruster.action.canceled += RightForwardThruster_Action_canceled;
        rightForwardThruster.action.Enable();

        leftBackwardThruster.action.started += LeftBackwardThruster_Action_started;
        leftBackwardThruster.action.performed += LeftBackwardThruster_Action_performed;
        leftBackwardThruster.action.canceled += LeftBackwardThruster_Action_canceled;
        leftBackwardThruster.action.Enable();

        rightBackwardThruster.action.started += RightBackwardThruster_Action_started;
        rightBackwardThruster.action.performed += RightBackwardThruster_Action_performed;
        rightBackwardThruster.action.canceled += RightBackwardThruster_Action_canceled;
        rightBackwardThruster.action.Enable();


        leftThrusterMain = leftThrusterParticle.main;
        rightThrusterMain = rightThrusterParticle.main;
        leftThrusterEm = leftThrusterParticle.emission;
        rightThrusterEm = rightThrusterParticle.emission;


        if (debug) canvas.gameObject.SetActive(true);
        else canvas.gameObject.SetActive(false);
    }




    private void LeftForwardThruster_Action_started(InputAction.CallbackContext obj)
    {
        if (!isLeftForward && canLeftThrust && leftInput == 0)
        {
            isLeftForward = true;
            StartCoroutine(RotateThruster(leftThrusterVisual));
        }
    }
    private void LeftForwardThruster_Action_performed(InputAction.CallbackContext obj)
    {
        if (isLeftForward)
        {
            leftInput = obj.ReadValue<float>();
        }
    }
    private void LeftForwardThruster_Action_canceled(InputAction.CallbackContext obj)
    {
        if (isLeftForward)
        {
            leftInput = 0f;
        }
    }


    private void RightForwardThruster_Action_started(InputAction.CallbackContext obj)
    {
        if (!isRightForward && canRightThrust && rightInput == 0)
        {
            isRightForward = true;
            StartCoroutine(RotateThruster(rightThrusterVisual));
        }
    }
    private void RightForwardThruster_Action_performed(InputAction.CallbackContext obj)
    {
        if (isRightForward)
        {
            rightInput = obj.ReadValue<float>();
        }
    }
    private void RightForwardThruster_Action_canceled(InputAction.CallbackContext obj)
    {
        if (isRightForward)
        {
            rightInput = 0f;
        }
    }


    private void LeftBackwardThruster_Action_started(InputAction.CallbackContext obj)
    {
        if (isLeftForward && canLeftThrust && leftInput == 0)
        {
            isLeftForward = false;
            StartCoroutine(RotateThruster(leftThrusterVisual));
        }
    }
    private void LeftBackwardThruster_Action_performed(InputAction.CallbackContext obj)
    {
        if (!isLeftForward)
        {
            leftInput = -obj.ReadValue<float>();
        }
    }
    private void LeftBackwardThruster_Action_canceled(InputAction.CallbackContext obj)
    {
        if (!isLeftForward)
        {
            leftInput = 0f;
        }
    }


    private void RightBackwardThruster_Action_started(InputAction.CallbackContext obj)
    {
        if (isRightForward && canRightThrust && rightInput == 0)
        {
            isRightForward = false;
            StartCoroutine(RotateThruster(rightThrusterVisual));
        }
    }
    private void RightBackwardThruster_Action_performed(InputAction.CallbackContext obj)
    {
        if (!isRightForward)
        {
            rightInput = -obj.ReadValue<float>();
        }
    }
    private void RightBackwardThruster_Action_canceled(InputAction.CallbackContext obj)
    {
        if (!isRightForward)
        {
            rightInput = 0f;
        }
    }



    private void Update()
    {
        leftThrusterAudioSource.volume = canLeftThrust && playerLife.IsAlive ? Mathf.Abs(leftInput) : 0;
        rightThrusterAudioSource.volume = canRightThrust && playerLife.IsAlive ? Mathf.Abs(rightInput) : 0;

        float particleValueMultiplier = 20f;
        leftThrusterMain.startSpeed = canLeftThrust && playerLife.IsAlive ? Mathf.Abs(leftInput) * particleValueMultiplier : 0;
        leftThrusterEm.rateOverTime = canLeftThrust && playerLife.IsAlive ? Mathf.Abs(leftInput) * particleValueMultiplier : 0;

        rightThrusterMain.startSpeed = canRightThrust && playerLife.IsAlive ? Mathf.Abs(rightInput) * particleValueMultiplier : 0;
        rightThrusterEm.rateOverTime = canRightThrust && playerLife.IsAlive ? Mathf.Abs(rightInput) * particleValueMultiplier : 0;
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

        float clamp = 1.5f;
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
            rotatesToForward = isLeftForward ? true : false;
        }
        else
        {
            canRightThrust = false;
            hand = rightControllerTransform;
            rotatesToForward = isRightForward ? true : false;
        }

        float rotationTime = 0.5f;
        float progress = 0f;

        while (progress < 1)
        {
            yield return null;
            progress += Time.deltaTime / rotationTime;
            float value = thrusterRotationAnimCurve.Evaluate(progress);
            transformToRotate.forward = rotatesToForward ? Vector3.Slerp(-hand.forward, hand.forward, value) : Vector3.Slerp(hand.forward, -hand.forward, value);
        }

        transformToRotate.forward = rotatesToForward ? hand.forward : -hand.forward;

        if (transformToRotate == leftThrusterVisual)
        {
            canLeftThrust = true;
        }
        else
        {
            canRightThrust = true;
        }
    }
}
