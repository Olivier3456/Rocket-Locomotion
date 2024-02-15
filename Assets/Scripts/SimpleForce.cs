using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleForce : MonoBehaviour
{

    private bool inputDetected;

    private Rigidbody rb;

    [SerializeField] private float forceValue = 1f;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            inputDetected = true;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            inputDetected = false;
        }
    }


    private void FixedUpdate()
    {
        if (inputDetected)
        {
            rb.AddForce(transform.up * forceValue);
        }
    }
}
