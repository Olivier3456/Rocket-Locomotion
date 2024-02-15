using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class GetForcesFromControllers : MonoBehaviour
{
    [SerializeField] private PlayerLife playerLife;
    [Space(20)]
    [SerializeField] private Transform leftControllerTransform;
    [SerializeField] private Transform rightControllerTransform;
    [Space(20)]
    [SerializeField] private InputActionReference leftForwardThruster;
    [SerializeField] private InputActionReference leftBackwardThruster;
    [SerializeField] private InputActionReference rightForwardThruster;
    [SerializeField] private InputActionReference rightBackwardThruster;
    [Space(20)]
    [SerializeField] private float thrusterForceFactor = 750f;
    [Space(20)]
    //[SerializeField] private Transform gravityCenter;
    [SerializeField] private Rigidbody rb;
    [Space(20)]
    [SerializeField] private AudioSource leftThrusterAudioSource;
    [SerializeField] private AudioSource rightThrusterAudioSource;
    [Space(20)]
    [SerializeField] private ParticleSystem leftForwardParticle;
    [SerializeField] private ParticleSystem rightForwardParticle;
    [SerializeField] private ParticleSystem leftBackwardParticle;
    [SerializeField] private ParticleSystem rightBackwardParticle;

    [Space(40)]
    public bool debug;
    public Canvas canvas;
    public TextMeshProUGUI rightText;
    public TextMeshProUGUI leftText;


    private float leftForwardValue = 0;
    private float leftBackwardValue = 0;
    private float rightForwardValue = 0;
    private float rightBackwardValue = 0;

    private ParticleSystem.MainModule leftForwardMain;
    private ParticleSystem.MainModule rightForwardMain;
    private ParticleSystem.EmissionModule leftForwardEm;
    private ParticleSystem.EmissionModule rightForwardEm;

    private ParticleSystem.MainModule leftBackwardMain;
    private ParticleSystem.MainModule rightBackwardMain;
    private ParticleSystem.EmissionModule leftBackwardEm;
    private ParticleSystem.EmissionModule rightBackwardEm;


    void Start()
    {
        leftForwardThruster.action.performed += LeftForwardThruster_Action_performed;
        leftForwardThruster.action.canceled += LeftForwardThruster_Action_canceled;
        leftForwardThruster.action.Enable();

        rightForwardThruster.action.performed += RightForwardThruster_Action_performed;
        rightForwardThruster.action.canceled += RightForwardThruster_Action_canceled;
        rightForwardThruster.action.Enable();

        leftBackwardThruster.action.performed += LeftBackwardThruster_Action_performed;
        leftBackwardThruster.action.canceled += LeftBackwardThruster_Action_canceled;
        leftBackwardThruster.action.Enable();

        rightBackwardThruster.action.performed += RightBackwardThruster_Action_performed;
        rightBackwardThruster.action.canceled += RightBackwardThruster_Action_canceled;
        rightBackwardThruster.action.Enable();


        leftForwardMain = leftForwardParticle.main;
        rightForwardMain = rightForwardParticle.main;
        leftForwardEm = leftForwardParticle.emission;
        rightForwardEm = rightForwardParticle.emission;

        leftBackwardMain = leftBackwardParticle.main;
        rightBackwardMain = rightBackwardParticle.main;
        leftBackwardEm = leftBackwardParticle.emission;
        rightBackwardEm = rightBackwardParticle.emission;


        if (debug) canvas.gameObject.SetActive(true);
        else canvas.gameObject.SetActive(false);
    }


    private void LeftForwardThruster_Action_performed(InputAction.CallbackContext obj)
    {
        leftForwardValue = obj.ReadValue<float>();
    }
    private void LeftForwardThruster_Action_canceled(InputAction.CallbackContext obj)
    {
        leftForwardValue = 0f;
    }
    private void RightForwardThruster_Action_performed(InputAction.CallbackContext obj)
    {
        rightForwardValue = obj.ReadValue<float>();
    }
    private void RightForwardThruster_Action_canceled(InputAction.CallbackContext obj)
    {
        rightForwardValue = 0f;
    }
    private void LeftBackwardThruster_Action_performed(InputAction.CallbackContext obj)
    {
        leftBackwardValue = obj.ReadValue<float>();
    }
    private void LeftBackwardThruster_Action_canceled(InputAction.CallbackContext obj)
    {
        leftBackwardValue = 0f;
    }
    private void RightBackwardThruster_Action_performed(InputAction.CallbackContext obj)
    {
        rightBackwardValue = obj.ReadValue<float>();
    }
    private void RightBackwardThruster_Action_canceled(InputAction.CallbackContext obj)
    {
        rightBackwardValue = 0f;
    }



    private void Update()
    {
        if (playerLife.IsAlive)
        {
            leftThrusterAudioSource.volume = Mathf.Max(leftForwardValue, leftBackwardValue);
            rightThrusterAudioSource.volume = Mathf.Max(rightForwardValue, rightBackwardValue);

            float particleValueMultiplier = 20f;
            leftForwardMain.startSpeed = leftForwardValue * particleValueMultiplier;
            leftForwardEm.rateOverTime = leftForwardValue * particleValueMultiplier;

            rightForwardMain.startSpeed = rightForwardValue * particleValueMultiplier;
            rightForwardEm.rateOverTime = rightForwardValue * particleValueMultiplier;

            leftBackwardMain.startSpeed = leftBackwardValue * particleValueMultiplier;
            leftBackwardEm.rateOverTime = leftBackwardValue * particleValueMultiplier;

            rightBackwardMain.startSpeed = rightBackwardValue * particleValueMultiplier;
            rightBackwardEm.rateOverTime = rightBackwardValue * particleValueMultiplier;
        }
    }



    void FixedUpdate()
    {
        if (!playerLife.IsAlive)
        {
            return;
        }

        float leftTotalInputValue = leftForwardValue - leftBackwardValue;
        float rightTotalInputValue = rightForwardValue - rightBackwardValue;

        if (leftTotalInputValue == 0 && rightTotalInputValue == 0)
        {
            return;
        }

        Vector3 leftPushVector = -leftControllerTransform.forward * leftTotalInputValue * thrusterForceFactor;
        Vector3 rightPushVector = -rightControllerTransform.forward * rightTotalInputValue * thrusterForceFactor;

        Vector3 totalPushVector = leftPushVector + rightPushVector;

        float clamp = 1.5f;
        Vector3 clampedPushVector = Vector3.ClampMagnitude(totalPushVector, thrusterForceFactor * clamp);

        rb.AddForce(clampedPushVector);
    }




    //private void AddTorqueFromController(Transform controller, float inputValue)
    //{
    //    Vector3 direction_From_Controller_To_GravityCenter = (gravityCenter.position - controller.position).normalized;
    //    Vector3 torqueAxis = Vector3.Cross(direction_From_Controller_To_GravityCenter, controller.forward).normalized;

    //    Debug.DrawRay(controller.position, direction_From_Controller_To_GravityCenter, Color.red);
    //    Debug.DrawRay(controller.position, controller.forward, Color.blue);
    //    Debug.DrawRay(gravityCenter.position, torqueAxis * 10, Color.green);
    //    Debug.DrawRay(gravityCenter.position, -torqueAxis * 10, Color.green);

    //    float angleFactor = 1 - Mathf.Abs(Vector3.Dot(direction_From_Controller_To_GravityCenter, controller.forward));
    //    float torque = inputValue * angleFactor * rotationForceFactor;

    //    rb.AddTorque(torqueAxis * torque);
    //}









    //void FixedUpdate()
    //{
    //    AddTorqueFromController(leftControllerTransform, leftForwardValue);
    //    AddForceFromController(leftControllerTransform, leftForwardValue);
    //    //AddTorqueFromController(rightControllerTransform, rightForwardValue);
    //    //AddForceFromController(rightControllerTransform, rightForwardValue);
    //}


    //private void AddForceFromController(Transform controller, float inputValue)
    //{
    //    Vector3 direction_From_Controller_To_GravityCenter = (gravityCenter.position - controller.position).normalized;
    //    float angleFactor = Mathf.Abs(Vector3.Dot(direction_From_Controller_To_GravityCenter, controller.forward));
    //    float force = inputValue * angleFactor * thrusterForceFactor;

    //    rb.AddForceFromController(-controller.forward * force);
    //}


    //private void AddTorqueFromController(Transform controller, float inputValue)
    //{
    //    Vector3 direction_From_Controller_To_GravityCenter = (gravityCenter.position - controller.position).normalized;
    //    Vector3 torqueAxis = Vector3.Cross(direction_From_Controller_To_GravityCenter, controller.forward).normalized;

    //    Debug.DrawRay(controller.position, direction_From_Controller_To_GravityCenter, Color.red);
    //    Debug.DrawRay(controller.position, controller.forward, Color.blue);
    //    Debug.DrawRay(gravityCenter.position, torqueAxis * 10, Color.green);
    //    Debug.DrawRay(gravityCenter.position, -torqueAxis * 10, Color.green);

    //    float angleFactor = 1 - Mathf.Abs(Vector3.Dot(direction_From_Controller_To_GravityCenter, controller.forward));
    //    float torque = inputValue * angleFactor * rotationForceFactor;

    //    rb.AddTorqueFromController(torqueAxis * torque);
    //}
}
