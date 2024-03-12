using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Race : MonoBehaviour, IEvent
{
    [SerializeField] private RaceCheckpoint[] checkPoints;
    [SerializeField] private float startTime = 90;
    [SerializeField, Range(3, 5)] private int countdownLength = 5;
    [Space(20)]
    [SerializeField, Tooltip("In Player HUD")] private GameObject eventUiGameObject;
    [SerializeField, Tooltip("In Player HUD")] private TextMeshProUGUI countdownText;
    [SerializeField, Tooltip("In Player HUD")] private TextMeshProUGUI timerText;
    [Space(20)]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip countdownClip;
    [SerializeField] private AudioClip countdownFinalClip;
    [SerializeField] private AudioClip checkpointReachedClip;
    [SerializeField] private AudioClip raceWonClip;
    [SerializeField] private AudioClip raceLostClip;

    private float time;
    private int nextCheckpointIndex = 0;

    private bool isEventStarted = false;
    private bool isEventFinished = false;
    private bool canPause = false;
    private bool canUnpause = false;

    private WaitForSeconds waitOneSec = new WaitForSeconds(1);


    // ================ IEvent implementation ================
    public void RegisterToMainManager()
    {
        MainManager.Instance.RegisterOngoingEvent(this);
    }

    public void UnregisterToMainManager()
    {
        MainManager.Instance.UnregisterOngoingEvent(this);
    }

    public bool IsPauseAllowed()
    {
        return canPause;
    }

    public bool IsUnpauseAllowed()
    {
        return canUnpause;
    }

    public bool IsEventStarted()
    {
        return isEventStarted;
    }

    public bool IsEventFinished()
    {
        return isEventFinished;
    }
    //========================================================


    private void Awake()
    {
        time = startTime;
        //RegisterToMainManager();  // Should be here, but for now in Start() for debug purpose, to prevent null ref exception (main manager instance not initalized yet)
    }

    private void Start()
    {
        RegisterToMainManager();    // Will be in Awake()

        foreach (var point in checkPoints)
        {
            point.gameObject.SetActive(false);
        }
        if (checkPoints.Length > 1)
        {
            checkPoints[0].gameObject.SetActive(true);
            checkPoints[1].gameObject.SetActive(true);
        }

        countdownText.text = string.Empty;
        eventUiGameObject.SetActive(true);

        StartCoroutine(StartRaceCoroutine());

        checkPoints[0].YouAreNext();
    }


    private int lastMinuts = 0;
    private int lastSeconds = 0;
    private void UpdateTimeDisplay(float timeInSec)
    {
        int totalSeconds = (int)Mathf.Floor(timeInSec);
        int minuts = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        if (minuts != lastMinuts || seconds != lastSeconds)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (minuts < 10) stringBuilder.Append("0");
            stringBuilder.Append(minuts);
            stringBuilder.Append(":");
            if (seconds < 10) stringBuilder.Append("0");
            stringBuilder.Append(seconds);
            timerText.text = stringBuilder.ToString();
        }

        lastMinuts = minuts;
        lastSeconds = seconds;
    }


    private IEnumerator StartRaceCoroutine()
    {
        int timeLeft = countdownLength + 1;
        audioSource.clip = countdownClip;

        yield return waitOneSec;

        while (timeLeft > 1)
        {
            yield return waitOneSec;
            timeLeft--;

            countdownText.text = timeLeft.ToString("0");
            audioSource.Play();
        }

        yield return waitOneSec;

        countdownText.text = "GO";
        audioSource.clip = countdownFinalClip;
        audioSource.Play();

        isEventStarted = true;
        canPause = true;
        canUnpause = true;

        yield return waitOneSec;

        countdownText.gameObject.SetActive(false);
    }


    private void Update()
    {
        if (MainManager.Instance.isSimulationRunning)
        {
            time -= Time.deltaTime;

            if (time < 0f)
            {
                NoMoreTime();
            }
        }

        UpdateTimeDisplay(time);
    }


    public void CheckpointReached(RaceCheckpoint checkpoint)
    {
        if (!MainManager.Instance.isSimulationRunning)
        {
            return;
        }

        if (checkpoint == checkPoints[nextCheckpointIndex])
        {
            checkPoints[nextCheckpointIndex++].gameObject.SetActive(false);

            if (nextCheckpointIndex == checkPoints.Length)
            {
                RaceWon();
                return;
            }

            if (nextCheckpointIndex < checkPoints.Length)
            {
                checkPoints[nextCheckpointIndex].YouAreNext();
            }

            if (nextCheckpointIndex < checkPoints.Length - 1)
            {
                checkPoints[nextCheckpointIndex + 1].gameObject.SetActive(true);
            }

            audioSource.clip = checkpointReachedClip;
            audioSource.Play();
        }
    }


    private void RaceWon()
    {
        Debug.Log("Last checkpoint reached! Race won!");
        isEventFinished = true;
        audioSource.clip = raceWonClip;
        audioSource.Play();
        countdownText.text = "WON";
        countdownText.gameObject.SetActive(true);
        StartCoroutine(RaceEndCoroutine());
    }


    private void NoMoreTime()
    {
        Debug.Log("No more time! Race lost!");
        isEventFinished = true;
        audioSource.clip = raceLostClip;
        audioSource.Play();
        countdownText.text = "LOST";
        countdownText.gameObject.SetActive(true);
        StartCoroutine(RaceEndCoroutine());
    }


    IEnumerator RaceEndCoroutine()
    {
        canPause = false;
        yield return waitOneSec;
        yield return waitOneSec;
        countdownText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
        canUnpause = false;
        canPause = true;
        MainManager.Instance.GameMenu.Show();
    }


    private void OnDestroy()
    {
        UnregisterToMainManager();
    }
}
