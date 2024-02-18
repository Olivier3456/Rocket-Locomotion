using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] private Transform colliderTransform;
    [SerializeField] private Transform playerFeetTransform;
    [SerializeField] private CapsuleCollider playerCollider;
    [SerializeField] private Camera cam;
    [SerializeField] private Transform xrOrigin;
    [SerializeField] private LayerMask groundLayerMask;



    private bool isGrounded;
    public bool IsGrounded { get { return isGrounded; } }

    public UnityEvent<bool> IsGroundedEvent = new UnityEvent<bool>();


    private void FixedUpdate()
    {
        UpdateCollider();

        UpdatePlayerFeetPosition();



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
}
