using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyThrustersForceToRigidbody : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Thruster leftThruster;

    private Vector3 leftThrustForce;
    private Vector3 rightThrustForce;

    private Vector3 leftBoostForce;
    private Vector3 rightBoostForce;

    private bool leftThrusterUpdated = false;
    private bool rightThrusterUpdated = false;

    public void SetForceVector(Thruster thruster, Vector3 thrustForce, Vector3 boostForce)
    {
        if (thruster == leftThruster)
        {
            leftThrustForce = thrustForce;
            leftBoostForce = boostForce;
            leftThrusterUpdated = true;
        }
        else
        {
            rightThrustForce = thrustForce;
            rightBoostForce = boostForce;
            rightThrusterUpdated = true;
        }

        if (leftThrusterUpdated && rightThrusterUpdated)
        {
            float clamp = 1.75f;
            Vector3 totalThrustVector = Vector3.ClampMagnitude(leftThrustForce + rightThrustForce, leftThruster.ThrusterBaseForceFactor * clamp);
            Vector3 totalBoostVector = leftBoostForce + rightBoostForce;
            Vector3 finalVector = totalThrustVector + totalBoostVector;

            rb.AddForce(finalVector);

            leftThrusterUpdated = false;
            rightThrusterUpdated = false;
        }
    }
}
