using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Rot_PhysicalContactsManager : MonoBehaviour
{
    [SerializeField] private SphereCollider playerCollider;
    [SerializeField] private Camera cam;
    [SerializeField] private Transform xrOrigin;
    [SerializeField] private LayerMask groundLayerMask;
    [Space(20)]
    public UnityEvent<GameObject, Vector3, Vector3> OnCollision = new UnityEvent<GameObject, Vector3, Vector3>();

        
    private void FixedUpdate()
    {
        UpdateColliderPosition();
    }        


    private void UpdateColliderPosition()
    {
        Vector3 camOffset = cam.transform.position - xrOrigin.position;
        playerCollider.center = camOffset;
    }
       
    
    private void OnCollisionEnter(Collision collision)
    {
        Vector3 relativeVelocity = collision.relativeVelocity;
        Vector3 collisionNormal = collision.contacts[0].normal;
        Debug.Log($"Number of contact points of the collision: {collision.contacts.Length}. Collision's normal vector (for the first contact point): {collisionNormal}");

        OnCollision.Invoke(collision.gameObject, relativeVelocity, collisionNormal);
    }
}
