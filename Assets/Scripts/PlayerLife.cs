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
    public UnityEvent<float, float, float> OnLifeLost = new UnityEvent<float, float, float>();

    public float RelativeVelocityThresholdToLoseLife { get { return relativeVelocityThresholdToLoseLife; } }

    private float currentLife;

    private const string MASK_AMOUNT = "_MaskAmount";

    private void Awake()
    {
        physicalContactsManager.OnCollision.AddListener(OnCollision);
        currentLife = startLife;
    }

    private void OnDisable()
    {
        redSpriteMaterial.SetFloat(MASK_AMOUNT, maxMaskAmount);
    }

    private void Start()
    {
        UpdateRedCanvasGroupAlpha();
    }


    private void Update()
    {
        if (MainManager.Instance.IsPlayerAlive && currentLife < startLife)
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
        redSpriteMaterial.SetFloat(MASK_AMOUNT, maskAmount);
    }


    private void OnCollision(float collisionForce)
    {
        if (!MainManager.Instance.isSimulationRunning)
        {
            return;
        }

        if (collisionForce > relativeVelocityThresholdToLoseLife)
        {
            float lifeLost = collisionForce * lifePointsLostPerCollisionForceUnit;
            currentLife -= lifeLost;
            OnLifeLost.Invoke(lifeLost, currentLife, startLife);

            UpdateRedCanvasGroupAlpha();

            if (currentLife <= 0)
            {
                //isAlive = false;                
                MainManager.Instance.PlayerDeath();
                playerAudioSource.PlayOneShot(deathAudioClip);
                //Debug.Log($"Player is dead. Life lost: {lifeLost}. Current life: {currentLife}");
            }
            else
            {
                playerAudioSource.PlayOneShot(injuryAudioClip);
                //Debug.Log($"Player is injured. Life lost: {lifeLost}. Current life: {currentLife}");
            }
        }
    }
}
