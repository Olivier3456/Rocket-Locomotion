using System.Collections;
using UnityEngine;

public class Wind : MonoBehaviour
{
    [SerializeField] private Vector3 initialDirection = new Vector3(1, 0, 0);
    [SerializeField] private float directionDelta = 100f;
    [Space(20)]
    [SerializeField] private float minimumSpeed = 10f;
    [SerializeField] private float maximumSpeed = 100f;
    [Space(20)]
    [SerializeField] private float speedChangeRate = 0.1f;
    [SerializeField] private float rotationChangeRate = 0.1f;


    private float perlinNoiseTimeSpeed = 0.0f;
    private float perlinNoiseTimeDirection = 0.0f;

    private float currentSpeed = 0f;
    private Vector3 currentDirection = Vector3.zero;
    private Vector3 windVector = Vector3.zero;

    public Vector3 Vector { get { return windVector; } }
    public Vector3 Direction { get { return currentDirection; } }
    public float Magnitude { get { return currentSpeed; } }
    public float MinimumSpeed { get { return minimumSpeed; } }
    public float MaximumSpeed { get { return maximumSpeed; } }


    private void Awake()
    {
        currentSpeed = minimumSpeed;

        initialDirection.y = 0.0f;  // The wind will only be horizontal
        initialDirection = initialDirection.normalized;
        currentDirection = initialDirection;

        if (minimumSpeed > maximumSpeed)
        {
            minimumSpeed = maximumSpeed;
        }
    }


    private void Update()
    {
        UpdateDirection();
        UpdateSpeed();
        windVector = currentDirection * currentSpeed;
    }

    private void UpdateDirection()
    {
        perlinNoiseTimeDirection += Time.deltaTime * rotationChangeRate;

        float perlinValue = Mathf.PerlinNoise(perlinNoiseTimeDirection, 0.5f);

        Quaternion rotation = Quaternion.Euler(0, perlinValue * directionDelta, 0);

        currentDirection = (rotation * initialDirection).normalized;
    }

    private void UpdateSpeed()
    {
        perlinNoiseTimeSpeed += Time.deltaTime * speedChangeRate;

        float perlinValue = Mathf.PerlinNoise(perlinNoiseTimeSpeed, 0.0f);

        currentSpeed = minimumSpeed + (perlinValue * (maximumSpeed - minimumSpeed));
    }
}
