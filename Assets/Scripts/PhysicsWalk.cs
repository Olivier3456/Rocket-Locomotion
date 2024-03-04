using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PhysicsWalk : MonoBehaviour
{
    [SerializeField] private PhysicalContactsManager physicalContactsManager;
    [SerializeField] private PlayerLife playerLife;
    [SerializeField] private InputActionReference moveInputAction;
    [SerializeField] private Transform head;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float walkSpeed;

    private Vector2 moveInput = Vector2.zero;


    private void Awake()
    {
        moveInputAction.action.performed += moveInputAction_performed;
        moveInputAction.action.canceled += moveInputAction_canceled;
        moveInputAction.action.Enable();
    }

    private void OnDestroy()
    {
        moveInputAction.action.Disable();
        moveInputAction.action.performed -= moveInputAction_performed;
        moveInputAction.action.canceled -= moveInputAction_canceled;
    }

    private void moveInputAction_performed(InputAction.CallbackContext obj)
    {
        moveInput = obj.ReadValue<Vector2>();
    }
    private void moveInputAction_canceled(InputAction.CallbackContext obj)
    {
        moveInput = Vector2.zero;
    }

    private void FixedUpdate()
    {
        if (moveInput == Vector2.zero || !physicalContactsManager.IsGrounded || MainManager.Instance.IsPaused || !MainManager.Instance.IsPlayerAlive)
        {
            return;
        }

        Vector3 horizontalHeadForward = new Vector3(head.forward.x, 0, head.forward.z);
        Vector3 horizontalHeadRight = new Vector3(head.right.x, 0, head.right.z);
        
        Vector3 forwardMovement = horizontalHeadForward * moveInput.y * walkSpeed;
        Vector3 rightMovement = horizontalHeadRight * moveInput.x * walkSpeed;

        Vector3 movementVector = forwardMovement + rightMovement;

        rb.velocity = movementVector;       
    }
}
