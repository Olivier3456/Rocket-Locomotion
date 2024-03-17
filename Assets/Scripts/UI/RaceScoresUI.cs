using System;
using System.Text;
using TMPro;
using UnityEngine;
using static RaceResultsSaveLoad;

public class RaceScoresUI : MonoBehaviour
{
    [SerializeField] private EventRace race;
    [SerializeField] private RectTransform raceScoresRectTransform;
    [SerializeField] private TextMeshProUGUI scoreTextTemplate;

    private bool isScoreAddedToBoard;
    private float distanceFromCamera = 2f;


    private void Start()
    {
        raceScoresRectTransform.gameObject.SetActive(false);

        if (race == null)
        {
            race = FindObjectOfType<EventRace>();
        }

        if (race != null)
        {
            race.OnRaceOver.AddListener(OnRaceOver);
        }
    }


    private void OnRaceOver(RaceScores raceScores)
    {
        foreach (var raceScore in raceScores.scores)
        {
            TextMeshProUGUI newScoreText = Instantiate(scoreTextTemplate, scoreTextTemplate.transform.parent);
            newScoreText.text = raceScore.date + " --- " + MakeTimeDisplay(raceScore.time);

            if (raceScore == ScoreAdded)
            {
                isScoreAddedToBoard = true;
                newScoreText.color = Color.red;
                newScoreText.fontSize *= 1.25f;
            }

            newScoreText.gameObject.SetActive(true);
        }

        if (!isScoreAddedToBoard)
        {
            TextMeshProUGUI newScoreText = Instantiate(scoreTextTemplate, scoreTextTemplate.transform.parent);
            RaceScore scoreAdded = ScoreAdded;
            newScoreText.text = scoreAdded.date + " --- " + MakeTimeDisplay(scoreAdded.time);
            newScoreText.color = Color.red;
            newScoreText.fontSize *= 1.25f;
            newScoreText.gameObject.SetActive(true);
        }

        Show();
    }



    private string MakeTimeDisplay(float timeInSec)
    {
        int totalSeconds = (int)Math.Floor(timeInSec);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        int milliseconds = (int)((timeInSec - totalSeconds) * 1000);

        return $"{minutes}:{seconds:D2}:{milliseconds:D2}";
    }


    private void Show()
    {
        Transform cameraTransform = Camera.main.transform;
        raceScoresRectTransform.position = cameraTransform.position + cameraTransform.forward * distanceFromCamera;
        raceScoresRectTransform.forward = cameraTransform.forward;
        raceScoresRectTransform.gameObject.SetActive(true);
    }


    private void OnDestroy()
    {
        if (race != null)
        {
            race.OnRaceOver.RemoveListener(OnRaceOver);
        }
    }
}
