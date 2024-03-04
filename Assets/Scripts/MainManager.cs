using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance { get; private set; }
    public GameMenu MainMenu { get; private set; }
    public bool IsPlayerAlive { get; private set; }
    public bool IsPaused { get; private set; }

    public UnityEvent<bool> OnPauseStatusChanged = new UnityEvent<bool>();

    public UnityEvent OnDeath = new UnityEvent();

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

        MainMenu = FindObjectOfType<GameMenu>();

        SceneManager.sceneLoaded += SceneManager_sceneLoaded;


        // =============== DEBUG ===============
        IsPlayerAlive = true;
    }

    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode)
    {
        MainMenu = FindObjectOfType<GameMenu>();
    }

    public void Pause(bool pause)
    {
        IsPaused = pause;
        OnPauseStatusChanged.Invoke(pause);
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



}
