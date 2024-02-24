using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrusterBoostManager : MonoBehaviour
{
    [SerializeField] private Thruster thruster;
    [SerializeField, Tooltip("Total lenght of the boost when the player presses the button entirely (boostValue = 1)")] private float boostLenght = 3f;
    [SerializeField, Range(0, 1)] private float reserveGainPerSec = 0.1f;
    [SerializeField, Range(0, 1)] private float minimumReserveForNewBoost = 0.5f;

    private const float maxReserve = 1f;
    private float currentReserve;
    public float MaxReserve { get { return maxReserve; } }
    public float CurrentReserve { get { return currentReserve; } }

    private float currentBoostValue = 0;

    private void Awake()
    {
        currentReserve = maxReserve;
    }


    private void Update()
    {
        if (currentReserve < maxReserve && currentBoostValue == 0)
        {
            currentReserve += Time.deltaTime * reserveGainPerSec;
        }

        if (currentReserve <= 0)
        {
            thruster.AuthoriseBoost(false);
        }
        else if (currentReserve > minimumReserveForNewBoost)
        {
            thruster.AuthoriseBoost(true);
        }
    }

    public void Boost(float boostValue)
    {
        currentBoostValue = boostValue;
        currentReserve -= (Time.deltaTime / boostLenght) * boostValue;
    }
}
