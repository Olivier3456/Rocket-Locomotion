using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TEST_2_Rotations : MonoBehaviour
{

    [SerializeField] private Rigidbody rb;
    [Space(20)]
    [SerializeField] private Transform controllerTransform;
    [Space(20)]
    [SerializeField] private float rotationSensibility = 0.1f;

    public Transform DEBUG_ControllerYOffsetVisualMarker;

    private Transform gravityCenter;

    private bool isRotating;

    private float inputValue;



    private void Start()
    {
        gravityCenter = rb.transform;
    }



    private void FixedUpdate()
    {
        bool up = true;
        Transform camTransform = rb.transform;

        Vector3 fromCamToController = controllerTransform.position - camTransform.position;
        float fromCamToControllerDistance = fromCamToController.magnitude;
        float angleFromCameraUpToControllerVector = Vector3.Angle(fromCamToController, camTransform.up);

        if (angleFromCameraUpToControllerVector > 90f)
        {
            angleFromCameraUpToControllerVector = 90f - (angleFromCameraUpToControllerVector - 90f);
            up = false;
        }

        float angleInRadians = angleFromCameraUpToControllerVector * Mathf.PI / 180;
        float cos = Mathf.Cos(angleInRadians);

        // Dans un triangle rectangle, coté adjacent à un angle (autre que l'angle droit) = hypotenuse * cos(angle)
        float controllerVerticalOffset = fromCamToControllerDistance * cos;

        Vector3 gravityCenterPosition = up ? camTransform.position + (camTransform.up * controllerVerticalOffset) : camTransform.position - (camTransform.up * controllerVerticalOffset);

        Debug.DrawLine(controllerTransform.position, camTransform.position);
        Debug.DrawLine(camTransform.position + (camTransform.up * 2), camTransform.position - (camTransform.up * 2));
        Debug.DrawLine(controllerTransform.position, gravityCenterPosition);
        DEBUG_ControllerYOffsetVisualMarker.position = gravityCenterPosition;
    }
}
