using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    [SerializeField] private ControllerManager controllerManager;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] InputActionReference shotInputAction;
    [SerializeField] private float shotLength = 0.25f;
    [Space(20)]
    [SerializeField] private Transform raycastOrigin;
    [SerializeField] private float maxRaycastDistance = 250f;
    //[SerializeField] private Transform laserPoint;
    //[Space(20)]
    //[SerializeField] private LineRenderer lineRenderer;
    //[SerializeField] private float laserLength = 2f;
    [Space(20)]
    [SerializeField] private Transform impactParticlePrefab;
    [SerializeField] private int impactParticleInPool = 10;


    public UnityEvent OnShot = new UnityEvent();


    private Queue<GunImpactEffect> gunImpactEffect_In_Pool = new Queue<GunImpactEffect>();


    private float shotTimer = 0f;
    private bool isShooting = false;
    private bool hasNewShotInput = false;

    private const string ANIMATOR_SHOT_TRIGGER = "Shot";

    private void OnEnable()
    {
        shotInputAction.action.started += shotInputAction_started;
        shotInputAction.action.canceled += shotInputAction_canceled;
        shotInputAction.action.Enable();
    }
    private void OnDisable()
    {
        shotInputAction.action.Disable();
        shotInputAction.action.started -= shotInputAction_started;
        shotInputAction.action.canceled -= shotInputAction_canceled;
    }

    private void shotInputAction_started(InputAction.CallbackContext obj)
    {
        hasNewShotInput = true;
    }

    private void shotInputAction_canceled(InputAction.CallbackContext obj)
    {
        hasNewShotInput = false;
    }


    private void Start()
    {
        for (int i = 0; i < impactParticleInPool; i++)
        {
            GunImpactEffect gunImpactEffect = Instantiate(impactParticlePrefab.GetComponent<GunImpactEffect>());
            gunImpactEffect.Initialize(gunImpactEffect_In_Pool);
            gunImpactEffect.gameObject.SetActive(false);
            gunImpactEffect_In_Pool.Enqueue(gunImpactEffect);
        }
    }



    private void Update()
    {
        //===================================================================>> COMMENTé POUR LE DEBUG
        if (!MainManager.Instance.IsSimulationRunning)
        {
            return;
        }
        //===================================================================<<


        if (hasNewShotInput && !isShooting)
        {
            isShooting = true;
            hasNewShotInput = false;
            shotTimer = 0f;
            float pitch = Random.Range(0.8f, 1.1f);
            audioSource.pitch = pitch;
            audioSource.Play();
            animator.SetTrigger(ANIMATOR_SHOT_TRIGGER);
            OnShot.Invoke();



            Vector3 bulletDirection = GetApproximatedDirection(raycastOrigin.forward);

            if (Physics.Raycast(raycastOrigin.position, bulletDirection, out RaycastHit hit, maxRaycastDistance))
            {
                StartCoroutine(WaitForImpactCoroutine(hit.point, hit.normal, hit.distance));
            }
        }

        if (isShooting)
        {
            shotTimer += Time.deltaTime;

            if (shotTimer > shotLength)
            {
                isShooting = false;
            }
        }
    }


    private Vector3 GetApproximatedDirection(Vector3 initialDirection)
    {
        float maxAngleDeviation = 0.25f;
        float randomX = Random.Range(-maxAngleDeviation, maxAngleDeviation);
        float randomY = Random.Range(-maxAngleDeviation, maxAngleDeviation);
        float randomZ = Random.Range(-maxAngleDeviation, maxAngleDeviation);

        Quaternion randomRotation = Quaternion.Euler(randomX, randomY, randomZ);

        Vector3 adjustedDirection = randomRotation * initialDirection;

        return adjustedDirection.normalized;
    }




    private IEnumerator WaitForImpactCoroutine(Vector3 hitPoint, Vector3 hitNormal, float hitDistance)
    {
        float bulletSpeed = 700f;
        float delay = hitDistance / bulletSpeed;

        float timer = 0f;
        while (timer < delay)
        {
            yield return null;
            timer += Time.deltaTime;
        }

        if (gunImpactEffect_In_Pool.Count > 0)
        {
            GunImpactEffect gie = gunImpactEffect_In_Pool.Dequeue();
            gie.DoYourEffect(hitPoint, hitNormal, hitDistance);
        }
    }
}
