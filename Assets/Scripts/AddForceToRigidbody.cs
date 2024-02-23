using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddForceToRigidbody : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Thruster leftThruster;

    private Vector3 leftThrustForce = Vector3.zero;
    private Vector3 rightThrustForce = Vector3.zero;

    private Vector3 leftBoostForce = Vector3.zero;
    private Vector3 rightBoostForce = Vector3.zero;

    private float thrustersUpdated = 0;

    public void SetForceVector(Thruster thruster, Vector3 thrustForce, Vector3 boostForce)
    {
        thrustersUpdated++;

        if (thruster == leftThruster)
        {
            leftThrustForce = thrustForce;
            leftBoostForce = boostForce;
        }
        else
        {
            rightThrustForce = thrustForce;
            rightBoostForce = boostForce;
        }

        if (thrustersUpdated == 2)
        {
            thrustersUpdated = 0;

            float clamp = 1.75f;
            Vector3 totalThrustVector = Vector3.ClampMagnitude(leftThrustForce + rightThrustForce, leftThruster.ThrusterBaseForceFactor * clamp);
            Vector3 totalBoostVector = leftBoostForce + rightBoostForce;
            Vector3 finalVector = totalThrustVector + totalBoostVector;

            rb.AddForce(finalVector);
        }
    }
}
