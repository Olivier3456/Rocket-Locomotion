using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rot3_Thrusters_Forces_Mixer : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Rot3_Thruster thrusterLeft;
    [SerializeField] private Rot3_Thruster thrusterRight;


    private Vector3 leftThrustValue;
    private bool isLeftThrustUpdated;

    private Vector3 rightThrustValue;
    private bool isRightThrustUpdated;

    //public TextMeshProUGUI DEBUG_text1;



    public void Thrust(Vector3 thrustValue, Rot3_Thruster thruster)
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


    private void Send_Forces_To_Rigidbody_If_All_Updates_Are_Done()
    {
        if (isLeftThrustUpdated && isRightThrustUpdated)
        {
            isLeftThrustUpdated = false;
            isRightThrustUpdated = false;

            Vector3 totalThrustForce = leftThrustValue + rightThrustValue;
            rb.AddForce(totalThrustForce);
        }
    }
}
