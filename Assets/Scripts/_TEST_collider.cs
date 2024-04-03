using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _TEST_collider : MonoBehaviour
{

    

    void Update()
    {
        if (Physics.CheckSphere(transform.position, 0.5f))
        {
            Debug.Log("Collider or trigger detected.");
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log("trigger entered");
    //}

    //private void OnTriggerStay(Collider other)
    //{
    //    Debug.Log("trigger stay");
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    Debug.Log("trigger exited");
    //}
}
