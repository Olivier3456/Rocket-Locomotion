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

    [SerializeField] private CanvasGroup redCanvasGroup;


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
    }

    private void OnCollision(GameObject other, float relativeVelocity)
    {
        if (relativeVelocity > relativeVelocityThresholdToLoseLife)
        {
            float lifeLost = relativeVelocity * lifePointsLostPerVelocityUnit;
            currentLife -= lifeLost;
            OnLifeLost.Invoke(lifeLost, currentLife);

            Debug.Log($"Player is injured. Life lost: {lifeLost}. Current life: {currentLife}");

            UpdateRedCanvasGroupAlpha();

            if (currentLife <= 0)
            {
                isAlive = false;
                OnDeath.Invoke();
                playerAudioSource.PlayOneShot(deathAudioClip);
            }
            else
            {
                playerAudioSource.PlayOneShot(injuryAudioClip);
            }
        }
    }



    private void Update()
    {
        if (isAlive && currentLife < startLife)
        {
            currentLife += Time.deltaTime * lifePointsRecoveredPerSec;
            currentLife = Mathf.Clamp(currentLife, 0, 100);
            UpdateRedCanvasGroupAlpha();

            Debug.Log($"Life: {currentLife}");
        }
    }


    private void UpdateRedCanvasGroupAlpha()
    {
        float alpha = 1 - (currentLife / startLife);
        redCanvasGroup.alpha = alpha;
    }
}
