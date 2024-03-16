using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Borders : MonoBehaviour
{
    private static int bordersEntered = 0;

    private const string PLAYER_TAG = "Player";

    public static UnityEvent OnBorderEntered = new UnityEvent();
    public static UnityEvent OnBorderExited = new UnityEvent();

    //private void Start()
    //{
    //    MainManager.Instance.OnGameEventRestarted.AddListener(OnGameEventRestarted);
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PLAYER_TAG))
        {
            bordersEntered++;

            if (bordersEntered == 1)
            {
                OnBorderEntered.Invoke();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(PLAYER_TAG))
        {
            bordersEntered--;

            if (bordersEntered == 0)
            {
                OnBorderExited.Invoke();
            }
        }
    }

    private void OnDestroy()
    {
        bordersEntered = 0;
        //MainManager.Instance.OnGameEventRestarted.RemoveListener(OnGameEventRestarted);
    }

    //private void OnGameEventRestarted()
    //{
    //    bordersEntered = 0;
    //}
}
