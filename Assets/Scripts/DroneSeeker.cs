using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DroneSeeker : MonoBehaviour, IBreakableByGun
{
    [SerializeField] private float speedTargetFound = 10f;
    [SerializeField] private float speedNoTarget = 5f;
    [SerializeField] private float droneRadius = 2f;
    [SerializeField] private Transform target;
    [Space(20)]
    [SerializeField] bool movementRandomFluctuations;
    [SerializeField] float fluctuationAmplitude = 0.1f;
    [SerializeField] private float fluctuationChangeSpeed = 0.25f;
    [Space(20)]
    [SerializeField] private GameObject droneVisual;
    [SerializeField] private ParticleSystem explosionParticles;
    [SerializeField] private ParticleSystem bulletImpactParticles;
    [Space(20)]
    [SerializeField] private AudioSource rotorsAudioSource;
    [SerializeField] private AudioSource alarmAudioSource;
    [SerializeField] private AudioSource damageAudioSource;
    [SerializeField] private AudioClip[] bulletImpactClips;
    [SerializeField] private AudioClip explosionClip;
    [Space(20)]
    [SerializeField] private float explosionForce = 50f;


    private float perlin_X = 0f;

    private int currentLife;

    private bool isDead;

    private bool isTargetInSight;

    private Vector3 currentDestination;

    private bool isWaitingForDestination = true;

    private bool isMovingTowardsRandomDestination = true;


    public void Initialize(Transform target, int startLife, float speedTargetFound, float speedNoTarget, float explosionForce)
    {
        this.target = target;
        this.speedTargetFound = speedTargetFound;
        this.speedNoTarget = speedNoTarget;
        this.explosionForce = explosionForce;

        currentLife = startLife;
    }


    private void Update()
    {
        if (!isDead)
        {
            if (target != null)
            {
                Vector3 direction = (target.position - transform.position).normalized;
                float maxDistance = Vector3.Distance(transform.position, target.position);

                if (Physics.SphereCast(transform.position, droneRadius, direction, out RaycastHit hit, maxDistance))
                {
                    if (hit.transform == target)
                    {
                        isTargetInSight = true;
                        isWaitingForDestination = false;
                        isMovingTowardsRandomDestination = false;
                        currentDestination = hit.point;
                    }
                    else
                    {
                        isTargetInSight = false;
                    }

                    if (isWaitingForDestination)
                    {
                        isWaitingForDestination = !FindRandomDestination(out currentDestination);
                    }

                    if (!isWaitingForDestination)
                    {
                        MoveTowardsDestination(currentDestination);
                    }

                    float distanceToCurrentDestination = Vector3.Distance(transform.position, currentDestination);

                    if (distanceToCurrentDestination < droneRadius)
                    {
                        if (!isTargetInSight)
                        {
                            //Debug.Log("Drone arrived at its actual random destination. Trying to find another one");
                            isWaitingForDestination = !FindRandomDestination(out currentDestination);
                        }
                        else
                        {
                            IBreakableByDrone bbd = target.GetComponentInChildren<IBreakableByDrone>();
                            if (bbd != null)
                            {
                                bbd.TakeDamage(explosionForce);
                            }

                            Explode();
                        }
                    }
                    //else
                    //{
                    //    Debug.Log("There is a problem: sphere cast should have found the target or another collider.");
                    //}

                    UpdateAudioSourcesValues(maxDistance);
                }
            }            
        }
        else
        {
            rotorsAudioSource.Stop();
            alarmAudioSource.Stop();
        }
    }


    private void MoveTowardsDestination(Vector3 destination)
    {
        float currentSpeed = !isMovingTowardsRandomDestination ? speedTargetFound : speedNoTarget;

        Vector3 direction = (destination - transform.position).normalized;

        Vector3 movementVector = direction * currentSpeed * Time.deltaTime + RandomFluctuationsMovement();

        UpdateDroneVisualDirection(direction);
        transform.Translate(movementVector, Space.World);
    }


    private bool FindRandomDestination(out Vector3 destination)
    {
        float randomX = Random.Range(-1f, 1f);
        float randomY = Random.Range(-0.1f, 0.1f);
        float randomZ = Random.Range(-1f, 1f);
        Vector3 randomDirection = new Vector3(randomX, randomY, randomZ).normalized;
        float distance = 20f;
        isMovingTowardsRandomDestination = true;

        if (!Physics.SphereCast(transform.position, droneRadius, randomDirection, out RaycastHit hit, distance))
        {
            //Debug.Log("Target Location is OK: no obstacle on the way.");
            destination = transform.position + (randomDirection * distance);
            return true;
        }
        else
        {
            //Debug.Log("An obstacle is on the way to target location.");
            destination = Vector3.zero;
            return false;
        }
    }


    private void UpdateDroneVisualDirection(Vector3 movementDirection)
    {
        Vector3 targetDirection = new Vector3(movementDirection.x, 0, movementDirection.z).normalized;    // y = 0 because the drone is always horizontal, even when it goes up or down.
                                                                                                          //transform.forward = targetDirection;
        transform.forward = Vector3.Slerp(transform.forward, targetDirection, Time.deltaTime);
    }


    private Vector3 RandomFluctuationsMovement()
    {
        if (movementRandomFluctuations)
        {
            perlin_X += Time.deltaTime * fluctuationChangeSpeed;

            float X = Mathf.PerlinNoise(perlin_X, 0.0f) * Time.deltaTime * fluctuationAmplitude;
            float Y = Mathf.PerlinNoise(perlin_X, 0.33f) * Time.deltaTime * fluctuationAmplitude;
            float Z = Mathf.PerlinNoise(perlin_X, 0.66f) * Time.deltaTime * fluctuationAmplitude;

            return new Vector3(X, Y, Z);
        }
        else
        {
            return Vector3.zero;
        }
    }


    public void TakeDamage(RaycastHit hit, float damage)
    {
        if (--currentLife < 1)
        {
            if (!isDead)
            {
                Explode();
            }
        }
        else
        {
            if (!isDead)
            {
                ImpactEffect(hit);
            }
        }
    }


    private void ImpactEffect(RaycastHit hit)
    {
        bulletImpactParticles.transform.position = hit.point;
        bulletImpactParticles.transform.forward = hit.normal;
        bulletImpactParticles.Play();

        damageAudioSource.clip = bulletImpactClips[Random.Range(0, bulletImpactClips.Length)];
        damageAudioSource.Play();
    }


    private void Explode()
    {
        isDead = true;
        droneVisual.SetActive(false);
        StartCoroutine(ExplodeCoroutine());
    }
    private IEnumerator ExplodeCoroutine()
    {
        explosionParticles.Play();
        damageAudioSource.clip = explosionClip;
        damageAudioSource.Play();

        while (explosionParticles.isPlaying || damageAudioSource.isPlaying)
        {
            yield return null;
        }

        Destroy(gameObject);
    }


    private void UpdateAudioSourcesValues(float rayLength)
    {
        float baseRotorPitch = 1.5f;
        float pitchSpeedFactor = 0.1f;
        float currentSpeed = !isMovingTowardsRandomDestination ? speedTargetFound : speedNoTarget;
        float rotorPitch = baseRotorPitch + (currentSpeed * pitchSpeedFactor);
        rotorsAudioSource.pitch = rotorPitch;

        if (isTargetInSight)
        {
            float distanceToTarget = rayLength;
            float minPitch = 1f;
            float maxPitch = 3f;
            float alarmPitch = Mathf.Clamp(maxPitch - (distanceToTarget * 0.05f), minPitch, maxPitch);
            alarmAudioSource.pitch = alarmPitch;
            if (!alarmAudioSource.isPlaying)
            {
                alarmAudioSource.Play();
            }
        }
        else
        {
            alarmAudioSource.Stop();
        }
    }
}
