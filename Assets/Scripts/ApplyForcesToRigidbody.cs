using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyForcesToRigidbody : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Thruster leftThruster;
    [Space(20)]
    [SerializeField] private float thrusterForceFactor = 700f;
    [SerializeField] private float thrusterBoostForceFactor = 700f;
    [Space(20)]
    [SerializeField] private float windFactor = 100f;

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
            Vector3 finalLeftThrustForce = leftThrustForce * thrusterForceFactor;
            Vector3 finalLeftBoostForce = leftBoostForce * thrusterBoostForceFactor;
            
            Vector3 finalRightThrustForce = rightThrustForce * thrusterForceFactor;
            Vector3 finalRightBoostForce = rightBoostForce * thrusterBoostForceFactor;
            
            Vector3 finalWindForce = wind.Vector * windFactor;
            
            float clamp = 1.75f;
            Vector3 totalThrustVector = Vector3.ClampMagnitude(finalLeftThrustForce + finalRightThrustForce, thrusterForceFactor * clamp);            
            Vector3 totalBoostVector = finalLeftBoostForce + finalRightBoostForce;
            
            Vector3 finalVector = wind == null ? totalThrustVector + totalBoostVector : totalThrustVector + totalBoostVector + finalWindForce;

            rb.AddForce(finalVector);

            leftThrusterUpdated = false;
            rightThrusterUpdated = false;
        }
    }
}
