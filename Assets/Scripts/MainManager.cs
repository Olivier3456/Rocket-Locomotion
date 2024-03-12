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
    public bool isSimulationRunning { get { return ongoingEvent == null ? IsPlayerAlive && !IsPaused : IsPlayerAlive && !IsPaused && ongoingEvent.IsEventStarted() && !ongoingEvent.IsEventFinished(); } }
    public bool CanPause { get { return (!IsPlayerAlive || (ongoingEvent != null && !ongoingEvent.IsPauseAllowed())) ? false : true; } }
    public bool CanUnpause { get { return (!IsPlayerAlive || (ongoingEvent != null && !ongoingEvent.IsUnpauseAllowed())) ? false : true; } }
    public bool IsPaused { get; private set; }

    public UnityEvent<bool> OnPauseStatusChanged = new UnityEvent<bool>();
    public UnityEvent OnDeath = new UnityEvent();

    private MySceneManager mySceneManager;

    private IEvent ongoingEvent;


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
    }


    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameMenu = FindObjectOfType<GameMenu>();
        IsPlayerAlive = true;
        IsPaused = false;
    }


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


    public void LoadScene(int buildIndex)
    {
        if (mySceneManager == null)
        {
            mySceneManager = gameObject.AddComponent<MySceneManager>();
        }

        mySceneManager.LoadScene(buildIndex);
    }


    public void RegisterOngoingEvent(IEvent IEvent)
    {
        if (ongoingEvent == null)
        {
            ongoingEvent = IEvent;
            Debug.Log("OngoingEvent registered to MainManager.");
        }
        else
        {
            Debug.Log("OngoingEvent is not null, can't register two at a time.");
        }
    }

    public void UnregisterOngoingEvent(IEvent IEvent)
    {
        if (ongoingEvent == null)
        {
            Debug.Log("Can't unregister ongoingEvent: it is already null.");
        }
        else if (ongoingEvent != IEvent)
        {
            Debug.Log("An ongoingEvent can't be unregistered by another ongoingEvent.");
        }
        else
        {
            ongoingEvent = null;
            Debug.Log("OngoingEvent unregistered to MainManager.");
        }
    }
}
