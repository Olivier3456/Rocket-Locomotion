using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionDetector : MonoBehaviour
{
    public UnityEvent<GameObject, Vector3, Vector3> OnCollision = new UnityEvent<GameObject, Vector3, Vector3>();

    [SerializeField] private Transform playerFeet;
    [SerializeField] private LayerMask groundLayerMask;

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 relativeVelocity = collision.relativeVelocity;
        Vector3 collisionNormal = collision.contacts[0].normal;
        Debug.Log($"Number of contact points of the collision: {collision.contacts.Length}. Collision's normal vector (for the first contact point): {collisionNormal}");
        

        //Vector3 contactNormal = collision.contacts[0].normal;
        OnCollision.Invoke(collision.gameObject, relativeVelocity, collisionNormal);
    }


    private void FixedUpdate()
    {
        if (Physics.CheckSphere(playerFeet.position, 0.1f, groundLayerMask))
        {
            //Debug.Log("Collider detected");
        }        
    }

    //// Not working
    //private void OnCollisionStay(Collision collision)
    //{
    //    Debug.Log("collision");
    //}
}
