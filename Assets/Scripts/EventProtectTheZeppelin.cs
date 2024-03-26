using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class DroneNavPoints
{
    public Transform spawnPoint;
    public Transform[] checkpoints;
}


public class EventProtectTheZeppelin : MonoBehaviour, IGameEvent
{
    [SerializeField] private GameEvent thisGameEvent;
    [SerializeField] private Drone dronePrefab;
    [SerializeField] private DroneNavPoints[] dronesNavPoints;
    [SerializeField] private float dronesSpawnStartRate = 10f;

    private static EventProtectTheZeppelin instance;


    // =========================== Interface implementation ===========================
    public bool IsEventFinished()
    {
        return false;
    }

    public bool IsEventStarted()
    {
        return true;
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
    // ================================================================================


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Only one instance of EventProtectTheZeppelin is allowed! Destroying new one");
            Destroy(gameObject);
            return;
        }

        RegisterToMainManager();
    }


    void Start()
    {
        StartCoroutine(Coroutine_SpawnDronesRepetitively());
    }    


    private IEnumerator Coroutine_SpawnDronesRepetitively()
    {
        float timer = dronesSpawnStartRate;

        while (true)
        {
            yield return null;

            if (MainManager.Instance.IsSimulationRunning)
            {
                timer += Time.deltaTime;
            }


            if (timer > dronesSpawnStartRate)
            {
                timer = 0f;
                SpawnNewDrone();
            }
        }
    }

    private void SpawnNewDrone()
    {
        int randomSpawnPointIndex = UnityEngine.Random.Range(0, dronesNavPoints.Length);
        DroneNavPoints navPoints = dronesNavPoints[randomSpawnPointIndex];
        Transform spawnPoint = navPoints.spawnPoint;
        Drone newDrone = Instantiate(dronePrefab, spawnPoint.position, spawnPoint.rotation);
        int startLife = 3;
        float speed = 10f;
        newDrone.Initialize(navPoints.checkpoints, startLife, speed);
    }

    private void OnDestroy()
    {
        instance = null;
        UnregisterToMainManager();
    }
}
