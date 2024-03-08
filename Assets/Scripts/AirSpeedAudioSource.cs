using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirSpeedAudioSource : MonoBehaviour
{
    [SerializeField] private AirMovements airMovements;
    [Space(20)]
    [SerializeField] private Thruster leftThruster;
    [SerializeField] private Thruster rightThruster;

    private AudioSource audioSource;

    private Transform camTransform;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        camTransform = Camera.main.transform;
    }

    void Update()
    {
        UpdateWindAudioSource();
    }


    private void UpdateWindAudioSource()
    {
        float distanceFromCam = 1f;        

        audioSource.transform.position = camTransform.position - (airMovements.Direction * distanceFromCam);

        float minPitch = 0.5f;
        float pitchAirSpeedMultiplier = 0.0075f;
        audioSource.pitch = minPitch + (airMovements.Speed * pitchAirSpeedMultiplier);

        float volumeAirSpeedMultiplier = 0.015f;
        audioSource.volume = (airMovements.Speed * volumeAirSpeedMultiplier) - Mathf.Max(leftThruster.ThrustValue, rightThruster.ThrustValue); // We don't ear wind if thrusters are in action.
    }
}
