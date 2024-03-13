using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderUI : MonoBehaviour
{
    [SerializeField] private GameObject borderColliderImageGameObject;
    [SerializeField] private AudioSource playerAudioSource;
    [SerializeField] private AudioClip borderColliderAudioClip;

    private WaitForSeconds wait1 = new WaitForSeconds(0.33f);
    private WaitForSeconds wait2 = new WaitForSeconds(0.8f);

    private bool coroutineRunning = false;

    private bool isBorderEntered = false;

    void Start()
    {
        borderColliderImageGameObject.SetActive(false);
        Borders.OnBorderEntered.AddListener(OnBorderEntered);
        Borders.OnBorderExited.AddListener(OnBorderExited);
    }


    private void OnBorderEntered()
    {
        isBorderEntered = true;

        if (!coroutineRunning)
        {
            StartCoroutine(BorderColliderCoroutine());
        }
    }

    private void OnBorderExited()
    {
        isBorderEntered = false;
    }

    IEnumerator BorderColliderCoroutine()
    {
        coroutineRunning = true;

        while (isBorderEntered)
        {
            if (MainManager.Instance.IsSimulationRunning)
            {
                playerAudioSource.PlayOneShot(borderColliderAudioClip);
                borderColliderImageGameObject.SetActive(true);
                yield return wait1;
                borderColliderImageGameObject.SetActive(false);
            }

            yield return wait2;
        }

        coroutineRunning = false;
    }

    private void OnDestroy()
    {
        Borders.OnBorderEntered.RemoveListener(OnBorderEntered);
        Borders.OnBorderExited.RemoveListener(OnBorderExited);
    }
}
