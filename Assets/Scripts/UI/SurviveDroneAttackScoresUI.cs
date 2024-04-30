using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static GameEventResultsManager;


// ===================>> This class is a copy of RaceScoresUI. Needs refacto. <<===================


public class SurviveDroneAttackScoresUI : MonoBehaviour
{
    [SerializeField] private EventSurviveDroneAttack eventSurviveDroneAttack;
    [SerializeField] private RectTransform scoresRectTransform;
    [SerializeField] private TextMeshProUGUI scoreTextTemplate;

    private bool isScoreAddedToBoard;
    private float distanceFromCamera = 2f;


    private void Start()
    {
        scoresRectTransform.gameObject.SetActive(false);

        if (eventSurviveDroneAttack == null)
        {
            eventSurviveDroneAttack = FindObjectOfType<EventSurviveDroneAttack>();
        }

        if (eventSurviveDroneAttack != null)
        {
            eventSurviveDroneAttack.OnSurviveEventOver.AddListener(OnEventOver);
        }
    }


    private void OnEventOver(SurviveDroneAttackScores eventScores)
    {
        foreach (var score in eventScores.scores)
        {
            TextMeshProUGUI newScoreText = Instantiate(scoreTextTemplate, scoreTextTemplate.transform.parent);
            newScoreText.text = score.date + " --- " + score.kills + " kills";

            if (score == SurviveDroneAttackScoreAdded)
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
            SurviveDroneAttackScore scoreAdded = SurviveDroneAttackScoreAdded;
            newScoreText.text = scoreAdded.date + " --- " + scoreAdded.kills + " kills";
            newScoreText.color = Color.red;
            newScoreText.fontSize *= 1.25f;
            newScoreText.gameObject.SetActive(true);
        }

        Show();
    }


    private void Show()
    {
        Transform cameraTransform = Camera.main.transform;
        scoresRectTransform.position = cameraTransform.position + cameraTransform.forward * distanceFromCamera;
        scoresRectTransform.forward = cameraTransform.forward;
        scoresRectTransform.gameObject.SetActive(true);
    }


    private void OnDestroy()
    {
        if (eventSurviveDroneAttack != null)
        {
            eventSurviveDroneAttack.OnSurviveEventOver.RemoveListener(OnEventOver);
        }
    }
}
