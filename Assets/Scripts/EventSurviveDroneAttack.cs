using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static GameEventResultsManager;

public class EventSurviveDroneAttack : MonoBehaviour, IGameEvent
{
    // ==================== IGameEvent interface implementation ====================
    public bool IsEventStarted()
    {
        return true;
    }

    public bool IsEventFinished()
    {
        return !MainManager.Instance.IsPlayerAlive;
    }

    public bool IsPauseAllowed()
    {
        return true;
    }

    public bool IsUnpauseAllowed()
    {
        return MainManager.Instance.IsPlayerAlive;
    }

    public void RegisterToMainManager()
    {
        MainManager.Instance.RegisterOngoingEvent(this);
    }

    public void UnregisterToMainManager()
    {
        MainManager.Instance.UnregisterOngoingEvent(this);
    }
    // ==================== End of IGameEvent interface implementation ====================

    [SerializeField] private GameEvent thisGameEvent;
    [SerializeField] private float initialDroneSpawnDelay = 0f;
    [SerializeField] private float startDronesSpawnInterval = 10f;
    [SerializeField] private float minDronesSpawnInterval = 5f;
    [SerializeField] private float dronesSpawnIntervalReduction = 0.1f;
    [Space(20)]
    [SerializeField] private GameObject droneSeekerPrefab;
    [Space(20)]
    [SerializeField] private Transform player;
    [Space(20)]
    [SerializeField] private Transform[] droneSeekerSpawnPoints;
    [Space(20)]
    [SerializeField] private int dronesStartLife = 3;
    [SerializeField] private float dronesSpeedWhenTargetFound = 30f;
    [SerializeField] private float dronesSpeedWhenNoTargetFound = 10f;
    [SerializeField] private float dronesExplosionForce = 50f;
    [Space(20)]
    [SerializeField, Tooltip("In Player HUD")] private GameObject eventUiGameObject;
    //[SerializeField, Tooltip("In Player HUD")] private TextMeshProUGUI countdownText;
    [SerializeField, Tooltip("In Player HUD")] private TextMeshProUGUI scoreText;

    private float currentIntervaleBetweenTwoDronesSpawn;
    private float droneSpawnTimer = 0f;

    public UnityEvent<SurviveDroneAttackScores> OnSurviveEventOver = new UnityEvent<SurviveDroneAttackScores>();

    private int score = 0;

    
    void Start()
    {
        currentIntervaleBetweenTwoDronesSpawn = startDronesSpawnInterval;
        droneSpawnTimer += startDronesSpawnInterval - (startDronesSpawnInterval - initialDroneSpawnDelay);

        for (int i = 0; i < 10; i++)
        {
            InstantiateNewDrone();
        }
        
        eventUiGameObject.SetActive(true);
        scoreText.text = $"killed: {score}";

        DroneSeeker.OnKilled.AddListener(OnDroneSeekerKilled);
        MainManager.Instance.OnDeath.AddListener(PlayerIsDead);

        RegisterToMainManager();
    }


    private void OnDisable()
    {
        DroneSeeker.OnKilled.RemoveListener(OnDroneSeekerKilled);
        MainManager.Instance.OnDeath.RemoveListener(PlayerIsDead);
    }


    private void OnDroneSeekerKilled()
    {
        ++score;
        scoreText.text = $"killed: {score}";
    }



    private void Update()
    {
        if (IsEventStarted() && !IsEventFinished())
        {
            droneSpawnTimer += Time.deltaTime;

            if (droneSpawnTimer > currentIntervaleBetweenTwoDronesSpawn)
            {
                if (InstantiateNewDrone())
                {
                    droneSpawnTimer = 0f;

                    if (currentIntervaleBetweenTwoDronesSpawn > minDronesSpawnInterval)
                    {
                        currentIntervaleBetweenTwoDronesSpawn -= dronesSpawnIntervalReduction;
                    }
                }
            }
        }        
    }


    private void PlayerIsDead()
    {
        MainManager.Instance.GameMenu.Show();
        SurviveDroneAttackScores scores = AddDroneAttackScore(thisGameEvent, score);

        OnSurviveEventOver.Invoke(scores);
    }


    private bool InstantiateNewDrone()
    {
        int randomIndex = Random.Range(0, droneSeekerSpawnPoints.Length);
        Vector3 spawnPosition = droneSeekerSpawnPoints[randomIndex].position;

        DroneSeeker ds = Instantiate(droneSeekerPrefab, spawnPosition, Quaternion.identity).GetComponent<DroneSeeker>();
        ds.Initialize(player, dronesStartLife, dronesSpeedWhenTargetFound, dronesSpeedWhenNoTargetFound, dronesExplosionForce);

        return true;
    }



    private void OnDestroy()
    {
        UnregisterToMainManager();
    }





    // For instantiating the drones near the player
    //private bool InstantiateNewDrone()
    //{
    //    int number_Of_Spawn_Points_In_Player_Range = DroneSeekerSpawnPoint.DroneSeekerSpawnPointsInPlayerRange.Count;

    //    if (number_Of_Spawn_Points_In_Player_Range < 1)
    //    {
    //        //Debug.Log("No drone seeker spawn point in player range. Can't instantiate a drone.");
    //        return false;
    //    }

    //    int randomIndex = Random.Range(0, number_Of_Spawn_Points_In_Player_Range);
    //    Vector3 spawnPosition = DroneSeekerSpawnPoint.DroneSeekerSpawnPointsInPlayerRange[randomIndex].transform.position;

    //    DroneSeeker ds = Instantiate(droneSeekerPrefab, spawnPosition, Quaternion.identity).GetComponent<DroneSeeker>();
    //    ds.Initialize(player, 2, 40f, 20f);

    //    return true;
    //}
}
