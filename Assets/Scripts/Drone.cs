using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour, IBreakableByGun
{
    [SerializeField] private Transform[] checkpoints;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float minDistanceToChangeCheckpoint = 2f;
    [Space(20)]
    [SerializeField] bool movementRandomFluctuations;
    [SerializeField] float fluctuationAmplitude = 0.1f;
    [SerializeField] private float fluctuationChangeSpeed = 0.25f;
    [Space(20)]
    [SerializeField] private GameObject droneVisual;
    [SerializeField] private ParticleSystem explosionParticles;
    [SerializeField] private ParticleSystem bulletImpactParticles;
    [Space(20)]
    [SerializeField] private AudioSource damageAudioSource;
    [SerializeField] private AudioClip[] bulletImpactClips;
    [SerializeField] private AudioClip explosionClip;
    [Space(20)]
    [SerializeField, Range(1, 10)] private int startLife = 5;

    private float perlin_X = 0f;

    private int currentCheckpointIndex = 0;

    private Vector3 targetPosition;

    private int currentLife;

    private bool isDead;


    private void Start()
    {
        targetPosition = checkpoints[0].position;
        currentLife = startLife;
    }

    private void Update()
    {
        // DEBUG
        //if (Input.GetKeyDown(KeyCode.Space)) { Explode(); }


        Vector3 nextCheckpointPosition = checkpoints[currentCheckpointIndex].position;

        UpdateTargetPosition(nextCheckpointPosition);

        UpdateDronePositionRotation();

        ChangeNextCheckpointIfNeeded(nextCheckpointPosition);
    }

    private void UpdateTargetPosition(Vector3 nextCheckpointPosition)
    {
        float lerpSpeedRatio = 0.8f;    // To ensure targetPosition is always ahead of transform.position.
        float lerp = 1 / (Vector3.Distance(targetPosition, nextCheckpointPosition) * lerpSpeedRatio / speed / Time.deltaTime);
        targetPosition = Vector3.Lerp(targetPosition, nextCheckpointPosition, lerp);
    }

    private void UpdateDronePositionRotation()
    {
        Vector3 movementDirection = (targetPosition - transform.position).normalized;
        Vector3 physicalDirection = new Vector3(movementDirection.x, 0, movementDirection.z).normalized;    // The drone is always horizontal.
        transform.forward = physicalDirection;

        Vector3 movementVector = movementDirection * speed * Time.deltaTime + RandomFluctuationsMovement();

        transform.Translate(movementVector, Space.World);
    }

    private void ChangeNextCheckpointIfNeeded(Vector3 nextCheckpointPosition)
    {
        float distanceToNextCheckpoint = Vector3.Distance(transform.position, nextCheckpointPosition);

        if (distanceToNextCheckpoint < minDistanceToChangeCheckpoint)
        {
            currentCheckpointIndex = currentCheckpointIndex < checkpoints.Length - 1 ? currentCheckpointIndex + 1 : 0;
        }
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
        StartCoroutine(ExpodeCoroutine());
    }

    private IEnumerator ExpodeCoroutine()
    {
        explosionParticles.Play();
        damageAudioSource.clip = explosionClip;
        damageAudioSource.Play();

        while (explosionParticles.isPlaying || damageAudioSource.isPlaying)
        {
            yield return null;
        }

        //gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
