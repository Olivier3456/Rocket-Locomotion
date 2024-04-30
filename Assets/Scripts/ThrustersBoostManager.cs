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
    [SerializeField] private bool raceCheckpointsRechargeBoost = true;


    private const float maxReserve = 1f;
    private float currentReserve;
    public float MaxReserve { get { return maxReserve; } }
    public float CurrentReserve { get { return currentReserve; } }

    private float currentBoostValueLeft = 0;
    private float currentBoostValueRight = 0;

    public UnityEvent<bool, bool> OnCanBoostStatusChange = new UnityEvent<bool, bool>();    // First bool = can boost, second bool = status changed by a game event (ex: race checkpoint reached).

    private bool isDepleted;
    public bool IsDepleted { get { return isDepleted; } }

    private void Awake()
    {
        currentReserve = maxReserve;

        if (raceCheckpointsRechargeBoost)
        {
            RaceCheckpoint.OnCheckpointReached.AddListener(OnRaceCheckpointReached);
        }
    }

    private void OnRaceCheckpointReached(RaceCheckpoint raceCheckpoint)
    {
        currentReserve = MaxReserve;
        isDepleted = false;
        OnCanBoostStatusChange.Invoke(true, true);
    }


    private void Update()
    {
        if (MainManager.Instance.IsSimulationRunning)
        {
            if (currentReserve < maxReserve && currentBoostValueLeft == 0 && currentBoostValueRight == 0)
            {
                currentReserve += Time.deltaTime * reserveGainPerSec;
            }

            if (currentReserve <= 0 && !isDepleted)
            {
                //Debug.Log("Boost reserve depleted");
                isDepleted = true;
                OnCanBoostStatusChange.Invoke(false, false);
            }
            else if (currentReserve > minimumReserveForNewBoost && isDepleted)
            {
                //Debug.Log("Player can boost again");
                isDepleted = false;
                OnCanBoostStatusChange.Invoke(true, false);
            }
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

    private void OnDestroy()
    {
        if (raceCheckpointsRechargeBoost)
        {
            RaceCheckpoint.OnCheckpointReached.RemoveListener(OnRaceCheckpointReached);
        }
    }
}
