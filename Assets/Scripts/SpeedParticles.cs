using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class SpeedParticles : MonoBehaviour
{
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private float sqrSpeedThreshold = 100f;
    [SerializeField] private float minParticleStartSpeed = 10f;

    private ParticleSystem.MainModule mainModule;
    private ParticleSystem.EmissionModule emissionModule;

    private float particleSystemDistanceToCamera = 10f;

    private float startSizeFactor;

    private void Awake()
    {
        mainModule = particle.main;
        emissionModule = particle.emission;
    }

    void Start()
    {
        mainModule.startSpeed = 0f;
        mainModule.startSize = 0f;
        emissionModule.rateOverTime = 0f;

        startSizeFactor = 0.01f / sqrSpeedThreshold;
    }


    void Update()
    {
        float velocitySqrMagnitude = playerRb.velocity.sqrMagnitude;

        //Debug.Log($"velocity square magnitude = {velocitySqrMagnitude}.");

        if (velocitySqrMagnitude < sqrSpeedThreshold)
        {
            if (particle.isPlaying)
            {
                particle.Clear();
                particle.Stop();

                mainModule.startSpeed = 0f;
                mainModule.startSize = 0f;
                emissionModule.rateOverTime = 0f;
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

        float startSpeedFactor = 0.05f;
        mainModule.startSpeed = minParticleStartSpeed + velocitySqrMagnitude * startSpeedFactor;
        
        mainModule.startSize = Mathf.Clamp((velocitySqrMagnitude * startSizeFactor) - (sqrSpeedThreshold * startSizeFactor), 0, 0.1f);

        float rateOverTimeFactor = 0.025f;
        emissionModule.rateOverTime = velocitySqrMagnitude * rateOverTimeFactor;
    }
}
