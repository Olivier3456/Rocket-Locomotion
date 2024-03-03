using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Rot2_Thrusters_Forces_Mixer : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Rot2_Thruster thrusterLeft;
    [SerializeField] private Rot2_Thruster thrusterRight;
   

    private Vector3 leftTorqueValue;
    private Vector3 leftThrustValue;
    private float leftThrusterPosRotFactors;
    private bool isLeftTorqueUpdated;
    private bool isLeftThrustUpdated;

    private Vector3 rightTorqueValue;
    private Vector3 rightThrustValue;
    private float rightThrusterPosRotFactors;
    private bool isRightTorqueUpdated;
    private bool isRightThrustUpdated;

    //public TextMeshProUGUI DEBUG_text1;


    public void Rotate(Vector3 torqueValue, float thrustersPosRotFactors, Rot2_Thruster thruster)
    {
        if (thruster == thrusterLeft)
        {
            leftTorqueValue = thrusterLeft.IsForward ? torqueValue : -torqueValue;
            leftThrusterPosRotFactors = thrustersPosRotFactors;
            isLeftTorqueUpdated = true;
        }
        else
        {
            rightTorqueValue = thrusterRight.IsForward ? torqueValue : -torqueValue;
            rightThrusterPosRotFactors = thrustersPosRotFactors;
            isRightTorqueUpdated = true;

            //DEBUG_text1.text = $"Right thruster posRotFactor = {rightThrusterPosRotFactors}";
        }

        Send_Forces_To_Rigidbody_If_All_Updates_Are_Done();
    }



    public void Thrust(Vector3 thrustValue, Rot2_Thruster thruster)
    {
        if (thruster == thrusterLeft)
        {
            leftThrustValue = thrustValue;
            isLeftThrustUpdated = true;
        }
        else
        {
            rightThrustValue = thrustValue;
            isRightThrustUpdated = true;
        }

        Send_Forces_To_Rigidbody_If_All_Updates_Are_Done();
    }



    public Vector3 ComputeRealThrustForce(Vector3 thrustValue, float thrusterPosRotFactors)
    {
        return thrustValue * thrusterPosRotFactors;
    }

    private void Send_Forces_To_Rigidbody_If_All_Updates_Are_Done()
    {
        if (isLeftTorqueUpdated && isRightTorqueUpdated && isLeftThrustUpdated && isRightThrustUpdated)
        {
            isLeftThrustUpdated = false;
            isLeftTorqueUpdated = false;
            isRightThrustUpdated = false;
            isRightTorqueUpdated = false;

            Vector3 totalThrustForce = ComputeRealThrustForce(leftThrustValue, leftThrusterPosRotFactors) + ComputeRealThrustForce(rightThrustValue, rightThrusterPosRotFactors);
            rb.AddForce(totalThrustForce);

            Vector3 totalTorqueForce = leftTorqueValue + rightTorqueValue;
            rb.AddTorque(totalTorqueForce, ForceMode.Force);
        }
    }
}
