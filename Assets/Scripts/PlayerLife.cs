using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerLife : MonoBehaviour
{
    [SerializeField] private PhysicalContactsManager physicalContactsManager;
    [Space(20)]
    [SerializeField] private float startLife = 100f;
    [SerializeField] private float lifePointsRecoveredPerSec = 5f;
    //[SerializeField] private float dangerousVelocityDifference = 100f;
    //[SerializeField] private float lifePointsLostPerVelocityDifferenceUnit = 0.2f;
    [SerializeField] private float relativeVelocityThresholdToLoseLife = 5f;
    [SerializeField] private float lifePointsLostPerCollisionForceUnit = 1f;
    [Space(20)]
    [SerializeField] private AudioSource playerAudioSource;
    [SerializeField] private AudioClip injuryAudioClip;
    [SerializeField] private AudioClip deathAudioClip;
    [Space(20)]
    [SerializeField] private Material redSpriteMaterial;
    [SerializeField] private float minMaskAmount = 0.6f;
    [SerializeField] private float maxMaskAmount = 1f;
    [Space(20)]
    public UnityEvent OnDeath = new UnityEvent();
    public UnityEvent<float, float> OnLifeLost = new UnityEvent<float, float>();


    private float currentLife;

    private bool isAlive = true;
    public bool IsAlive { get { return isAlive; } }


    private void Awake()
    {
        //physicalContactsManager.OnPhysicalShock.AddListener(OnCollision);
        physicalContactsManager.OnCollision.AddListener(OnCollision);
        currentLife = startLife;
    }

    private void OnDisable()
    {
        //physicalContactsManager.OnPhysicalShock.RemoveListener(OnCollision);
        redSpriteMaterial.SetFloat("_MaskAmount", maxMaskAmount);
    }

    private void Start()
    {
        //physicalContactsManager.SetDangerousVelocityDifference(dangerousVelocityDifference);
        UpdateRedCanvasGroupAlpha();
    }


    //private void OnCollision(float velocityDifference)
    //{
    //    float lifeLost = velocityDifference * lifePointsLostPerVelocityDifferenceUnit;
    //    currentLife -= lifeLost;
    //    OnLifeLost.Invoke(lifeLost, currentLife);

    //    UpdateRedCanvasGroupAlpha();

    //    if (currentLife <= 0)
    //    {
    //        isAlive = false;
    //        OnDeath.Invoke();
    //        playerAudioSource.PlayOneShot(deathAudioClip);
    //        Debug.Log($"Player is dead. Life lost: {lifeLost}. Current life: {currentLife}");
    //    }
    //    else
    //    {
    //        playerAudioSource.PlayOneShot(injuryAudioClip);
    //        Debug.Log($"Player is injured. Life lost: {lifeLost}. Current life: {currentLife}");
    //    }
    //}


    private void Update()
    {
        if (isAlive && currentLife < startLife)
        {
            currentLife += Time.deltaTime * lifePointsRecoveredPerSec;
            currentLife = Mathf.Clamp(currentLife, 0f, 100f);
            UpdateRedCanvasGroupAlpha();
        }
    }


    private void UpdateRedCanvasGroupAlpha()
    {
        minMaskAmount = 0.6f;
        maxMaskAmount = 1f;
        float difference = maxMaskAmount - minMaskAmount;
        float maskAmount = maxMaskAmount - (((startLife - currentLife) / startLife) * difference);
        redSpriteMaterial.SetFloat("_MaskAmount", maskAmount);
    }





    // The legacy method to detect injuries, before the collider placement, which disable collisions detection on the collider. Was a listener of the event OnCollision of CollisionDectector.

    private void OnCollision(GameObject other, Vector3 relativeVelocity, Vector3 collisionNormal)
    {
        if (currentLife < 0)
        {
            return;
        }

        float relativeVelocityMagnitude = relativeVelocity.magnitude;
        Vector3 relativeVelocityDirection = relativeVelocity.normalized;
        float dot = Vector3.Dot(collisionNormal, relativeVelocityDirection);
        float collisionForce = relativeVelocityMagnitude * dot;

        Debug.Log($"Relative velocity magnitude: {relativeVelocityMagnitude}. Dot product of relative velocity direction and collisionNormal: {dot}. Collision force: {collisionForce}");

        if (collisionForce > relativeVelocityThresholdToLoseLife)
        {
            float lifeLost = collisionForce * lifePointsLostPerCollisionForceUnit;
            currentLife -= lifeLost;
            OnLifeLost.Invoke(lifeLost, currentLife);

            UpdateRedCanvasGroupAlpha();

            if (currentLife <= 0)
            {
                isAlive = false;
                OnDeath.Invoke();
                playerAudioSource.PlayOneShot(deathAudioClip);
                Debug.Log($"Player is dead. Life lost: {lifeLost}. Current life: {currentLife}");
            }
            else
            {
                playerAudioSource.PlayOneShot(injuryAudioClip);
                Debug.Log($"Player is injured. Life lost: {lifeLost}. Current life: {currentLife}");
            }
        }
    }
}
