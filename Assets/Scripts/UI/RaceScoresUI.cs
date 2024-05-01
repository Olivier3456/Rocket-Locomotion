using System;
using System.Text;
using TMPro;
using UnityEngine;
using static GameEventResultsManager;

public class RaceScoresUI : MonoBehaviour
{
    [SerializeField] private RectTransform raceScoresRectTransform;
    [SerializeField] private TextMeshProUGUI scoreTextTemplate;

    private EventRace race;
    
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
            newScoreText.text = raceScore.date + "   ---   Time: " + MakeTimeDisplay(raceScore.time);

            if (raceScore == RaceScoreAdded)
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
            RaceScore scoreAdded = RaceScoreAdded;
            newScoreText.text = scoreAdded.date + "   ---   Time: " + MakeTimeDisplay(scoreAdded.time);
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
        Vector3 directionFromCamera = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
        raceScoresRectTransform.position = cameraTransform.position + (directionFromCamera * distanceFromCamera);
        raceScoresRectTransform.forward = directionFromCamera;
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
