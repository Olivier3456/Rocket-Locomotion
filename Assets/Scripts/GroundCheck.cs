using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] private Transform playerFeet;
    [SerializeField] private LayerMask groundLayerMask;

    private bool isGrounded;
    public bool IsGrounded { get { return isGrounded; } }

    public UnityEvent<bool> IsGroundedEvent = new UnityEvent<bool>();


    private void FixedUpdate()
    {
        if (Physics.CheckSphere(playerFeet.position, 0.1f, groundLayerMask))
        {
            if (!isGrounded)
            {
                isGrounded = true;
                IsGroundedEvent.Invoke(isGrounded);
            }
        }
        else
        {
            if (isGrounded)
            {
                isGrounded = false;
                IsGroundedEvent.Invoke(isGrounded);
            }
        }
    }
}
