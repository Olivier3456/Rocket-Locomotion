using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PhysicalContactsManager : MonoBehaviour
{
    [SerializeField] private Transform colliderTransform;
    [SerializeField] private Transform playerFeetTransform;
    [SerializeField] private CapsuleCollider playerCollider;
    [SerializeField] private Camera cam;
    [SerializeField] private Transform xrOrigin;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private LayerMask groundLayerMask;
    [Space(20)]
    public UnityEvent<bool> IsGroundedEvent = new UnityEvent<bool>();
    public UnityEvent<float> OnPhysicalShock = new UnityEvent<float>();

    
    private bool isGrounded;
    public bool IsGrounded { get { return isGrounded; } }


    private Vector3 lastVelocity = Vector3.zero;
    private float lastVelocitySqrtMagnitude = 0;


    private float highestVelocityDifference = 0;

    private float dangerousVelocityDifference = 1000;


    private void FixedUpdate()
    {
        UpdateCollider();
        UpdatePlayerFeetPosition();        
        GroundCheck();
        VelocityCheck();
    }


    private void VelocityCheck()
    {
        Vector3 velocity = rb.velocity;
        float velocitySqrtMagnitude = velocity.sqrMagnitude;

        float velocityDifference = Mathf.Abs(velocitySqrtMagnitude - lastVelocitySqrtMagnitude);
        

        if (velocityDifference > dangerousVelocityDifference)
        {
            Debug.Log($"Violent shock detected: velocity difference = {velocityDifference}");
            OnPhysicalShock.Invoke(velocityDifference);
        }


        lastVelocity = velocity;
        lastVelocitySqrtMagnitude = velocitySqrtMagnitude;
    }

    

    private void UpdateCollider()
    {
        float camYoffset = cam.transform.position.y - xrOrigin.position.y;
        playerCollider.height = (camYoffset * 2f) - (playerCollider.radius * 2);
        //halfColliderHeight = (capsuleCollider.height * 0.5f) + capsuleCollider.radius;

        float yOffset = camYoffset - ((playerCollider.height * 0.5f) - playerCollider.radius);
        Vector3 position = new Vector3(cam.transform.position.x, xrOrigin.position.y + yOffset, cam.transform.position.z);

        colliderTransform.position = position;
    }


    private void UpdatePlayerFeetPosition()
    {
        float playerFeetYOffset = (playerCollider.height * 0.5f);

        Vector3 playerFeetPosition = new Vector3(colliderTransform.position.x, colliderTransform.position.y - playerFeetYOffset, colliderTransform.position.z);

        playerFeetTransform.position = playerFeetPosition;
    }


    private void GroundCheck()
    {
        if (Physics.CheckSphere(playerFeetTransform.position, 0.1f, groundLayerMask))
        {
            if (!isGrounded)
            {
                isGrounded = true;
                IsGroundedEvent.Invoke(isGrounded);
                Debug.Log("Player is grounded");
            }
        }
        else
        {
            if (isGrounded)
            {
                isGrounded = false;
                IsGroundedEvent.Invoke(isGrounded);
                Debug.Log("Player is not grounded anymore.");
            }
        }
    }

    public void SetDangerousVelocityDifference(float threshold)
    {
        dangerousVelocityDifference = threshold;
    }
}
