using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFriction : MonoBehaviour
{
    [SerializeField] private PhysicalContactsManager physicalContactsManager;
    [Space(20)]
    [SerializeField] private PhysicMaterial playerPhysicMaterial;
    [SerializeField] private float staticFrictionWhenGrounded = 5f;
    [SerializeField] private float dynamicFrictionWhenGrounded = 5f;
    [SerializeField] private float staticFrictionWhenflying = 0.6f;
    [SerializeField] private float dynamicFrictionWhenFlying = 0.6f;


    private void Awake()
    {
        physicalContactsManager.IsGroundedEvent.AddListener(OnPlayerGrounded);
    }

    private void OnDisable()
    {
        physicalContactsManager.IsGroundedEvent.RemoveListener(OnPlayerGrounded);
    }

    private void OnPlayerGrounded(bool isGrounded)
    {
        if (isGrounded) 
        {
            playerPhysicMaterial.staticFriction = staticFrictionWhenGrounded;
            playerPhysicMaterial.dynamicFriction = dynamicFrictionWhenGrounded;
        }
        else
        {
            playerPhysicMaterial.staticFriction = staticFrictionWhenflying;
            playerPhysicMaterial.dynamicFriction = dynamicFrictionWhenFlying;
        }
    }
}
