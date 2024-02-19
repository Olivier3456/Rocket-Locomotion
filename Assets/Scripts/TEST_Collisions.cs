using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST_Collisions : MonoBehaviour
{

    public Rigidbody rb;
    

    void Update()
    {
        Vector3 randomPosChange = new Vector3(Random.value, Random.value, Random.value);
        transform.position += randomPosChange * Time.deltaTime;

        rb.velocity = Vector3.right;
    }



    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Collision. Relative velocity magnitude = {collision.relativeVelocity.magnitude}.");
    }
}
