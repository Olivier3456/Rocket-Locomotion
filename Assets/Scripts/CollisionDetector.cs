using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionDetector : MonoBehaviour
{
    public UnityEvent<GameObject, float> OnCollision = new UnityEvent<GameObject, float>();

    private void OnCollisionEnter(Collision collision)
    {
        float relativeVelocity = collision.relativeVelocity.magnitude;
        Debug.Log($"Relative velocity of the collision: {relativeVelocity}");

        OnCollision.Invoke(collision.gameObject, relativeVelocity);
    }
}
