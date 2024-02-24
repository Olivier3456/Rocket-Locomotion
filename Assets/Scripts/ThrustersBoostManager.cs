using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrustersBoostManager : MonoBehaviour
{
    [SerializeField] private Thruster leftThruster;
    [SerializeField] private Thruster rightThruster;
    [SerializeField, Range(0, 1)] private float reserveGainPerSec = 0.1f;
    [SerializeField] private float boostTotalLenghtFullValue = 3f;

    private float maxReserve = 1f;
    private float leftCurrentReserve;
    private float rightCurrentReserve;

    private void Update()
    {
        if (leftCurrentReserve < maxReserve)
        {
            leftCurrentReserve += Time.deltaTime * reserveGainPerSec;
        }
        if (rightCurrentReserve < maxReserve)
        {
            rightCurrentReserve += Time.deltaTime * reserveGainPerSec;
        }

        if (leftCurrentReserve <= 0)
        {
            leftThruster.AuthoriseBoost(false);
        }
        else
        {
            leftThruster.AuthoriseBoost(true);
        }

        if (rightCurrentReserve <= 0)
        {
            rightThruster.AuthoriseBoost(false);
        }
        else
        {
            rightThruster.AuthoriseBoost(true);
        }
    }

    public void Boost(Thruster thruster, float boostValue)
    {
        if (thruster == leftThruster)
        {

        }
        else
        {

        }
    }
}
