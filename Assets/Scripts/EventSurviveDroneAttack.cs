using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSurviveDroneAttack : MonoBehaviour, IGameEvent
{
    // ==================== IGameEvent interface implementation ====================
    public bool IsEventStarted()
    {
        return true;
    }

    public bool IsEventFinished()
    {
        return false;
    }

    public bool IsPauseAllowed()
    {
        return true;
    }

    public bool IsUnpauseAllowed()
    {
        return true;
    }

    public void RegisterToMainManager()
    {
        MainManager.Instance.RegisterOngoingEvent(this);
    }

    public void UnregisterToMainManager()
    {
        MainManager.Instance.UnregisterOngoingEvent(this);
    }
    // ==================== End IGameEvent interface implementation ====================


    [SerializeField]
    private float initialDroneSpawnDelay = 0f;
    [SerializeField] private float startDronesSpawnInterval = 10f;
    [SerializeField] private float minDronesSpawnInterval = 5f;
    [SerializeField] private float dronesSpawnIntervalReduction = 0.1f;
    [Space(20)]
    [SerializeField] private GameObject droneSeekerPrefab;
    [Space(20)]
    [SerializeField] private Transform player;


    private float currentIntervaleBetweenTwoDronesSpawn;
    private float droneSpawnTimer = 0f;




    void Start()
    {
        currentIntervaleBetweenTwoDronesSpawn = startDronesSpawnInterval;
        droneSpawnTimer += startDronesSpawnInterval - (startDronesSpawnInterval - initialDroneSpawnDelay);
    }

    private void Update()
    {
        if (IsEventStarted() && !IsEventFinished())
        {
            droneSpawnTimer += Time.deltaTime;

            if (droneSpawnTimer > currentIntervaleBetweenTwoDronesSpawn)
            {
                droneSpawnTimer = 0f;
                InstantiateNewDrone();

                if (currentIntervaleBetweenTwoDronesSpawn > minDronesSpawnInterval)
                {
                    currentIntervaleBetweenTwoDronesSpawn -= dronesSpawnIntervalReduction;
                }


            }



        }
    }


    private void InstantiateNewDrone()
    {
        DroneSeeker ds = Instantiate(droneSeekerPrefab).GetComponent<DroneSeeker>();
        ds.Initialize(player, 3, 50f, 20f);
    }




}
