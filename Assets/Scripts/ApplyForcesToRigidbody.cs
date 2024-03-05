using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyForcesToRigidbody : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Thruster leftThruster;
    
    private Wind wind;

    private Vector3 leftThrustForce;
    private Vector3 rightThrustForce;

    private Vector3 leftBoostForce;
    private Vector3 rightBoostForce;

    private bool leftThrusterUpdated = false;
    private bool rightThrusterUpdated = false;

    private void Start()
    {
        wind = FindObjectOfType<Wind>();
    }


    public void SetThrusterForceVector(Thruster thruster, Vector3 thrustForce, Vector3 boostForce)
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
            Vector3 finalVector = wind == null ? totalThrustVector + totalBoostVector : totalThrustVector + totalBoostVector + wind.Vector;

            rb.AddForce(finalVector);

            leftThrusterUpdated = false;
            rightThrusterUpdated = false;
        }
    }
}
