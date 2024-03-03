using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST_Rotations : MonoBehaviour
{
    public Transform initialRotationTransform;
    public Transform rotationTransform;
    public Rigidbody rigidbodyToRotate;

    public float rotationSensibility = 0.1f;



    void FixedUpdate()
    {

        Vector3 rotationToApply = (Quaternion.Inverse(initialRotationTransform.localRotation) * rotationTransform.localRotation).eulerAngles;

        CorrectAngle(ref rotationToApply.x);
        CorrectAngle(ref rotationToApply.y);
        CorrectAngle(ref rotationToApply.z);

        rigidbodyToRotate.AddRelativeTorque(rotationToApply * rotationSensibility);

    }


    private float CorrectAngle(ref float angle)
    {
        if (angle < -180) angle += 360;
        else if (angle > 180) angle -= 360;
        return angle;
    }
}
