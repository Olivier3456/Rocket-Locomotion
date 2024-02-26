using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ThrustersBoostManager : MonoBehaviour
{
    [SerializeField] private Thruster thrusterLeft;
    [SerializeField] private Thruster thrusterRight;
    [SerializeField, Tooltip("Total lenght of the boost when the player presses the button entirely (boostValue = 1)")] private float boostLenght = 3f;
    [SerializeField, Range(0, 1)] private float reserveGainPerSec = 0.1f;
    [SerializeField, Range(0, 1)] private float minimumReserveForNewBoost = 0.5f;


    private const float maxReserve = 1f;
    private float currentReserve;
    public float MaxReserve { get { return maxReserve; } }
    public float CurrentReserve { get { return currentReserve; } }

    private float currentBoostValueLeft = 0;
    private float currentBoostValueRight = 0;

    public UnityEvent OnDepleted = new UnityEvent();
    public UnityEvent OnCanBoostAgain = new UnityEvent();

    private bool isDepleted;
    public bool IsDepleted { get { return isDepleted; } }

    private void Awake()
    {
        currentReserve = maxReserve;
    }


    private void Update()
    {
        if (currentReserve < maxReserve && currentBoostValueLeft == 0 && currentBoostValueRight == 0)
        {
            currentReserve += Time.deltaTime * reserveGainPerSec;
        }

        if (currentReserve <= 0 && !isDepleted)
        {
            isDepleted = true;
            thrusterLeft.AuthoriseBoost(false);
            thrusterRight.AuthoriseBoost(false);
            OnDepleted.Invoke();
            Debug.Log("Boost reserve depleted");
        }
        else if (currentReserve > minimumReserveForNewBoost && isDepleted)
        {
            isDepleted = false;
            thrusterLeft.AuthoriseBoost(true);
            thrusterRight.AuthoriseBoost(true);
            OnCanBoostAgain.Invoke();
            Debug.Log("Player can boost again");
        }
    }

    public void Boost(Thruster thruster, float boostValue)
    {
        if (thruster == thrusterLeft)
        {
            currentBoostValueLeft = boostValue;
        }
        else
        {
            currentBoostValueRight = boostValue;
        }

        currentReserve -= (Time.deltaTime / boostLenght) * boostValue * 0.5f;
    }
}
