using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerLife : MonoBehaviour
{
    [SerializeField] private float startLife = 100f;
    [SerializeField] private float lifePointsRecoveredPerSec = 5f;
    [SerializeField] private float relativeVelocityThresholdToLoseLife = 7f;
    [SerializeField] private float lifePointsLostPerVelocityUnit = 5f;

    [SerializeField] private AudioSource playerAudioSource;
    [SerializeField] private AudioClip injuryAudioClip;
    [SerializeField] private AudioClip deathAudioClip;

    [SerializeField] private Material redSpriteMaterial;
    [SerializeField] private float minMaskAmount = 0.6f;
    [SerializeField] private float maxMaskAmount = 1f;


    private float currentLife;

    private bool isAlive = true;
    public bool IsAlive { get { return isAlive; } }



    [SerializeField] private CollisionDetector collisionDetector;

    public UnityEvent OnDeath = new UnityEvent();

    public UnityEvent<float, float> OnLifeLost = new UnityEvent<float, float>();

    private void Awake()
    {
        collisionDetector.OnCollision.AddListener(OnCollision);
        currentLife = startLife;
    }

    private void OnDisable()
    {
        collisionDetector.OnCollision.RemoveListener(OnCollision);
        redSpriteMaterial.SetFloat("_MaskAmount", maxMaskAmount);
    }

    private void Start()
    {
        UpdateRedCanvasGroupAlpha();
    }

    private void OnCollision(GameObject other, Vector3 relativeVelocity, Vector3 collisionNormal)
    {
        float relativeVelocityMagnitude = relativeVelocity.magnitude;
        Vector3 relativeVelocityDirection = relativeVelocity.normalized;
        float dot = Vector3.Dot(collisionNormal, relativeVelocityDirection);
        float collisionForce = relativeVelocityMagnitude * dot;

        Debug.Log($"Relative velocity magnitude: {relativeVelocityMagnitude}. Dot product of relative velocity direction and collisionNormal: {dot}. Collision force: {collisionForce}");


        if (collisionForce > relativeVelocityThresholdToLoseLife)
        {
            float lifeLost = relativeVelocityMagnitude * lifePointsLostPerVelocityUnit;
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
}
