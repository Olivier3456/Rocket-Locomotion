using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class SpeedParticles : MonoBehaviour
{
    [SerializeField] private AirMovements airMovements;
    
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private float speedThreshold = 10f;
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

        startSizeFactor = 0.01f / speedThreshold;
    }


    void Update()
    {
        float airSpeed = airMovements.Speed;

        //Debug.Log($"velocity square magnitude = {airSpeed}.");

        if (airSpeed < speedThreshold)
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

        Vector3 particleDirection = -airMovements.Direction;
        transform.forward = particleDirection;
        transform.position = transform.parent.position - particleDirection * particleSystemDistanceToCamera;

        float startSpeedFactor = 0.75f;
        mainModule.startSpeed = minParticleStartSpeed + airSpeed * startSpeedFactor;

        mainModule.startSize = Mathf.Clamp((airSpeed * startSizeFactor) - (speedThreshold * startSizeFactor), 0, 0.1f);

        float rateOverTimeFactor = 0.25f;
        emissionModule.rateOverTime = airSpeed * rateOverTimeFactor;
    }
}
