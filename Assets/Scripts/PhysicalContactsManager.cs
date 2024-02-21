using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PhysicalContactsManager : MonoBehaviour
{
    [SerializeField] private CapsuleCollider playerCollider;
    [SerializeField] private Transform playerFeetTransform;
    [SerializeField] private Camera cam;
    [SerializeField] private Transform xrOrigin;
    //[SerializeField] private Rigidbody rb;
    [SerializeField] private LayerMask groundLayerMask;
    [Space(20)]
    public UnityEvent<bool> OnGrounded = new UnityEvent<bool>();
    //public UnityEvent<float> OnPhysicalShock = new UnityEvent<float>();
    public UnityEvent<GameObject, Vector3, Vector3> OnCollision = new UnityEvent<GameObject, Vector3, Vector3>();


    private bool isGrounded;
    public bool IsGrounded { get { return isGrounded; } }

    //private float lastVelocitySqrtMagnitude = 0;

    //private float dangerousVelocityDifference = 1000;


    private void FixedUpdate()
    {
        UpdateCollider();
        UpdatePlayerFeetPosition();        
        GroundCheck();
        //VelocityCheck();
    }


    //private void VelocityCheck()
    //{
    //    Vector3 velocity = rb.velocity;
    //    float velocitySqrtMagnitude = velocity.sqrMagnitude;

    //    float velocityDifference = Mathf.Abs(velocitySqrtMagnitude - lastVelocitySqrtMagnitude);        

    //    if (velocityDifference > dangerousVelocityDifference)
    //    {
    //        Debug.Log($"Violent shock detected: velocity difference = {velocityDifference}");
    //        OnPhysicalShock.Invoke(velocityDifference);
    //    }

    //    lastVelocitySqrtMagnitude = velocitySqrtMagnitude;
    //}

    

    private void UpdateCollider()
    {
        Vector3 camOffset = cam.transform.position - xrOrigin.position;

        //playerCollider.height = (camOffset.y * 2f) - (playerCollider.radius * 2);

        float yOffset = (playerCollider.height * 0.5f) - playerCollider.radius;

        Vector3 position = new Vector3(camOffset.x, yOffset, camOffset.z);

        playerCollider.center = position;
    }


    private void UpdatePlayerFeetPosition()
    {
        float playerFeetYOffset = (playerCollider.height * 0.5f);

        Vector3 playerFeetPosition = new Vector3(playerCollider.center.x, playerCollider.center.y - playerFeetYOffset, playerCollider.center.z);

        playerFeetTransform.localPosition = playerFeetPosition;
    }


    private void GroundCheck()
    {
        if (Physics.CheckSphere(playerFeetTransform.position, 0.1f, groundLayerMask))
        {
            if (!isGrounded)
            {
                isGrounded = true;
                OnGrounded.Invoke(isGrounded);
                Debug.Log("Player is grounded");
            }
        }
        else
        {
            if (isGrounded)
            {
                isGrounded = false;
                OnGrounded.Invoke(isGrounded);
                Debug.Log("Player is not grounded anymore.");
            }
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        Vector3 relativeVelocity = collision.relativeVelocity;
        Vector3 collisionNormal = collision.contacts[0].normal;
        Debug.Log($"Number of contact points of the collision: {collision.contacts.Length}. Collision's normal vector (for the first contact point): {collisionNormal}");

        //Vector3 contactNormal = collision.contacts[0].normal;
        OnCollision.Invoke(collision.gameObject, relativeVelocity, collisionNormal);
    }



    //public void SetDangerousVelocityDifference(float threshold)
    //{
    //    dangerousVelocityDifference = threshold;
    //}
}
