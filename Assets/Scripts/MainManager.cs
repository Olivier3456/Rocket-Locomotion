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
    public IGameEvent OngoingEvent { get; private set; }

    public WindParameters windParameters = null;

    private MySceneManager mySceneManager;

    private Rigidbody playerRb;

    private Vector3 playerVelocityBeforeImmobilisation = Vector3.zero;


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
        IsPlayerAlive = true;
        IsPaused = false;
    }


    public void SetPlayerRigidbody(Rigidbody playerRb)
    {
        this.playerRb = playerRb;
    }
    public void SetGameMenu(GameMenu gameMenu)
    {
        GameMenu = gameMenu;
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

        ImmobilizePlayer(IsPaused);

        OnPauseStatusChanged.Invoke(pause);
        return true;
    }


    public void ImmobilizePlayer(bool immobilize)
    {
        if (immobilize)
        {
            playerVelocityBeforeImmobilisation = playerRb.velocity;
            playerRb.isKinematic = true;
        }
        else
        {
            playerRb.isKinematic = false;
            playerRb.velocity = playerVelocityBeforeImmobilisation;
        }
    }


    public void PlayerDeath()
    {
        if (IsPlayerAlive)
        {
            GameObject.FindWithTag("Environment").SetActive(false);
            IsPlayerAlive = false;
            ImmobilizePlayer(true);
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
