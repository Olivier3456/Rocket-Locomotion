using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColliderPlacement : MonoBehaviour
{
    [SerializeField] private CapsuleCollider capsuleCollider;

    [SerializeField] private Transform cam;
    [SerializeField] private Transform xrOrigin;

    //private float halfColliderHeight;
    //private void Awake()
    //{
    //    halfColliderHeight = (capsuleCollider.height * 0.5f) + capsuleCollider.radius;
    //}


    //void Update()
    //{
    //    float camYoffset = camTransform.position.y - xrOrigin.position.y;
    //    capsuleCollider.height = (camYoffset * 2f) - (capsuleCollider.radius * 2);
    //    //halfColliderHeight = (capsuleCollider.height * 0.5f) + capsuleCollider.radius;

    //    float yOffset = camYoffset - ((capsuleCollider.height * 0.5f) - capsuleCollider.radius);
    //    Vector3 position = new Vector3(camTransform.position.x, xrOrigin.position.y + yOffset, camTransform.position.z);

    //    transform.position = position;
    //}




    //void Update()
    //{
    //    float camYoffset = camTransform.position.y - xrOrigin.position.y;
    //    capsuleCollider.height = camYoffset * 2f - capsuleCollider.radius * 2;
    //    halfColliderHeight = (capsuleCollider.height * 0.5f) + capsuleCollider.radius;

    //    float yOffset = camYoffset - halfColliderHeight;
    //    Vector3 position = new Vector3(camTransform.position.x, xrOrigin.position.y + yOffset, camTransform.position.z);

    //    transform.position = position;
    //}
}
