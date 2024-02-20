using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedParticles : MonoBehaviour
{
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private float sqrSpeedThreshold = 50f;
    [SerializeField] private float minParticleStartSpeed = 10f;
    [SerializeField] private float minParticleEmissionRateOverTime = 10f;

    private ParticleSystem.MainModule mainModule;
    private ParticleSystem.EmissionModule emissionModule;

    private float particleSystemDistanceToCamera = 10f;

    private void Awake()
    {
        mainModule = particle.main;
        emissionModule = particle.emission;
    }

    void Start()
    {
        mainModule.startSpeed = 0f;
    }


    void Update()
    {
        float velocitySqrMagnitude = playerRb.velocity.sqrMagnitude;

        //Debug.Log($"velocity square magnitude = {velocitySqrMagnitude}.");

        if (velocitySqrMagnitude < sqrSpeedThreshold)
        {
            mainModule.startSpeed = 0f;
            emissionModule.rateOverTime = 0f;

            if (particle.isPlaying)
            {
                particle.Clear();
                particle.Stop();
            }

            return;
        }

        if (!particle.isPlaying)
        {
            particle.Play();
        }

        //Debug.Log("Speed particules emission.");

        Vector3 particleDirection = -playerRb.velocity.normalized;
        transform.forward = particleDirection;
        transform.position = transform.parent.position - particleDirection * particleSystemDistanceToCamera;

        mainModule.startSpeed = minParticleStartSpeed + velocitySqrMagnitude * 0.05f;
        emissionModule.rateOverTime = velocitySqrMagnitude * 0.025f;
    }
}
