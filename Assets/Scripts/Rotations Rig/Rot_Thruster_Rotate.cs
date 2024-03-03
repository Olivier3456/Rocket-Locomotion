using UnityEngine;
using UnityEngine.InputSystem;


public class Rot_Thruster_Rotate : MonoBehaviour
{
    [SerializeField] private Rot_PlayerLife playerLife;
    [Space(20)]
    [SerializeField] private Rot_Thruster_Forces_Mixer forcesMixer;
    [Space(20)]
    [SerializeField] private Transform controllerTransform;
    [SerializeField] private Transform xrRigMainTransform;
    [Space(20)]
    [SerializeField] private InputActionReference RotateActionReference;
    [Space(20)]
    [SerializeField] private float rotationSensibility = 0.1f;
    [Space(20)]
    [SerializeField] private float gravityCenterYOffsetFromCamera = 0f;


    private float normalDistanceFromGravityCenter = 0.1f;   // Reduces torque force if controller is very near gravity center.

    private float inputValue;

    void Start()
    {
        RotateActionReference.action.performed += Rotation_Action_performed;
        RotateActionReference.action.canceled += Rotation_Action_canceled;
        RotateActionReference.action.Enable();
    }



    private void OnDestroy()
    {
        RotateActionReference.action.Disable();
        RotateActionReference.action.performed -= Rotation_Action_performed;
        RotateActionReference.action.canceled -= Rotation_Action_canceled;
    }


    private void Rotation_Action_performed(InputAction.CallbackContext obj)
    {
        inputValue = obj.ReadValue<float>();
    }


    private void Rotation_Action_canceled(InputAction.CallbackContext obj)
    {
        inputValue = 0f;
    }


    void FixedUpdate()
    {
        if (!playerLife.IsAlive)
        {
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

        float torqueMagnitude = inputValue * positionRotationFactors * rotationSensibility;

        Vector3 torque = torqueAxis * torqueMagnitude;

        forcesMixer.Rotate(torque, 1 - positionRotationFactors, this);
    }
}
