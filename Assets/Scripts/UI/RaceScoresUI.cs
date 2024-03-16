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


    private void Awake()
    {
        race.OnRaceOver.AddListener(OnRaceOver);
        raceScoresRectTransform.gameObject.SetActive(false);
    }
    private void Start()
    {
        if (race == null)
        {
            race = FindObjectOfType<EventRace>();
        }
    }


    private void OnRaceOver(RaceScores raceScores)
    {
        foreach (var raceScore in raceScores.scores)
        {
            TextMeshProUGUI newScoreText = Instantiate(scoreTextTemplate, scoreTextTemplate.transform.parent);
            newScoreText.text = raceScore.date + " --- " + raceScore.time.ToString("0.00");

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
            newScoreText.text = scoreAdded.date + " --- " + scoreAdded.time.ToString("0.00");
            newScoreText.color = Color.red;
            newScoreText.fontSize *= 1.25f;
            newScoreText.gameObject.SetActive(true);
        }

        Show();
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
        race.OnRaceOver.RemoveListener(OnRaceOver);
    }
}
