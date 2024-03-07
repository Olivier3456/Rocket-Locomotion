using System.Collections;
using System.Collections.Generic;
using TMPro;
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


    public TextMeshProUGUI DEBUG_Text1;
    public TextMeshProUGUI DEBUG_Text2;
    public TextMeshProUGUI DEBUG_Text3;



    private void Awake()
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

        DEBUG_Text1.text = $"Wind speed km/h: {(windVelocity.magnitude * 3.6f).ToString("0")}";
        DEBUG_Text2.text = $"Player speed km/h: {(playerVelocity.magnitude * 3.6f).ToString("0")}";
        DEBUG_Text3.text = $"Air speed km/h: {(airSpeed * 3.6f).ToString("0")}";
    }
}
