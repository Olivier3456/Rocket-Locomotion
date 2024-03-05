using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class WindIndicator : MonoBehaviour
{
    [SerializeField] private Transform windIndicatorVisual;
    [SerializeField] private TextMeshProUGUI speedText;
    [Space(20)]
    [SerializeField] private bool changeScale;
    [SerializeField, Tooltip("The length of the arrow when the wind is at its maximum value.")] private float maximumLocalScale = 2f;
    [Space(20)]
    [SerializeField] private float indicatorYAngleOffset = 270f;
    [Space(20)]
    [SerializeField] private Thruster leftThruster;
    [SerializeField] private Thruster rightThruster;

    private Wind wind;
    private AudioSource windAudioSource;

    private float refreshDelay = 1f;
    private float refreshTimer = 0;

    private float scaleUnit;

    private Transform camTransform;


    void Start()
    {
        wind = FindObjectOfType<Wind>();

        if (wind == null)
        {
            Debug.Log("No wind in the scene. Destroying all objects related to wind indications");
            Destroy(windIndicatorVisual.gameObject);
            Destroy(speedText.gameObject);
            Destroy(windAudioSource.gameObject);
            Destroy(this);
            return;
        }

        scaleUnit = maximumLocalScale / wind.MaximumSpeed;

        camTransform = Camera.main.transform;

        windAudioSource = wind.gameObject.GetComponentInChildren<AudioSource>();
    }


    void Update()
    {
        if (wind == null)
        {
            return;
        }

        Vector3 direction = wind.Direction;
        Quaternion YAngleOffset = Quaternion.Euler(0, indicatorYAngleOffset, 0);
        float speed = wind.Magnitude;

        UpdateWindIndicator(direction, YAngleOffset, speed);
        DisplayWindSpeed(speed);
        UpdateWindAudioSource(direction, speed);
    }


    private void UpdateWindAudioSource(Vector3 windDirection, float speed)
    {
        float distanceFromCam = 1f;
        windAudioSource.transform.position = camTransform.position - (windDirection * distanceFromCam);

        float minPitch = 0.5f;
        float pitchSpeedMultiplier = 0.002f;
        windAudioSource.pitch = minPitch + (speed * pitchSpeedMultiplier);

        float volumeSpeedMultiplier = 0.001f;
        windAudioSource.volume = (speed * volumeSpeedMultiplier) - Mathf.Max(leftThruster.ThrustValue, rightThruster.ThrustValue); // We don't ear wind if thrusters are in action.
    }

    private void UpdateWindIndicator(Vector3 direction, Quaternion YAngleOffset, float speed)
    {
        if (changeScale)
        {
            float xLocalScale = speed * scaleUnit;
            windIndicatorVisual.localScale = new Vector3(xLocalScale, windIndicatorVisual.localScale.y, windIndicatorVisual.localScale.z);
        }

        windIndicatorVisual.rotation = Quaternion.LookRotation(YAngleOffset * direction);
    }

    private void DisplayWindSpeed(float speed)
    {
        refreshTimer += Time.deltaTime;
        if (refreshTimer > refreshDelay)
        {
            refreshTimer = 0f;
            float speedDisplayed = speed * 0.1f;
            speedText.text = speedDisplayed.ToString("0");
        }
    }
}
