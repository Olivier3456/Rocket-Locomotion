using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnvironmentAudioSource : MonoBehaviour
{
    [SerializeField] private Thruster leftThruster;
    [SerializeField] private Thruster rightThruster;
    private AudioSource audioSource;
    private float initialVolume;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        initialVolume = audioSource.volume;
    }

    void Update()
    {
        audioSource.volume = initialVolume - (initialVolume * Mathf.Max(leftThruster.ThrustValue, rightThruster.ThrustValue));
    }
}
