using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance { get; private set; }
    public GameMenu GameMenu { get; private set; }
    public bool IsPlayerAlive { get; private set; }
    public bool IsSimulationRunning { get { return OngoingEvent == null ? IsPlayerAlive && !IsPaused : IsPlayerAlive && !IsPaused && OngoingEvent.IsEventStarted() && !OngoingEvent.IsEventFinished(); } }
    public bool CanPause { get { return (!IsPlayerAlive || (OngoingEvent != null && !OngoingEvent.IsPauseAllowed())) ? false : true; } }
    public bool CanUnpause { get { return (!IsPlayerAlive || (OngoingEvent != null && !OngoingEvent.IsUnpauseAllowed())) ? false : true; } }
    public bool IsPaused { get; private set; }

    public UnityEvent<bool> OnPauseStatusChanged = new UnityEvent<bool>();
    public UnityEvent OnDeath = new UnityEvent();
    //public UnityEvent OnGameEventRestarted = new UnityEvent();

    public IGameEvent OngoingEvent { get; private set; }
    //public IGameEvent EventToLoad { get; private set; }

    private MySceneManager mySceneManager;



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        GameMenu = FindObjectOfType<GameMenu>();

        SceneManager.sceneLoaded += SceneManager_sceneLoaded;


        // =============== DEBUG ===============
        IsPlayerAlive = true;

        //TestRaceResults();
    }



    //private void TestRaceResults()
    //{
    //    for (int i = 0; i < 20; i++)
    //    {
    //        RaceResultsSaveLoad.AddRaceScore(GameEvent.Race1, UnityEngine.Random.Range(0f, 100f));
    //    }
    //}


    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode)
    {
        IsPlayerAlive = true;
        IsPaused = false;

        GameMenu = FindObjectOfType<GameMenu>();

        //OngoingEvent = null;

        //if (scene.buildIndex == MySceneManager.LOADING_SCENE_BUILD_INDEX)
        //{
        //    return;
        //}
        //else if (scene.buildIndex == MySceneManager.MAIN_MENU_SCENE_BUILD_INDEX)
        //{
        //    EventToLoad = null;
        //    return;
        //}

        //StartGameEvent();
    }

    //private void StartGameEvent()
    //{
    //    if (EventToLoad != null)
    //    {
    //        switch (EventToLoad)
    //        {
    //            case EventRace:
    //                OngoingEvent = Instantiate(EventToLoad as EventRace);
    //                break;
    //            case EventNone:
    //                OngoingEvent = Instantiate(EventToLoad as EventNone);
    //                break;
    //        }
    //    }

    //    GameMenu = FindObjectOfType<GameMenu>();
    //}

    //public void RestartGameEvent()
    //{
    //    IsPlayerAlive = true;
    //    IsPaused = false;
    //    DestroyOnGoingEvent();
    //    StartGameEvent();
    //    OnGameEventRestarted.Invoke();
    //}

    //private void DestroyOnGoingEvent()
    //{
    //    if (OngoingEvent != null)
    //    {
    //        switch (OngoingEvent)
    //        {
    //            case EventRace:
    //                Destroy((OngoingEvent as EventRace).gameObject);
    //                break;
    //            case EventNone:
    //                Destroy((OngoingEvent as EventNone).gameObject);
    //                break;
    //        }
    //    }
    //}

    public bool Pause(bool pause)
    {
        if (pause && !CanPause)
        {
            return false;
        }
        if (!pause && !CanUnpause)
        {
            return false;
        }

        IsPaused = pause;
        OnPauseStatusChanged.Invoke(pause);
        return true;
    }



    public void PlayerDeath()
    {
        if (IsPlayerAlive)
        {
            IsPlayerAlive = false;
            OnDeath.Invoke();
        }
    }



    //public void LoadScene(int buildIndex, IGameEvent gameEvent = null)
    //{
    //    EventToLoad = gameEvent;

    //    if (EventToLoad == null && buildIndex != MySceneManager.MAIN_MENU_SCENE_BUILD_INDEX)
    //    {
    //        Debug.LogError("Only the Main Menu Scene can be loaded without a Game Event!");
    //        return;
    //    }

    //    if (mySceneManager == null)
    //    {
    //        mySceneManager = gameObject.AddComponent<MySceneManager>();
    //    }

    //    mySceneManager.LoadScene(buildIndex);
    //}

    public void LoadScene(int buildIndex)
    {
        if (mySceneManager == null)
        {
            mySceneManager = gameObject.AddComponent<MySceneManager>();
        }

        mySceneManager.LoadScene(buildIndex);
    }


    public void RegisterOngoingEvent(IGameEvent IEvent)
    {
        if (OngoingEvent == null)
        {
            OngoingEvent = IEvent;
            Debug.Log("OngoingEvent registered to MainManager.");
        }
        else
        {
            Debug.Log("OngoingEvent is not null, can't register two at a time.");
        }
    }

    public void UnregisterOngoingEvent(IGameEvent IEvent)
    {
        if (OngoingEvent == null)
        {
            Debug.Log("Can't unregister OngoingEvent: it is already null.");
        }
        else if (OngoingEvent != IEvent)
        {
            Debug.Log("An OngoingEvent can't be unregistered by another OngoingEvent.");
        }
        else
        {
            OngoingEvent = null;
            Debug.Log("OngoingEvent unregistered to MainManager.");
        }
    }
}
