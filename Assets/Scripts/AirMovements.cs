using UnityEngine;

public class AirMovements : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;

    private Wind wind;

    private Vector3 totalVelocity = Vector3.zero;
    private Vector3 airDirection = Vector3.zero;
    private float airSpeed = 0f;

    public Vector3 Velocity { get { return totalVelocity; } }
    public Vector3 Direction { get { return airDirection; } }
    public float Speed { get { return airSpeed; } }


    private void Start()
    {
        wind = FindObjectOfType<Wind>();
    }


    void Update()
    {
        Vector3 playerVelocity = rb.velocity;
        Vector3 windVelocity = wind == null ? Vector3.zero : wind.Vector;
        totalVelocity = playerVelocity - windVelocity;
        airDirection = totalVelocity.normalized;
        airSpeed = totalVelocity.magnitude;       
    }
}
