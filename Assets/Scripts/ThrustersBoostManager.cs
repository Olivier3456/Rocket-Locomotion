using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrustersBoostManager : MonoBehaviour
{
    [SerializeField] private Thruster leftThruster;
    [SerializeField] private Thruster rightThruster;
    [SerializeField, Tooltip("Total lenght of the boost when the player presses the button entirely (boostValue = 1)")] private float boostLenght = 3f;
    [SerializeField, Range(0, 1)] private float reserveGainPerSec = 0.1f;
    [SerializeField, Range(0, 1)] private float minimumReserveToBoost = 0.5f;

    private const float maxReserve = 1f;
    private float leftCurrentReserve;
    private float rightCurrentReserve;


    private void Awake()
    {
        leftCurrentReserve = maxReserve;
        rightCurrentReserve = maxReserve;
    }


    private void Update()
    {
        if (leftCurrentReserve < maxReserve)
        {
            leftCurrentReserve += Time.deltaTime * reserveGainPerSec;
            //Debug.Log("Left current boost reserve augmented. Current value: " + leftCurrentReserve);
        }
        if (rightCurrentReserve < maxReserve)
        {
            rightCurrentReserve += Time.deltaTime * reserveGainPerSec;
        }

        if (leftCurrentReserve <= 0)
        {
            leftThruster.AuthoriseBoost(false);
        }
        else if (leftCurrentReserve > minimumReserveToBoost)
        {
            leftThruster.AuthoriseBoost(true);
        }

        if (rightCurrentReserve <= 0)
        {
            rightThruster.AuthoriseBoost(false);
        }
        else if (rightCurrentReserve > minimumReserveToBoost)
        {
            rightThruster.AuthoriseBoost(true);
        }
    }

    public void Boost(Thruster thruster, float boostValue)
    {
        if (thruster == leftThruster)
        {
            leftCurrentReserve -= (Time.deltaTime / boostLenght) * boostValue;
        }
        else
        {
            rightCurrentReserve -= (Time.deltaTime / boostLenght) * boostValue;
        }
    }
}
