using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TEST_Vector3Dot : MonoBehaviour
{
    public Transform turningObject;
    public Transform dotValueMarker;

    Vector3 originDirection;

    void Start()
    {
        originDirection = turningObject.forward;

        StartCoroutine(TurnCoroutine());
    }



    IEnumerator TurnCoroutine()
    {
        float dot = Vector3.Dot(originDirection, turningObject.forward);
        float lastDot = 1;
        while (dot > 0)
        {
            yield return null;
            lastDot = dot;
            dot = Vector3.Dot(originDirection, turningObject.forward);
            turningObject.Rotate(Vector3.up, Time.deltaTime * 10f);
            dotValueMarker.position += Vector3.forward * dot * Time.deltaTime * 0.1f;
            dotValueMarker.position += Vector3.right * Time.deltaTime * 0.1f;
            Debug.Log($"Dot difference from last frame: {Mathf.Abs(lastDot - dot) * 1000}");
        }
    }


}
